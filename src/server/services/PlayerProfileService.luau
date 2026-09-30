--!strict

local DataStoreService = game:GetService("DataStoreService")
local Players = game:GetService("Players")
local RunService = game:GetService("RunService")

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local root = ReplicatedStorage:WaitForChild("RisingSteps")
local EconomyConfig = require(root.config.EconomyConfig)
local MonetizationConfig = require(root.config.MonetizationConfig)
local RetentionConfig = require(root.config.RetentionConfig)
local PersistenceRules = require(root.rules.PersistenceRules)
local ProfileRules = require(root.rules.ProfileRules)
local ReceiptRules = require(root.rules.ReceiptRules)
local RetentionRules = require(root.rules.RetentionRules)

type RuntimeProfile = any

local DiagnosticsService = require(script.Parent.DiagnosticsService)

local PlayerProfileService = {}

local STORE: any = nil
local storeAvailable = false
local storeOk, storeResult = pcall(function()
	return DataStoreService:GetDataStore(EconomyConfig.DataStoreName)
end)
if storeOk then
	STORE = storeResult
	storeAvailable = true
elseif not RunService:IsStudio() then
	DiagnosticsService.warn("DataStoreUnavailable", {})
end

local profiles: { [Player]: RuntimeProfile } = {}

local function keyFor(player: Player): string
	return ("u_%d"):format(player.UserId)
end

local function syncAttributes(player: Player, profile: RuntimeProfile)
	player:SetAttribute("RisingStepsPB", profile.PB)
	player:SetAttribute("RisingStepsCoins", profile.Coins)
	player:SetAttribute("RisingStepsTrail", profile.Equipped.Trails)
	player:SetAttribute("RisingStepsLandingEffect", profile.Equipped.LandingEffects)
	player:SetAttribute("RisingStepsStepTheme", profile.Equipped.StepThemes)
	player:SetAttribute("RisingStepsEnvironmentTheme", profile.Equipped.EnvironmentThemes)
	player:SetAttribute("RisingStepsReducedMotion", profile.Settings.ReducedMotion)
	player:SetAttribute("RisingStepsTutorialComplete", profile.TutorialComplete)
	player:SetAttribute("RisingStepsProfileLoaded", profile.Loaded)
	player:SetAttribute("RisingStepsPersistenceHealthy", profile.PersistenceHealthy)
	player:SetAttribute("RisingStepsRevives", profile.Inventory.Revives)
	player:SetAttribute("RisingStepsStabilityUntil", profile.Boosts.StabilityUntil)
end

local function runtimeFromData(data: any): RuntimeProfile
	local migrated = ProfileRules.migrate(data)
	migrated.Loaded = false
	migrated.PersistenceHealthy = false
	migrated.Dirty = false
	migrated.Revision = 0
	return migrated
end

local function ensureProfile(player: Player): RuntimeProfile
	local existing = profiles[player]
	if existing then
		return existing
	end

	local profile = runtimeFromData(nil)
	profiles[player] = profile
	syncAttributes(player, profile)
	return profile
end

local function markDirty(profile: RuntimeProfile)
	profile.Dirty = true
	profile.Revision += 1
end

local function persistentPayload(profile: RuntimeProfile)
	return {
		Version = EconomyConfig.ProfileVersion,
		PB = profile.PB,
		Coins = profile.Coins,
		Owned = profile.Owned,
		Equipped = profile.Equipped,
		Settings = profile.Settings,
		TutorialComplete = profile.TutorialComplete,
		Daily = profile.Daily,
		Challenge = profile.Challenge,
		Achievements = profile.Achievements,
		Totals = profile.Totals,
		Inventory = profile.Inventory,
		Boosts = profile.Boosts,
		ProcessedReceipts = profile.ProcessedReceipts,
	}
end

local function currentDay(): number
	return RetentionRules.dayIndex(os.time())
end

local function ensureCurrentChallenge(profile: RuntimeProfile): boolean
	local day = currentDay()
	if profile.Challenge.Day == day then
		return false
	end
	profile.Challenge.Day = day
	profile.Challenge.Progress = 0
	profile.Challenge.Claimed = false
	markDirty(profile)
	return true
end

