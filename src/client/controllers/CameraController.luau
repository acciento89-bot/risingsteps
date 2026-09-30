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
local FOLLOW_RESPONSE = 7.5
local DEFAULT_FOV = 70
local COMPACT_FOV = 73
local MIN_CAMERA_DISTANCE = 6.5
local WALL_PADDING = 0.8

local renderConnection: RBXScriptConnection? = nil

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

local function targetPosition(height: number, rootPosition: Vector3): Vector3
	local course = currentCourse()
	local steps = course and course:FindFirstChild("Steps")
	if not steps then
		return rootPosition + Vector3.new(0, 4.2, 0)
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
		return rootPosition + Vector3.new(0, 4.2, 0)
	end

	local average = Vector3.zero
	for _, position in positions do
		average += position
	end
	average /= #positions

	local focus = rootPosition:Lerp(average, 0.34)
	return focus + Vector3.new(0, 2.8, 0)
end

local function horizontalDirection(from: Vector3, to: Vector3): Vector3
	local delta = Vector3.new(to.X - from.X, 0, to.Z - from.Z)
	if delta.Magnitude < 0.5 then
		return Vector3.new(0, 0, -1)
	end
	return delta.Unit
end

local function obstructionAdjustedPosition(focus: Vector3, desiredPosition: Vector3, character: Model?): Vector3
	local direction = desiredPosition - focus
	if direction.Magnitude <= MIN_CAMERA_DISTANCE then
		return desiredPosition
	end

	local params = RaycastParams.new()
	params.FilterType = Enum.RaycastFilterType.Exclude
	params.IgnoreWater = true
	if character then
		params.FilterDescendantsInstances = { character }
	end

	local hit = Workspace:Raycast(focus, direction, params)
	if not hit then
		return desiredPosition
	end

	local safeDistance = math.max(MIN_CAMERA_DISTANCE, hit.Distance - WALL_PADDING)
	return focus + direction.Unit * safeDistance
end

local function configureCharacter(character: Model)
	local humanoid = character:WaitForChild("Humanoid", 10)
	if humanoid and humanoid:IsA("Humanoid") then
		humanoid.CameraOffset = Vector3.zero
	end
end

local function startRenderLoop()
	if renderConnection then
		renderConnection:Disconnect()
	end

	renderConnection = RunService.RenderStepped:Connect(function(deltaTime)
		local camera = Workspace.CurrentCamera
		local character = LOCAL_PLAYER.Character
		local rootPart = character and character:FindFirstChild("HumanoidRootPart")
		if not camera or not rootPart or not rootPart:IsA("BasePart") then
			return
		end

		camera.CameraType = Enum.CameraType.Scriptable

		local heightAttribute = LOCAL_PLAYER:GetAttribute("RisingStepsHeight")
		local height = if type(heightAttribute) == "number" then math.max(0, math.floor(heightAttribute)) else 0
		local rootPosition = rootPart.Position
		local focus = targetPosition(height, rootPosition)

		local course = currentCourse()
		local steps = course and course:FindFirstChild("Steps")
		local nextStep = steps and steps:FindFirstChild(("Step_%03d"):format(height + 1))
		local nextCollision = nextStep and nextStep:FindFirstChild("Collision")
		local nextPosition = if nextCollision and nextCollision:IsA("BasePart")
			then nextCollision.Position
			else focus + Vector3.new(0, 0, -8)
		local forward = horizontalDirection(rootPosition, nextPosition)

		local compact = camera.ViewportSize.X <= 760
		camera.FieldOfView = if compact then COMPACT_FOV else DEFAULT_FOV
		local distance = if compact then 11.5 else 13.5
		local vertical = if compact then 7.2 else 8.4
		local desiredPosition = focus - forward * distance + Vector3.new(0, vertical, 0)
		desiredPosition = obstructionAdjustedPosition(focus, desiredPosition, character)

		local desired = CFrame.lookAt(desiredPosition, focus, Vector3.yAxis)
		local alpha = 1 - math.exp(-FOLLOW_RESPONSE * math.max(0, deltaTime))
		camera.CFrame = camera.CFrame:Lerp(desired, alpha)
	end)
end

function CameraController.start()
	assert(
		GameConfig.Camera.MinimumUpcomingTargetsVisible >= 2,
		"camera product lock requires upcoming target visibility"
	)

	if LOCAL_PLAYER.Character then
		task.spawn(configureCharacter, LOCAL_PLAYER.Character)
	end
	LOCAL_PLAYER.CharacterAdded:Connect(configureCharacter)

	startRenderLoop()
	Workspace:GetPropertyChangedSignal("CurrentCamera"):Connect(startRenderLoop)
end

return CameraController
