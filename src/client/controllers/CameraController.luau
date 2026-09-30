--!strict

local Players = game:GetService("Players")
local Workspace = game:GetService("Workspace")

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local root = ReplicatedStorage:WaitForChild("RisingSteps")
local GameConfig = require(root.config.GameConfig)
local ControlConfig = require(root.config.ControlConfig)

local CameraController = {}

local LOCAL_PLAYER = Players.LocalPlayer
local CAMERA_OFFSET = Vector3.new(0, 3.4, 0)
local MIN_ZOOM = 10
local MAX_ZOOM = 14
local FIELD_OF_VIEW = 72

local function configureCamera()
	local camera = Workspace.CurrentCamera
	if not camera then
		return
	end

	camera.CameraType = Enum.CameraType.Custom
	camera.FieldOfView = FIELD_OF_VIEW
	LOCAL_PLAYER.CameraMinZoomDistance = MIN_ZOOM
	LOCAL_PLAYER.CameraMaxZoomDistance = MAX_ZOOM
end

local function configureCharacter(character: Model)
	local humanoid = character:WaitForChild("Humanoid", 10)
	if not humanoid or not humanoid:IsA("Humanoid") then
		return
	end

	humanoid.CameraOffset = CAMERA_OFFSET
	configureCamera()
end

function CameraController.start()
	assert(ControlConfig.AllowCameraOrbit == true, "Rising Steps V1 requires orbit-capable third-person camera")
	assert(
		GameConfig.Camera.MinimumUpcomingTargetsVisible >= 2,
		"camera product lock requires upcoming target visibility"
	)

	configureCamera()
	if LOCAL_PLAYER.Character then
		task.spawn(configureCharacter, LOCAL_PLAYER.Character)
	end

	LOCAL_PLAYER.CharacterAdded:Connect(configureCharacter)
	Workspace:GetPropertyChangedSignal("CurrentCamera"):Connect(configureCamera)
end

return CameraController