local function mergeSessionProgress(loaded: RuntimeProfile, current: RuntimeProfile)
	loaded.PB = math.max(loaded.PB, current.PB)
	loaded.Coins += current.Coins
	loaded.TutorialComplete = loaded.TutorialComplete or current.TutorialComplete
	loaded.Totals.Perfect += current.Totals.Perfect
	loaded.Totals.Clean += current.Totals.Clean

	for achievementId, unlocked in current.Achievements do
		if unlocked then
			loaded.Achievements[achievementId] = true
		end
	end

	if current.Challenge.Day == loaded.Challenge.Day then
		loaded.Challenge.Progress = math.max(loaded.Challenge.Progress, current.Challenge.Progress)
		loaded.Challenge.Claimed = loaded.Challenge.Claimed or current.Challenge.Claimed
	end

	loaded.Dirty = current.Dirty or current.Coins > 0 or current.PB > 0
	loaded.Revision = current.Revision
end

local function loadPlayer(player: Player)
	local profile = ensureProfile(player)
	local loadedValue = nil
	local success = false

	if not storeAvailable and RunService:IsStudio() then
		success = true
		loadedValue = nil
	else
		for attempt = 1, EconomyConfig.LoadAttempts do
			local ok, result = pcall(function()
				return STORE:GetAsync(keyFor(player))
			end)
			if ok then
				success = true
				loadedValue = result
				break
			end
			if attempt < EconomyConfig.LoadAttempts then
				task.wait(EconomyConfig.LoadRetrySeconds * attempt)
			end
		end
	end

	if success then
		local loaded = runtimeFromData(loadedValue)
		loaded.Loaded = true
		loaded.PersistenceHealthy = true
		mergeSessionProgress(loaded, profile)
		profiles[player] = loaded
		ensureCurrentChallenge(loaded)
		syncAttributes(player, loaded)
	else
		profile.Loaded = true
		profile.PersistenceHealthy = false
		syncAttributes(player, profile)
		DiagnosticsService.warn("ProfileLoadFailed", { UserId = player.UserId })
	end
end

local function challengeSnapshot(profile: RuntimeProfile)
	local day = currentDay()
	local challenge = RetentionRules.challengeForDay(day)
	local progress = if profile.Challenge.Day == day then profile.Challenge.Progress else 0
	local claimed = profile.Challenge.Day == day and profile.Challenge.Claimed

	return {
		Kind = challenge.Kind,
		Target = challenge.Target,
		Progress = progress,
		Claimed = claimed,
		RewardCoins = challenge.RewardCoins,
	}
end

function PlayerProfileService.getSnapshot(player: Player)
	local profile = ensureProfile(player)
	if profile.Loaded then
		ensureCurrentChallenge(profile)
	end

	return {
		Version = profile.Version,
		PB = profile.PB,
		Coins = profile.Coins,
		Owned = profile.Owned,
		Equipped = profile.Equipped,
		Settings = profile.Settings,
		TutorialComplete = profile.TutorialComplete,
		Daily = {
			Claimable = profile.Daily.LastClaimDay ~= currentDay(),
			ClaimStreak = profile.Daily.ClaimStreak,
		},
		Challenge = challengeSnapshot(profile),
		Achievements = profile.Achievements,
		Inventory = profile.Inventory,
		Boosts = profile.Boosts,
		Loaded = profile.Loaded,
		PersistenceHealthy = profile.PersistenceHealthy,
	}
end

function PlayerProfileService.recordHeight(player: Player, height: number): (number, boolean)
	local profile = ensureProfile(player)
	local normalizedHeight = math.max(0, math.floor(height))
	if normalizedHeight <= profile.PB then
		return profile.PB, false
	end

	profile.PB = normalizedHeight
	markDirty(profile)
	syncAttributes(player, profile)
	return profile.PB, true
end

