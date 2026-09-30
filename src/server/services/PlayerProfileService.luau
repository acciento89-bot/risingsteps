--!strict

local DataStoreService = game:GetService("DataStoreService")
local Players = game:GetService("Players")

type Profile = {
	Version: number,
	PB: number,
	Loaded: boolean,
	Dirty: boolean,
}

local PlayerProfileService = {}

local STORE = DataStoreService:GetDataStore("RisingStepsProfileV1")
local profiles: { [Player]: Profile } = {}

local function keyFor(player: Player): string
	return ("u_%d"):format(player.UserId)
end

local function sanitizeStored(value: any): number
	if type(value) ~= "table" then
		return 0
	end
	local pb = value.PB
	if type(pb) ~= "number" or pb ~= pb or pb < 0 or pb == math.huge then
		return 0
	end
	return math.floor(pb)
end

local function ensureProfile(player: Player): Profile
	local profile = profiles[player]
	if profile then
		return profile
	end

	profile = {
		Version = 1,
		PB = 0,
		Loaded = false,
		Dirty = false,
	}
	profiles[player] = profile
	player:SetAttribute("RisingStepsPB", 0)
	return profile
end

local function loadPlayer(player: Player)
	local profile = ensureProfile(player)
	local success, stored = pcall(function()
		return STORE:GetAsync(keyFor(player))
	end)

	if success then
		profile.PB = math.max(profile.PB, sanitizeStored(stored))
	end
	profile.Loaded = true
	player:SetAttribute("RisingStepsPB", profile.PB)
end

function PlayerProfileService.recordHeight(player: Player, height: number): (number, boolean)
	local profile = ensureProfile(player)
	local normalizedHeight = math.max(0, math.floor(height))
	if normalizedHeight <= profile.PB then
		return profile.PB, false
	end

	profile.PB = normalizedHeight
	profile.Dirty = true
	player:SetAttribute("RisingStepsPB", profile.PB)
	return profile.PB, true
end

function PlayerProfileService.getPB(player: Player): number
	return ensureProfile(player).PB
end

function PlayerProfileService.flushPlayer(player: Player): boolean
	local profile = profiles[player]
	if not profile or not profile.Dirty then
		return true
	end

	local desiredPB = profile.PB
	local success = pcall(function()
		STORE:UpdateAsync(keyFor(player), function(oldValue)
			local storedPB = sanitizeStored(oldValue)
			return {
				Version = 1,
				PB = math.max(storedPB, desiredPB),
			}
		end)
	end)

	if success and profile.PB <= desiredPB then
		profile.Dirty = false
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

	game:BindToClose(function()
		for player in profiles do
			PlayerProfileService.flushPlayer(player)
		end
	end)
end

return PlayerProfileService
