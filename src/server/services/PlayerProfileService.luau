--!strict

local DataStoreService = game:GetService("DataStoreService")
local Players = game:GetService("Players")

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local root = ReplicatedStorage:WaitForChild("RisingSteps")
local EconomyConfig = require(root.config.EconomyConfig)
local ProfileRules = require(root.rules.ProfileRules)

type RuntimeProfile = {
	Version: number,
	PB: number,
	Coins: number,
	Owned: { [string]: { [string]: boolean } },
	Equipped: { [string]: string },
	Settings: { ReducedMotion: boolean },
	Loaded: boolean,
	PersistenceHealthy: boolean,
	Dirty: boolean,
}

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
	player:SetAttribute("RisingStepsProfileLoaded", profile.Loaded)
	player:SetAttribute("RisingStepsPersistenceHealthy", profile.PersistenceHealthy)
end

local function runtimeFromData(data: any): RuntimeProfile
	local migrated = ProfileRules.migrate(data)
	return {
		Version = migrated.Version,
		PB = migrated.PB,
		Coins = migrated.Coins,
		Owned = migrated.Owned,
		Equipped = migrated.Equipped,
		Settings = migrated.Settings,
		Loaded = false,
		PersistenceHealthy = false,
		Dirty = false,
	}
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
	}
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

		local current = profiles[player]
		if current then
			loaded.PB = math.max(loaded.PB, current.PB)
			loaded.Coins += current.Coins
			loaded.Dirty = current.Dirty or loaded.Coins ~= ProfileRules.migrate(loadedValue).Coins
		end
		profiles[player] = loaded
		syncAttributes(player, loaded)
	else
		profile.Loaded = true
		profile.PersistenceHealthy = false
		syncAttributes(player, profile)
	end
end

function PlayerProfileService.getSnapshot(player: Player)
	local profile = ensureProfile(player)
	return {
		Version = profile.Version,
		PB = profile.PB,
		Coins = profile.Coins,
		Owned = profile.Owned,
		Equipped = profile.Equipped,
		Settings = profile.Settings,
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

function PlayerProfileService.awardLanding(player: Player, grade: string, streak: number, height: number): number
	local profile = ensureProfile(player)
	local reward = ProfileRules.rewardForLanding(grade, streak, height)
	if reward <= 0 then
		return 0
	end

	profile.Coins += reward
	profile.Dirty = true
	PlayerProfileService.recordHeight(player, height)
	syncAttributes(player, profile)
	return reward
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
