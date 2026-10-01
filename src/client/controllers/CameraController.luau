--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local GameConfig = require(root.config.GameConfig)
local WorldConfig = require(root.config.WorldConfig)

local CameraController = {}

local LOCAL_PLAYER = Players.LocalPlayer
local FOLLOW_RESPONSE = 8.5
local DEFAULT_FOV = 66
local COMPACT_FOV = 70
local WALL_PADDING = 0.8
local MIN_CAMERA_DISTANCE = 5.5

local renderConnection: RBXScriptConnection? = nil
local snappedCharacter: Model? = nil
local lastHeight: number? = nil

local function currentCourse(): Model?
	local world = Workspace:FindFirstChild(WorldConfig.WorldFolderName)
	if not world then
		return nil
	end

	local course = world:FindFirstChild(("Course_%d"):format(LOCAL_PLAYER.UserId))
	if course and course:IsA("Model") then
		return course
	end
	return nil
end

local function horizontalUnit(vector: Vector3, fallback: Vector3): Vector3
	local flat = Vector3.new(vector.X, 0, vector.Z)
	if flat.Magnitude >= 0.25 then
		return flat.Unit
	end

	local fallbackFlat = Vector3.new(fallback.X, 0, fallback.Z)
	if fallbackFlat.Magnitude >= 0.25 then
		return fallbackFlat.Unit
	end

	return Vector3.new(0, 0, -1)
end

local function targetPosition(height: number, rootPart: BasePart, compact: boolean): Vector3
	local course = currentCourse()
	local steps = course and course:FindFirstChild("Steps")
	if not steps then
		return rootPart.Position + Vector3.new(0, if compact then 2.0 else 2.35, 0)
	end

	local positions = {}
	for offset = 1, 3 do
		local step = steps:FindFirstChild(("Step_%03d"):format(height + offset))
		local collision = step and step:FindFirstChild("Collision")
		if collision and collision:IsA("BasePart") then
			table.insert(positions, collision.Position)
		end
	end

	if #positions == 0 then
		return rootPart.Position + Vector3.new(0, 2.35, 0)
	end

	local average = Vector3.zero
	for _, position in positions do
		average += position
	end
	average /= #positions

	local futureWeight = if compact then 0.20 else 0.28
	local verticalLead = if compact then 2.05 else 2.45
	local focus = rootPart.Position:Lerp(average, futureWeight)
	return focus + Vector3.new(0, verticalLead, 0)
end

local function forwardDirection(height: number, rootPart: BasePart): Vector3
	local course = currentCourse()
	local steps = course and course:FindFirstChild("Steps")
	local nextStep = steps and steps:FindFirstChild(("Step_%03d"):format(height + 1))
	local collision = nextStep and nextStep:FindFirstChild("Collision")

	if collision and collision:IsA("BasePart") then
		return horizontalUnit(collision.Position - rootPart.Position, rootPart.CFrame.LookVector)
	end

	return horizontalUnit(rootPart.CFrame.LookVector, Vector3.new(0, 0, -1))
end

local function obstructionAdjustedPosition(focus: Vector3, desiredPosition: Vector3, character: Model): Vector3
	local direction = desiredPosition - focus
	if direction.Magnitude <= MIN_CAMERA_DISTANCE then
		return desiredPosition
	end

	local params = RaycastParams.new()
	params.FilterType = Enum.RaycastFilterType.Exclude
	params.IgnoreWater = true
	params.FilterDescendantsInstances = { character }

	local hit = Workspace:Raycast(focus, direction, params)
	if not hit then
		return desiredPosition
	end

	local safeDistance = math.max(MIN_CAMERA_DISTANCE, hit.Distance - WALL_PADDING)
	return focus + direction.Unit * safeDistance
end

local function desiredCameraCFrame(camera: Camera, character: Model, rootPart: BasePart, height: number): CFrame
	local compact = camera.ViewportSize.X <= 760
	camera.FieldOfView = if compact then COMPACT_FOV else DEFAULT_FOV

	local focus = targetPosition(height, rootPart, compact)
	local forward = forwardDirection(height, rootPart)
	local distance = if compact then 11.2 else 11.6
	local vertical = if compact then 6.4 else 7.2
	local sideOffset = if compact then 0.35 else 0.55

	local desiredPosition = focus
		- forward * distance
		+ Vector3.new(0, vertical, 0)
		+ rootPart.CFrame.RightVector * sideOffset
	desiredPosition = obstructionAdjustedPosition(focus, desiredPosition, character)

	return CFrame.lookAt(desiredPosition, focus, Vector3.yAxis)
