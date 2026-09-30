--!strict

local DataStoreService = game:GetService("DataStoreService")
local Players = game:GetService("Players")

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local root = ReplicatedStorage:WaitForChild("RisingSteps")
local EconomyConfig = require(root.config.EconomyConfig)
local RetentionConfig = require(root.config.RetentionConfig)
local ProfileRules = require(root.rules.ProfileRules)
local RetentionRules = require(root.rules.RetentionRules)

type RuntimeProfile = any

local PlayerProfileService = {}

local STORE = DataStoreService:GetDataStore(EconomyConfig.DataStoreName)
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
end

local function runtimeFromData(data: any): RuntimeProfile
	local migrated = ProfileRules.migrate(data)
	migrated.Loaded = false
	migrated.PersistenceHealthy = false
	migrated.Dirty = false
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
	profile.Dirty = true
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
end

local function loadPlayer(player: Player)
	local profile = ensureProfile(player)
	local loadedValue = nil
	local success = false

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
	profile.Dirty = true
	syncAttributes(player, profile)
	return profile.PB, true
end

function PlayerProfileService.awardLanding(player: Player, grade: string, streak: number, height: number)
	local profile = ensureProfile(player)
	ensureCurrentChallenge(profile)

	local oldPB = profile.PB
	local normalizedHeight = math.max(0, math.floor(height))
	profile.PB = math.max(profile.PB, normalizedHeight)
	local newPB = profile.PB > oldPB

	if grade == "Perfect" then
		profile.Totals.Perfect += 1
	elseif grade == "Clean" then
		profile.Totals.Clean += 1
	end

	local landingCoins = ProfileRules.rewardForLanding(grade, streak, normalizedHeight)
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

	profile.Dirty = true
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
	profile.Dirty = true
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
	profile.Dirty = true
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
	profile.Dirty = true
	syncAttributes(player, profile)
	return true, "ok"
end

function PlayerProfileService.setReducedMotion(player: Player, enabled: boolean): boolean
	local profile = ensureProfile(player)
	if not profile.Loaded then
		return false
	end

	profile.Settings.ReducedMotion = enabled
	profile.Dirty = true
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

	local desired = persistentPayload(profile)
	local success = pcall(function()
		STORE:UpdateAsync(keyFor(player), function(oldValue)
			local stored = ProfileRules.migrate(oldValue)
			desired.PB = math.max(stored.PB, desired.PB)
			return desired
		end)
	end)

	if success then
		profile.PB = desired.PB
		profile.Dirty = false
		syncAttributes(player, profile)
	end

	return success
end

local function removePlayer(player: Player)
	PlayerProfileService.flushPlayer(player)
	profiles[player] = nil
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
