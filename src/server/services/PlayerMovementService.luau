--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local GameConfig = require(root.config.GameConfig)

local PlayerMovementService = {}

local function configureCharacter(character: Model)
	local humanoid = character:WaitForChild("Humanoid", 10)
	if not humanoid or not humanoid:IsA("Humanoid") then
		return
	end

	Workspace.Gravity = GameConfig.Movement.Gravity
	humanoid.UseJumpPower = GameConfig.Movement.UseJumpPower
	humanoid.WalkSpeed = GameConfig.Movement.WalkSpeed
	humanoid.JumpPower = GameConfig.Movement.JumpPower
	humanoid.AutoRotate = true
	humanoid:SetAttribute("RisingStepsMovementProfile", "v1")
end

local function bindPlayer(player: Player)
	if player.Character then
		task.spawn(configureCharacter, player.Character)
	end
	player.CharacterAdded:Connect(configureCharacter)
end

function PlayerMovementService.start()
	for _, player in Players:GetPlayers() do
		bindPlayer(player)
	end
	Players.PlayerAdded:Connect(bindPlayer)
end

return PlayerMovementService