function PlayerProfileService.awardLanding(
	player: Player,
	grade: string,
	streak: number,
	height: number,
	competitive: boolean?
)
	local profile = ensureProfile(player)
	ensureCurrentChallenge(profile)

	local oldPB = profile.PB
	local normalizedHeight = math.max(0, math.floor(height))
	if competitive ~= false then
		profile.PB = math.max(profile.PB, normalizedHeight)
	end
	local newPB = profile.PB > oldPB

	if grade == "Perfect" then
		profile.Totals.Perfect += 1
	elseif grade == "Clean" then
		profile.Totals.Clean += 1
	end

	local landingCoins = ProfileRules.rewardForLanding(grade, streak, normalizedHeight)
	local multiplier = player:GetAttribute("RisingStepsCoinMultiplier")
	if type(multiplier) == "number" then
		landingCoins *= math.max(1, math.floor(multiplier))
	end
	profile.Coins += landingCoins

	local event = {
		Grade = grade,
		Height = normalizedHeight,
		Streak = math.max(0, math.floor(streak)),
		PerfectTotal = profile.Totals.Perfect,
	}

	local challengeCoins = 0
	local challenge = RetentionRules.challengeForDay(profile.Challenge.Day)
	profile.Challenge.Progress = RetentionRules.challengeProgress(challenge, profile.Challenge.Progress, event)
	if profile.Challenge.Progress >= challenge.Target and not profile.Challenge.Claimed then
		profile.Challenge.Claimed = true
		challengeCoins = challenge.RewardCoins
		profile.Coins += challengeCoins
	end

	local unlockedAchievements = {}
	local achievementCoins = 0
	for _, achievementId in RetentionRules.achievementIdsForEvent(event) do
		if profile.Achievements[achievementId] ~= true then
			profile.Achievements[achievementId] = true
			local definition = RetentionConfig.Achievements[achievementId]
			achievementCoins += definition.RewardCoins
			table.insert(unlockedAchievements, achievementId)
		end
	end
	profile.Coins += achievementCoins

	if not profile.TutorialComplete then
		profile.TutorialComplete = true
	end

	markDirty(profile)
	syncAttributes(player, profile)

	return {
		LandingCoins = landingCoins,
		ChallengeCoins = challengeCoins,
		AchievementCoins = achievementCoins,
		NewPB = newPB,
		UnlockedAchievements = unlockedAchievements,
	}
end

function PlayerProfileService.claimDaily(player: Player): (boolean, string, number)
	local profile = ensureProfile(player)
	if not profile.Loaded or not profile.PersistenceHealthy then
		return false, "profile_unavailable", 0
	end

	local day = currentDay()
	local ok, reason, nextStreak, reward =
		RetentionRules.dailyClaim(profile.Daily.LastClaimDay, profile.Daily.ClaimStreak, day)
	if not ok then
		return false, reason, 0
	end

	profile.Daily.LastClaimDay = day
	profile.Daily.ClaimStreak = nextStreak
	profile.Coins += reward
	markDirty(profile)
	syncAttributes(player, profile)
	return true, "ok", reward
end

function PlayerProfileService.getPB(player: Player): number
	return ensureProfile(player).PB
end

function PlayerProfileService.purchaseCosmetic(player: Player, category: string, itemId: string): (boolean, string)
	local profile = ensureProfile(player)
	if not profile.Loaded or not profile.PersistenceHealthy then
		return false, "profile_unavailable"
	end

	local ok, reason, price = ProfileRules.validatePurchase(profile, category, itemId)
	if not ok then
		return false, reason
	end

	profile.Coins -= price
	profile.Owned[category][itemId] = true
	markDirty(profile)
	syncAttributes(player, profile)
	return true, "ok"
end

function PlayerProfileService.equipCosmetic(player: Player, category: string, itemId: string): (boolean, string)
	local profile = ensureProfile(player)
	if not profile.Loaded then
		return false, "profile_unavailable"
	end

	local ok, reason = ProfileRules.validateEquip(profile, category, itemId)
	if not ok then
		return false, reason
	end

	profile.Equipped[category] = itemId
	markDirty(profile)
	syncAttributes(player, profile)
	return true, "ok"
end

function PlayerProfileService.setReducedMotion(player: Player, enabled: boolean): boolean
	local profile = ensureProfile(player)
	if not profile.Loaded then
		return false
	end

	profile.Settings.ReducedMotion = enabled
	markDirty(profile)
	syncAttributes(player, profile)
	return true
end

