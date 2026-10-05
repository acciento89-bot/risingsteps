--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local GameConfig = require(root.config.GameConfig)

local CameraController = {}

local LOCAL_PLAYER = Players.LocalPlayer
local DEFAULT_FOV = 66
local COMPACT_FOV = 70

local viewportConnection: RBXScriptConnection? = nil

local function keepAvatarVisible(character: Model)
	for _, descendant in character:GetDescendants() do
		if descendant:IsA("BasePart") and descendant.LocalTransparencyModifier ~= 0 then
			descendant.LocalTransparencyModifier = 0
		end
	end
end

local function applyViewport(camera: Camera)
	camera.FieldOfView = if camera.ViewportSize.X <= 760 then COMPACT_FOV else DEFAULT_FOV
end

local function bindViewport(camera: Camera)
	if viewportConnection then
		viewportConnection:Disconnect()
		viewportConnection = nil
	end
	applyViewport(camera)
	viewportConnection = camera:GetPropertyChangedSignal("ViewportSize"):Connect(function()
		applyViewport(camera)
	end)
end

local function attachPlayerCamera(character: Model?)
	local camera = Workspace.CurrentCamera
	if not camera then
		return
	end

	camera.CameraType = Enum.CameraType.Custom
	applyViewport(camera)

	if not character then
		return
	end

	local humanoid = character:FindFirstChildOfClass("Humanoid")
	if humanoid then
		humanoid.CameraOffset = Vector3.zero
		camera.CameraSubject = humanoid
	end
	keepAvatarVisible(character)
end

local function configureCharacter(character: Model)
	local humanoid = character:WaitForChild("Humanoid", 10)
	if not humanoid or not humanoid:IsA("Humanoid") then
		return
	end

	humanoid.CameraOffset = Vector3.zero
	attachPlayerCamera(character)
end

function CameraController.start()
	assert(
		GameConfig.Camera.MinimumUpcomingTargetsVisible >= 2,
		"camera product lock requires upcoming target visibility"
	)

	local camera = Workspace.CurrentCamera
	if camera then
		bindViewport(camera)
	end
	attachPlayerCamera(LOCAL_PLAYER.Character)

	if LOCAL_PLAYER.Character then
		task.defer(configureCharacter, LOCAL_PLAYER.Character)
	end

	LOCAL_PLAYER.CharacterAdded:Connect(function(character)
		task.defer(configureCharacter, character)
	end)

	LOCAL_PLAYER.CharacterRemoving:Connect(function()
		attachPlayerCamera(nil)
	end)

	Workspace:GetPropertyChangedSignal("CurrentCamera"):Connect(function()
		local current = Workspace.CurrentCamera
		if current then
			bindViewport(current)
		end
		attachPlayerCamera(LOCAL_PLAYER.Character)
	end)
end

return CameraController