end

local function fallbackToCharacterCamera(character: Model?)
	local camera = Workspace.CurrentCamera
	if not camera then
		return
	end

	local humanoid = character and character:FindFirstChildOfClass("Humanoid")
	camera.CameraType = Enum.CameraType.Custom
	if humanoid then
		camera.CameraSubject = humanoid
	end
end

local function snapToCharacter(character: Model)
	local camera = Workspace.CurrentCamera
	local rootPart = character:FindFirstChild("HumanoidRootPart")
	if not camera or not rootPart or not rootPart:IsA("BasePart") then
		return
	end

	local heightAttribute = LOCAL_PLAYER:GetAttribute("RisingStepsHeight")
	local height = if type(heightAttribute) == "number" then math.max(0, math.floor(heightAttribute)) else 0
	camera.CameraType = Enum.CameraType.Scriptable
	camera.CFrame = desiredCameraCFrame(camera, character, rootPart, height)
	snappedCharacter = character
	lastHeight = height
end

local function configureCharacter(character: Model)
	snappedCharacter = nil
	lastHeight = nil
	fallbackToCharacterCamera(character)

	local humanoid = character:WaitForChild("Humanoid", 10)
	local rootPart = character:WaitForChild("HumanoidRootPart", 10)
	if not humanoid or not humanoid:IsA("Humanoid") or not rootPart or not rootPart:IsA("BasePart") then
		return
	end

	humanoid.CameraOffset = Vector3.zero
	local camera = Workspace.CurrentCamera
	if camera then
		camera.CameraSubject = humanoid
	end

	-- First frame must already be attached to the avatar. Never lerp from the Studio/map camera.
	task.defer(function()
		if character.Parent and LOCAL_PLAYER.Character == character then
			snapToCharacter(character)
		end
	end)
end

local function startRenderLoop()
	if renderConnection then
		renderConnection:Disconnect()
	end

	renderConnection = RunService.RenderStepped:Connect(function(deltaTime)
		local camera = Workspace.CurrentCamera
		local character = LOCAL_PLAYER.Character
		local rootPart = character and character:FindFirstChild("HumanoidRootPart")
		if not camera or not character or not rootPart or not rootPart:IsA("BasePart") then
			fallbackToCharacterCamera(character)
			return
		end

		local heightAttribute = LOCAL_PLAYER:GetAttribute("RisingStepsHeight")
		local height = if type(heightAttribute) == "number" then math.max(0, math.floor(heightAttribute)) else 0
		local desired = desiredCameraCFrame(camera, character, rootPart, height)

		camera.CameraType = Enum.CameraType.Scriptable
		local heightJump = if lastHeight == nil then math.huge else math.abs(height - lastHeight)
		if snappedCharacter ~= character or heightJump >= 3 then
			-- Startup/respawn and large authoritative height corrections must never expose stale framing.
			-- Normal gameplay advances one step at a time and therefore retains smooth follow motion.
			camera.CFrame = desired
			snappedCharacter = character
			lastHeight = height
			return
		end

		lastHeight = height
		local alpha = 1 - math.exp(-FOLLOW_RESPONSE * math.max(0, deltaTime))
		camera.CFrame = camera.CFrame:Lerp(desired, alpha)
	end)
end

function CameraController.start()
	assert(
		GameConfig.Camera.MinimumUpcomingTargetsVisible >= 2,
		"camera product lock requires upcoming target visibility"
	)

	fallbackToCharacterCamera(LOCAL_PLAYER.Character)

	if LOCAL_PLAYER.Character then
		task.spawn(configureCharacter, LOCAL_PLAYER.Character)
	end

	LOCAL_PLAYER.CharacterAdded:Connect(configureCharacter)
	LOCAL_PLAYER.CharacterRemoving:Connect(function(character)
		if snappedCharacter == character then
			snappedCharacter = nil
			lastHeight = nil
		end
		fallbackToCharacterCamera(nil)
	end)

	startRenderLoop()
	Workspace:GetPropertyChangedSignal("CurrentCamera"):Connect(function()
		snappedCharacter = nil
		lastHeight = nil
		fallbackToCharacterCamera(LOCAL_PLAYER.Character)
		if LOCAL_PLAYER.Character then
			task.defer(snapToCharacter, LOCAL_PLAYER.Character)
		end
	end)
end

return CameraController
