--!strict

local PlayerProfileService = require(script.Parent.PlayerProfileService)

local ProgressionService = {}

local MAX_TEXT_LENGTH = 32
local lastCommandAt: { [Player]: number } = {}

local function validText(value: any): boolean
	return type(value) == "string" and #value >= 1 and #value <= MAX_TEXT_LENGTH
end

local function emitProfile(remoteFolder: Instance, player: Player, result: any?)
	local stateSnapshot = remoteFolder:FindFirstChild("StateSnapshot")
	if stateSnapshot and stateSnapshot:IsA("RemoteEvent") then
		stateSnapshot:FireClient(player, {
			Kind = "Profile",
			Profile = PlayerProfileService.getSnapshot(player),
			Result = result,
		})
	end
end

function ProgressionService.start(remoteFolder: Folder)
	local requestBootstrap = remoteFolder:WaitForChild("RequestBootstrap")
	local shopCommand = remoteFolder:WaitForChild("ShopCommand")
	assert(requestBootstrap:IsA("RemoteFunction"), "RequestBootstrap must be a RemoteFunction")
	assert(shopCommand:IsA("RemoteEvent"), "ShopCommand must be a RemoteEvent")

	requestBootstrap.OnServerInvoke = function(player)
		return PlayerProfileService.getSnapshot(player)
	end

	shopCommand.OnServerEvent:Connect(function(player, payload)
		if type(payload) ~= "table" then
			return
		end

		local now = os.clock()
		local previous = lastCommandAt[player] or 0
		if now - previous < 0.15 then
			return
		end
		lastCommandAt[player] = now

		local action = payload.Action
		if not validText(action) then
			return
		end

		if action == "Purchase" then
			if not validText(payload.Category) or not validText(payload.ItemId) then
				return
			end
			local ok, reason = PlayerProfileService.purchaseCosmetic(player, payload.Category, payload.ItemId)
			emitProfile(remoteFolder, player, { Action = action, Ok = ok, Reason = reason })
		elseif action == "Equip" then
			if not validText(payload.Category) or not validText(payload.ItemId) then
				return
			end
			local ok, reason = PlayerProfileService.equipCosmetic(player, payload.Category, payload.ItemId)
			emitProfile(remoteFolder, player, { Action = action, Ok = ok, Reason = reason })
		elseif action == "ReducedMotion" and type(payload.Enabled) == "boolean" then
			local ok = PlayerProfileService.setReducedMotion(player, payload.Enabled)
			emitProfile(remoteFolder, player, {
				Action = action,
				Ok = ok,
				Reason = if ok then "ok" else "profile_unavailable",
			})
		end
	end)
end

return ProgressionService
