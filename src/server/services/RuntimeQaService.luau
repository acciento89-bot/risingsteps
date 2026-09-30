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

local function run(player: Player)
	task.wait(RuntimeQaConfig.StartupDelaySeconds)
	PlayerProfileService.qaSetPersistenceHealthy(player, true)
	check("profile_loaded", player:GetAttribute("RisingStepsProfileLoaded") == true)

	check("course_ready", waitReady(player))
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

	StepCourseService.qaFail(player, "StudioQAFailure")
	check(
		"failure_state",
		waitUntil(function()
			return player:GetAttribute("RisingStepsRunFailed") == true
		end, 2)
	)
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

	local dailyOk, dailyReason, dailyReward = PlayerProfileService.claimDaily(player)
	check("daily_reward_path", dailyOk or dailyReason == "already_claimed", {
		Reason = dailyReason,
		Reward = dailyReward,
	})

	local persistent = PlayerProfileService.qaPersistentPayload(player)
	check(
		"persistent_payload",
		persistent.PB >= 12 and persistent.Equipped.Trails == "ion" and persistent.Equipped.EnvironmentThemes == "dusk",
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