function PlayerProfileService.grantDeveloperProduct(
	player: Player,
	productKey: string,
	purchaseId: string
): (boolean, string)
	local profile = ensureProfile(player)
	if not profile.Loaded or not profile.PersistenceHealthy then
		return false, "profile_unavailable"
	end
	if ReceiptRules.contains(profile.ProcessedReceipts, purchaseId) then
		return true, "already_processed"
	end

	local product = MonetizationConfig.DeveloperProducts[productKey]
	if not product then
		return false, "unknown_product"
	end

	if product.Grant == "Revive" then
		profile.Inventory.Revives = math.min(product.MaxInventory, profile.Inventory.Revives + 1)
	elseif product.Grant == "Stability" then
		local startAt = math.max(os.time(), profile.Boosts.StabilityUntil)
		profile.Boosts.StabilityUntil = startAt + product.DurationSeconds
	else
		return false, "unknown_grant"
	end

	profile.ProcessedReceipts = ReceiptRules.append(profile.ProcessedReceipts, purchaseId)
	markDirty(profile)
	syncAttributes(player, profile)
	return true, "ok"
end

function PlayerProfileService.consumeRevive(player: Player): boolean
	local profile = ensureProfile(player)
	if profile.Inventory.Revives <= 0 then
		return false
	end

	profile.Inventory.Revives -= 1
	markDirty(profile)
	syncAttributes(player, profile)
	return true
end

function PlayerProfileService.flushPlayer(player: Player): boolean
	local profile = profiles[player]
	if not profile or not profile.Dirty then
		return true
	end
	if not profile.PersistenceHealthy then
		return false
	end

	if not storeAvailable and RunService:IsStudio() then
		profile.Dirty = false
		syncAttributes(player, profile)
		return true
	end
	if not storeAvailable then
		return false
	end

	local desired = persistentPayload(profile)
	local revisionAtStart = profile.Revision
	local success = pcall(function()
		STORE:UpdateAsync(keyFor(player), function(oldValue)
			local stored = ProfileRules.migrate(oldValue)
			desired.PB = math.max(stored.PB, desired.PB)
			return desired
		end)
	end)

	if success then
		profile.PB = desired.PB
		if PersistenceRules.shouldClearDirty(true, revisionAtStart, profile.Revision) then
			profile.Dirty = false
		end
		syncAttributes(player, profile)
	else
		DiagnosticsService.warn("ProfileSaveFailed", {
			UserId = player.UserId,
			Revision = revisionAtStart,
		})
	end

	return success
end

local function removePlayer(player: Player)
	PlayerProfileService.flushPlayer(player)
	profiles[player] = nil
end


function PlayerProfileService.qaGrantCoins(player: Player, amount: number)
	assert(RunService:IsStudio(), "QA helpers are Studio-only")
	local profile = ensureProfile(player)
	assert(profile.Loaded, "QA profile not loaded")
	profile.Coins += math.max(0, math.floor(amount))
	markDirty(profile)
	syncAttributes(player, profile)
end

function PlayerProfileService.qaPersistentPayload(player: Player)
	assert(RunService:IsStudio(), "QA helpers are Studio-only")
	local profile = ensureProfile(player)
	return persistentPayload(profile)
end

function PlayerProfileService.qaSetPersistenceHealthy(player: Player, healthy: boolean)
	assert(RunService:IsStudio(), "QA helpers are Studio-only")
	local profile = ensureProfile(player)
	profile.Loaded = true
	profile.PersistenceHealthy = healthy
	syncAttributes(player, profile)
end

function PlayerProfileService.start()
	for _, player in Players:GetPlayers() do
		ensureProfile(player)
		task.spawn(loadPlayer, player)
	end

	Players.PlayerAdded:Connect(function(player)
		ensureProfile(player)
		task.spawn(loadPlayer, player)
	end)
	Players.PlayerRemoving:Connect(removePlayer)

	task.spawn(function()
		while true do
			task.wait(EconomyConfig.AutosaveSeconds)
			for player in profiles do
				task.spawn(PlayerProfileService.flushPlayer, player)
			end
		end
	end)

	game:BindToClose(function()
		for player in profiles do
			PlayerProfileService.flushPlayer(player)
		end
	end)
end

return PlayerProfileService
