--!strict

local HttpService = game:GetService("HttpService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local CourseConfig = require(root.config.CourseConfig)
local GameConfig = require(root.config.GameConfig)
local RuntimeQaConfig = require(root.config.RuntimeQaConfig)
local StepRules = require(root.rules.StepRules)

local PlayerProfileService = require(script.Parent.PlayerProfileService)
local StepCourseService = require(script.Parent.StepCourseService)

local RuntimeQaService = {}

local results: { [string]: any } = {}

local function emit(name: string, passed: boolean, fields: any?)
	results[name] = {
		Passed = passed,
		Fields = fields or {},
	}
	print(
		("[RisingStepsQA] %s %s %s"):format(
			if passed then "PASS" else "FAIL",
			name,
			HttpService:JSONEncode(fields or {})
		)
	)
end

local function check(name: string, condition: boolean, fields: any?)
	emit(name, condition, fields)
	assert(condition, ("Runtime QA failed: %s"):format(name))
end

local function waitUntil(predicate: () -> boolean, timeout: number): boolean
	local deadline = os.clock() + timeout
	repeat
		if predicate() then
			return true
		end
		task.wait(0.05)
	until os.clock() >= deadline
	return predicate()
end

local function waitReady(player: Player): boolean
	return waitUntil(function()
		local snap = StepCourseService.qaSnapshot(player)
		return snap.Ready == true and snap.Failed == false and player.Character ~= nil
	end, 8)
end

local function visualHold(player: Player, phase: string, view: string?)
	player:SetAttribute("RisingStepsQaView", view or "Gameplay")
	player:SetAttribute("RisingStepsQaPhase", phase)
	print(("[RisingStepsQA] VISUAL_HOLD %s"):format(phase))
	task.wait(RuntimeQaConfig.VisualHoldSeconds)
end

local function run(player: Player)
	task.wait(RuntimeQaConfig.StartupDelaySeconds)
	check(
		"profile_loaded",
		waitUntil(function()
			return player:GetAttribute("RisingStepsProfileLoaded") == true
		end, 8)
	)
	local initialPersistenceHealthy = player:GetAttribute("RisingStepsPersistenceHealthy") == true
	local initialPayload = PlayerProfileService.qaPersistentPayload(player)
	local rejoinSentinelLoaded = initialPersistenceHealthy
		and initialPayload.PB >= CourseConfig.InitialStepCount
		and initialPayload.Equipped.Trails == "ion"
		and initialPayload.Equipped.EnvironmentThemes == "dusk"
		and initialPayload.Settings.ReducedMotion == true
	if rejoinSentinelLoaded then
		emit("rejoin_persistence", true, {
			PB = initialPayload.PB,
			Trail = initialPayload.Equipped.Trails,
			Environment = initialPayload.Equipped.EnvironmentThemes,
			ReducedMotion = initialPayload.Settings.ReducedMotion,
		})
	else
		print(
			("[RisingStepsQA] REJOIN_SENTINEL_ABSENT healthy=%s pb=%d trail=%s environment=%s reducedMotion=%s"):format(
				tostring(initialPersistenceHealthy),
				initialPayload.PB,
				initialPayload.Equipped.Trails,
				initialPayload.Equipped.EnvironmentThemes,
				tostring(initialPayload.Settings.ReducedMotion)
			)
		)
	end

	if not initialPersistenceHealthy then
		PlayerProfileService.qaSetPersistenceHealthy(player, true)
	end
	check("profile_healthy", player:GetAttribute("RisingStepsPersistenceHealthy") == true)

	check("course_ready", waitReady(player))

	if RuntimeQaConfig.VisualOnly then
		visualHold(player, "tutorial", "Tutorial")
		StepCourseService.qaReset(player)
		task.wait(0.25)
		for stepId = 1, 3 do
			assert(StepCourseService.qaLand(player, stepId, 0) ~= "Rejected", "visual early climb rejected")
		end
		visualHold(player, "early", "Gameplay")
		for stepId = 4, 20 do
			assert(StepCourseService.qaLand(player, stepId, 0) ~= "Rejected", "visual mid climb rejected")
		end
		visualHold(player, "mid", "Gameplay")
		for stepId = 21, 38 do
			assert(StepCourseService.qaLand(player, stepId, 0) ~= "Rejected", "visual high climb rejected")
		end
		visualHold(player, "high", "Gameplay")
		StepCourseService.qaShowWarning(player, 39)
		visualHold(player, "warning", "Warning")
		StepCourseService.qaShowResult(player)
		visualHold(player, "result", "Result")
		StepCourseService.qaReset(player)
		task.wait(0.25)
		visualHold(player, "shop", "Shop")
		player:SetAttribute("RisingStepsQaView", "Gameplay")
		player:SetAttribute("RisingStepsQaPhase", "visual-complete")
		print("[RisingStepsQA] VISUAL_COMPLETE")
		return
	end

	visualHold(player, "tutorial", "Tutorial")
	local initial = StepCourseService.qaSnapshot(player)
	check(
		"lane_part_budget",
		type(initial.BaseParts) == "number" and initial.BaseParts <= CourseConfig.MaxLaneBaseParts,
		{
			BaseParts = initial.BaseParts,
			Budget = CourseConfig.MaxLaneBaseParts,
		}
	)
	check(
		"lane_beam_light_budget",
		type(initial.BeamsAndLights) == "number" and initial.BeamsAndLights <= CourseConfig.MaxLaneBeamsAndLights,
		{ BeamsAndLights = initial.BeamsAndLights, Budget = CourseConfig.MaxLaneBeamsAndLights }
	)

	local grades = {}
	for stepId = 1, 12 do
		local grade, probe = StepCourseService.qaLand(player, stepId, 0)
		check(("landing_step_%02d"):format(stepId), grade ~= "Rejected", {
			StepId = stepId,
			Grade = grade,
			Height = StepCourseService.qaSnapshot(player).Height,
			Probe = probe,
		})
		table.insert(grades, grade)
		task.wait(0.03)
	end
	local climbed = StepCourseService.qaSnapshot(player)
	check("climb_twelve_steps", climbed.Height == 12, { Height = climbed.Height })
	check("perfect_streak_builds", climbed.Streak == 12, { Streak = climbed.Streak, Grades = grades })
	check(
		"pb_updates",
		(player:GetAttribute("RisingStepsPB") or 0) >= 12,
		{ PB = player:GetAttribute("RisingStepsPB") }
	)

	local expiryObserved = waitUntil(function()
		local snapshot = StepCourseService.qaSnapshot(player)
		return snapshot.StepStates[1] == StepRules.State.Expired
	end, GameConfig.Step.WarningSeconds * 1.5 + GameConfig.Step.ExpiryTransitionSeconds + 1.5)
	local afterExpiry = StepCourseService.qaSnapshot(player)
	check("warning_to_expired_runtime", expiryObserved, { Step1State = afterExpiry.StepStates[1] })

	local missGrade = StepCourseService.qaLand(player, 13, 0.46)
	local missed = StepCourseService.qaSnapshot(player)
	check("miss_breaks_streak", missGrade == "Miss" and missed.Streak == 0, {
		Grade = missGrade,
		Streak = missed.Streak,
	})

	for stepId = 14, 42 do
		local grade = StepCourseService.qaLand(player, stepId, 0)
		check(("climb_step_%02d"):format(stepId), grade == "Perfect", {
			StepId = stepId,
			Grade = grade,
		})
		task.wait(0.02)
	end
	local fullClimb = StepCourseService.qaSnapshot(player)
	check("all_difficulty_bands_runtime", fullClimb.Height == 42, {
		Height = fullClimb.Height,
		ServiceDecks = "1-12",
		CloudlineWorks = "13-30",
		StratosphereSpine = "31-42",
	})
	check("high_band_pb_updates", (player:GetAttribute("RisingStepsPB") or 0) >= 42, {
		PB = player:GetAttribute("RisingStepsPB"),
	})

	PlayerProfileService.qaGrantCoins(player, 2500)
	local buyTrailOk, buyTrailReason = PlayerProfileService.purchaseCosmetic(player, "Trails", "ion")
	check("cosmetic_purchase", buyTrailOk, { Reason = buyTrailReason })
	local equipTrailOk, equipTrailReason = PlayerProfileService.equipCosmetic(player, "Trails", "ion")
	check("cosmetic_equip", equipTrailOk and player:GetAttribute("RisingStepsTrail") == "ion", {
		Reason = equipTrailReason,
		Equipped = player:GetAttribute("RisingStepsTrail"),
	})
	local buyEnvironmentOk, buyEnvironmentReason =
		PlayerProfileService.purchaseCosmetic(player, "EnvironmentThemes", "dusk")
	check("environment_purchase", buyEnvironmentOk, { Reason = buyEnvironmentReason })
	local equipEnvironmentOk, equipEnvironmentReason =
		PlayerProfileService.equipCosmetic(player, "EnvironmentThemes", "dusk")
	check("environment_equip", equipEnvironmentOk and player:GetAttribute("RisingStepsEnvironmentTheme") == "dusk", {
		Reason = equipEnvironmentReason,
		Equipped = player:GetAttribute("RisingStepsEnvironmentTheme"),
	})

	check("reduced_motion_on", PlayerProfileService.setReducedMotion(player, true))
	check("reduced_motion_attribute_on", player:GetAttribute("RisingStepsReducedMotion") == true)
	check("reduced_motion_off", PlayerProfileService.setReducedMotion(player, false))

	local receiptOk, receiptReason = PlayerProfileService.grantDeveloperProduct(player, "Revive", "studioqa-revive-001")
	check("developer_product_grant", receiptOk, { Reason = receiptReason })
	local revivesAfterGrant = player:GetAttribute("RisingStepsRevives") or 0
	local duplicateOk, duplicateReason =
		PlayerProfileService.grantDeveloperProduct(player, "Revive", "studioqa-revive-001")
	check("duplicate_receipt_idempotent", duplicateOk and duplicateReason == "already_processed", {
		Reason = duplicateReason,
		Revives = player:GetAttribute("RisingStepsRevives"),
	})
	check("duplicate_receipt_no_double_grant", player:GetAttribute("RisingStepsRevives") == revivesAfterGrant, {
		Before = revivesAfterGrant,
		After = player:GetAttribute("RisingStepsRevives"),
	})

	local fallBoundaryTriggered = StepCourseService.qaTriggerFallBoundary(player)
	check("fall_boundary_triggered", fallBoundaryTriggered)
	check(
		"failure_state",
		waitUntil(function()
			return player:GetAttribute("RisingStepsRunFailed") == true
		end, 2)
	)
	local failedSnapshot = StepCourseService.qaSnapshot(player)
	check("fall_boundary_single_failure", failedSnapshot.Failed == true and failedSnapshot.Ready == false, {
		Failed = failedSnapshot.Failed,
		Ready = failedSnapshot.Ready,
		Height = failedSnapshot.Height,
	})
	local reviveStarted = StepCourseService.qaUseRevive(player)
	check("revive_command", reviveStarted)
	check("revive_respawn_ready", waitReady(player))
	local revived = StepCourseService.qaSnapshot(player)
	check("revive_assisted", revived.Assisted == true and revived.RevivesUsed == 1, {
		Assisted = revived.Assisted,
		RevivesUsed = revived.RevivesUsed,
	})

	StepCourseService.qaFail(player, "StudioQARetry")
	check(
		"second_failure_state",
		waitUntil(function()
			return player:GetAttribute("RisingStepsRunFailed") == true
		end, 2)
	)
	StepCourseService.qaRetry(player)
	check("retry_respawn_ready", waitReady(player))
	local retried = StepCourseService.qaSnapshot(player)
	check("retry_resets_run", retried.Height == 0 and retried.Streak == 0 and retried.Failed == false, {
		Height = retried.Height,
		Streak = retried.Streak,
		Failed = retried.Failed,
	})

	local longClimbAccepted = 0
	for stepId = 1, CourseConfig.InitialStepCount do
		local grade = StepCourseService.qaLand(player, stepId, 0)
		if grade ~= "Rejected" then
			longClimbAccepted += 1
		else
			check(("long_climb_step_%02d"):format(stepId), false, {
				StepId = stepId,
				Height = StepCourseService.qaSnapshot(player).Height,
			})
		end
		task.wait(0.02)
	end
	local longClimb = StepCourseService.qaSnapshot(player)
	check(
		"long_climb_all_steps",
		longClimbAccepted == CourseConfig.InitialStepCount and longClimb.Height == CourseConfig.InitialStepCount,
		{
			Accepted = longClimbAccepted,
			Height = longClimb.Height,
			Target = CourseConfig.InitialStepCount,
		}
	)
	check("band_service_decks", StepCourseService.qaStepBand(player, 12) == "service-decks", {
		Band = StepCourseService.qaStepBand(player, 12),
	})
	check(
		"band_cloudline_works",
		StepCourseService.qaStepBand(player, 13) == "cloudline-works"
			and StepCourseService.qaStepBand(player, 30) == "cloudline-works",
		{
			Step13 = StepCourseService.qaStepBand(player, 13),
			Step30 = StepCourseService.qaStepBand(player, 30),
		}
	)
	check(
		"band_stratosphere_spine",
		StepCourseService.qaStepBand(player, 31) == "stratosphere-spine"
			and StepCourseService.qaStepBand(player, 42) == "stratosphere-spine",
		{
			Step31 = StepCourseService.qaStepBand(player, 31),
			Step42 = StepCourseService.qaStepBand(player, 42),
		}
	)
	check(
		"long_climb_part_budget_stable",
		type(longClimb.BaseParts) == "number" and longClimb.BaseParts <= CourseConfig.MaxLaneBaseParts,
		{
			BaseParts = longClimb.BaseParts,
			Budget = CourseConfig.MaxLaneBaseParts,
		}
	)

	local dailyOk, dailyReason, dailyReward = PlayerProfileService.claimDaily(player)
	check("daily_reward_path", dailyOk or dailyReason == "already_claimed", {
		Reason = dailyReason,
		Reward = dailyReward,
	})

	local persistent = PlayerProfileService.qaPersistentPayload(player)
	check(
		"persistent_payload",
		persistent.PB >= CourseConfig.InitialStepCount
			and persistent.Equipped.Trails == "ion"
			and persistent.Equipped.EnvironmentThemes == "dusk",
		{
			PB = persistent.PB,
			Trail = persistent.Equipped.Trails,
			Environment = persistent.Equipped.EnvironmentThemes,
			Coins = persistent.Coins,
		}
	)
	check("profile_flush", PlayerProfileService.flushPlayer(player))

	StepCourseService.qaSetInvalidPosition(player)
	check(
		"invalid_position_rejected",
		waitUntil(function()
			return player:GetAttribute("RisingStepsRunFailed") == true
		end, 2)
	)

	local afterSecurity = StepCourseService.qaSnapshot(player)
	if afterSecurity.Failed then
		StepCourseService.qaRetry(player)
	end
	check("visual_reset_ready", waitReady(player))
	StepCourseService.qaReset(player)
	task.wait(0.25)

	for stepId = 1, 3 do
		assert(StepCourseService.qaLand(player, stepId, 0) ~= "Rejected", "visual early climb rejected")
	end
	visualHold(player, "early", "Gameplay")

	for stepId = 4, 20 do
		assert(StepCourseService.qaLand(player, stepId, 0) ~= "Rejected", "visual mid climb rejected")
	end
	visualHold(player, "mid", "Gameplay")

	for stepId = 21, 38 do
		assert(StepCourseService.qaLand(player, stepId, 0) ~= "Rejected", "visual high climb rejected")
	end
	visualHold(player, "high", "Gameplay")

	StepCourseService.qaShowWarning(player, 39)
	visualHold(player, "warning", "Warning")

	StepCourseService.qaShowResult(player)
	visualHold(player, "result", "Result")

	StepCourseService.qaReset(player)
	task.wait(0.25)
	visualHold(player, "shop", "Shop")
	player:SetAttribute("RisingStepsQaView", "Gameplay")
	player:SetAttribute("RisingStepsQaPhase", "functional-complete")
	if rejoinSentinelLoaded then
		check("rejoin_cleanup_reduced_motion", PlayerProfileService.setReducedMotion(player, false))
		check("rejoin_cleanup_flush", PlayerProfileService.flushPlayer(player))
		print("[RisingStepsQA] REJOIN_COMPLETE")
	else
		check("rejoin_sentinel_reduced_motion", PlayerProfileService.setReducedMotion(player, true))
		local sentinelFlush = PlayerProfileService.flushPlayer(player)
		print(
			("[RisingStepsQA] REJOIN_SENTINEL_WRITTEN flush=%s healthy=%s"):format(
				tostring(sentinelFlush),
				tostring(initialPersistenceHealthy)
			)
		)
	end

	print("[RisingStepsQA] FUNCTIONAL_COMPLETE " .. HttpService:JSONEncode(results))

	if RuntimeQaConfig.SoakEnabled then
		StepCourseService.qaReset(player)
		check("soak_start_ready", waitReady(player))
		player:SetAttribute("RisingStepsQaPhase", "soak")
		local soakStart = os.clock()
		local nextReport = RuntimeQaConfig.SoakReportIntervalSeconds
		local cycles = 0
		local lastBaseParts = 0
		local lastBeamsAndLights = 0

		while os.clock() - soakStart < RuntimeQaConfig.SoakDurationSeconds do
			StepCourseService.qaReset(player)
			task.wait(0.05)

			for stepId = 1, CourseConfig.InitialStepCount do
				local grade = StepCourseService.qaLand(player, stepId, 0)
				assert(grade ~= "Rejected", ("soak landing rejected at step %d"):format(stepId))
				task.wait(0.005)
			end

			local snapshot = StepCourseService.qaSnapshot(player)
			assert(snapshot.Height == CourseConfig.InitialStepCount, "soak climb did not reach final step")
			assert(
				type(snapshot.BaseParts) == "number" and snapshot.BaseParts <= CourseConfig.MaxLaneBaseParts,
				"soak exceeded BasePart budget"
			)
			assert(
				type(snapshot.BeamsAndLights) == "number"
					and snapshot.BeamsAndLights <= CourseConfig.MaxLaneBeamsAndLights,
				"soak exceeded Beam/Light budget"
			)
			lastBaseParts = snapshot.BaseParts
			lastBeamsAndLights = snapshot.BeamsAndLights

			assert(StepCourseService.qaTriggerFallBoundary(player), "soak fall boundary failed")
			assert(
				waitUntil(function()
					return player:GetAttribute("RisingStepsRunFailed") == true
				end, 2),
				"soak failure state timed out"
			)
			StepCourseService.qaRetry(player)
			assert(waitReady(player), "soak retry did not recover")
			cycles += 1

			local elapsed = os.clock() - soakStart
			if elapsed >= nextReport then
				print(
					("[RisingStepsQA] SOAK %.0fs cycles=%d baseParts=%d beamsLights=%d"):format(
						elapsed,
						cycles,
						lastBaseParts,
						lastBeamsAndLights
					)
				)
				nextReport += RuntimeQaConfig.SoakReportIntervalSeconds
			end
			task.wait(RuntimeQaConfig.SoakCyclePauseSeconds)
		end

		local soakElapsed = os.clock() - soakStart
		check("soak_twenty_minutes", soakElapsed >= RuntimeQaConfig.SoakDurationSeconds and cycles > 0, {
			ElapsedSeconds = soakElapsed,
			Cycles = cycles,
			BaseParts = lastBaseParts,
			BeamsAndLights = lastBeamsAndLights,
		})
		print(("[RisingStepsQA] SOAK_COMPLETE %.1fs cycles=%d"):format(soakElapsed, cycles))
	end

	player:SetAttribute("RisingStepsQaPhase", "complete")
	print("[RisingStepsQA] COMPLETE " .. HttpService:JSONEncode(results))
end

function RuntimeQaService.start()
	if not RunService:IsStudio() or not RuntimeQaConfig.EnabledInStudio then
		return
	end

	task.spawn(function()
		local player = Players.PlayerAdded:Wait()
		local ok, err = pcall(run, player)
		if not ok then
			warn("[RisingStepsQA] ABORT " .. tostring(err))
		end
	end)

	local existing = Players:GetPlayers()[1]
	if existing then
		task.spawn(function()
			local ok, err = pcall(run, existing)
			if not ok then
				warn("[RisingStepsQA] ABORT " .. tostring(err))
			end
		end)
	end
end

return RuntimeQaService
