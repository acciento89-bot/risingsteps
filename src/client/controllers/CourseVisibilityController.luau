--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local TweenService = game:GetService("TweenService")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local CourseConfig = require(root.config.CourseConfig)
local WorldConfig = require(root.config.WorldConfig)

local CourseVisibilityController = {}

local LOCAL_PLAYER = Players.LocalPlayer
local REVEAL_TWEEN = TweenInfo.new(0.24, Enum.EasingStyle.Quint, Enum.EasingDirection.Out)
local connectedSteps: Folder? = nil
local childConnection: RBXScriptConnection? = nil
local refreshToken = 0

local function currentHeight(): number
	local value = LOCAL_PLAYER:GetAttribute("RisingStepsHeight")
	return if type(value) == "number" then math.max(0, math.floor(value)) else 0
end

local function stepId(model: Model): number?
	local value = string.match(model.Name, "^Step_(%d+)$")
	return if value then tonumber(value) else nil
end

local function setModelVisibility(model: Model, visible: boolean, immediate: boolean)
	local target = if visible then 0 else 1
	for _, descendant in model:GetDescendants() do
		if descendant:IsA("BasePart") then
			local previous = descendant:GetAttribute("RisingStepsLocalVisibility")
			if previous == target then
				continue
			end
			descendant:SetAttribute("RisingStepsLocalVisibility", target)

			if immediate then
				descendant.LocalTransparencyModifier = target
			else
				TweenService:Create(descendant, REVEAL_TWEEN, {
					LocalTransparencyModifier = target,
				}):Play()
			end
		end
	end
end

local function applyVisibility(immediate: boolean)
	local steps = connectedSteps
	if not steps or not steps.Parent then
		return
	end

	local height = currentHeight()
	local maximumVisibleId = height + CourseConfig.ClientVisibleStepsAhead
	for _, child in steps:GetChildren() do
		if child:IsA("Model") then
			local id = stepId(child)
			if id then
				setModelVisibility(child, id <= maximumVisibleId, immediate)
			end
		end
	end
end

local function connectSteps(steps: Folder)
	if connectedSteps == steps then
		applyVisibility(false)
		return
	end

	if childConnection then
		childConnection:Disconnect()
		childConnection = nil
	end

	connectedSteps = steps
	childConnection = steps.ChildAdded:Connect(function(child)
		if not child:IsA("Model") then
			return
		end
		task.defer(function()
			if child.Parent == steps then
				applyVisibility(true)
			end
		end)
	end)
	applyVisibility(true)
end

local function locateCourse()
	refreshToken += 1
	local token = refreshToken

	task.spawn(function()
		for _ = 1, 40 do
			if token ~= refreshToken then
				return
			end

			local world = Workspace:FindFirstChild(WorldConfig.WorldFolderName)
			local course = world and world:FindFirstChild(("Course_%d"):format(LOCAL_PLAYER.UserId))
			local steps = course and course:FindFirstChild("Steps")
			if steps and steps:IsA("Folder") then
				connectSteps(steps)
				return
			end
			task.wait(0.1)
		end
	end)
end

function CourseVisibilityController.start()
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsHeight"):Connect(function()
		applyVisibility(false)
	end)

	Workspace.ChildAdded:Connect(function(child)
		if child.Name == WorldConfig.WorldFolderName then
			locateCourse()
		end
	end)

	LOCAL_PLAYER.CharacterAdded:Connect(function()
		task.delay(0.15, locateCourse)
	end)

	locateCourse()
end

return CourseVisibilityController
