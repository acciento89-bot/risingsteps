--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local TweenService = game:GetService("TweenService")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local WorldConfig = require(root.config.WorldConfig)

local HudController = {}

local LOCAL_PLAYER = Players.LocalPlayer

local function hex(hexColor: string): Color3
	local value = hexColor:gsub("#", "")
	local r = tonumber(value:sub(1, 2), 16)
	local g = tonumber(value:sub(3, 4), 16)
	local b = tonumber(value:sub(5, 6), 16)
	assert(r and g and b, "invalid configured hex color")
	return Color3.fromRGB(r, g, b)
end

local COLORS = {
	Foundation = hex(WorldConfig.Palette.Foundation),
	Steel = hex(WorldConfig.Palette.Steel),
	Warm = hex(WorldConfig.Palette.WarmAccent),
	Active = hex(WorldConfig.Palette.GameplayAccent),
	Fail = Color3.fromRGB(255, 93, 108),
	Text = Color3.fromRGB(242, 247, 252),
	Muted = Color3.fromRGB(168, 180, 196),
}

local function addCorner(parent: Instance, radius: number)
	local corner = Instance.new("UICorner")
	corner.CornerRadius = UDim.new(0, radius)
	corner.Parent = parent
end

local function addStroke(parent: Instance, color: Color3, transparency: number)
	local stroke = Instance.new("UIStroke")
	stroke.Color = color
	stroke.Transparency = transparency
	stroke.Thickness = 1
	stroke.Parent = parent
end

local function makeLabel(
	parent: Instance,
	name: string,
	text: string,
	size: UDim2,
	position: UDim2,
	font: Enum.Font,
	textSize: number,
	color: Color3
): TextLabel
	local label = Instance.new("TextLabel")
	label.Name = name
	label.BackgroundTransparency = 1
	label.Size = size
	label.Position = position
	label.Font = font
	label.Text = text
	label.TextSize = textSize
	label.TextColor3 = color
	label.TextXAlignment = Enum.TextXAlignment.Left
	label.TextYAlignment = Enum.TextYAlignment.Center
	label.Parent = parent
	return label
end

local function createHud()
	local playerGui = LOCAL_PLAYER:WaitForChild("PlayerGui")
	local existing = playerGui:FindFirstChild("RisingStepsHUD")
	if existing then
		existing:Destroy()
	end

	local gui = Instance.new("ScreenGui")
	gui.Name = "RisingStepsHUD"
	gui.ResetOnSpawn = false
	gui.IgnoreGuiInset = false
	gui.DisplayOrder = 20
	gui.ZIndexBehavior = Enum.ZIndexBehavior.Sibling
	gui.Parent = playerGui

	local scale = Instance.new("UIScale")
	scale.Name = "ResponsiveScale"
	scale.Scale = 1
	scale.Parent = gui

	local metrics = Instance.new("Frame")
	metrics.Name = "Metrics"
	metrics.AnchorPoint = Vector2.new(0, 0)
	metrics.Position = UDim2.fromOffset(18, 18)
	metrics.Size = UDim2.fromOffset(196, 86)
	metrics.BackgroundColor3 = COLORS.Foundation
	metrics.BackgroundTransparency = 0.12
	metrics.BorderSizePixel = 0
	metrics.Parent = gui
	addCorner(metrics, 14)
	addStroke(metrics, COLORS.Active, 0.68)

	local gradient = Instance.new("UIGradient")
	gradient.Rotation = 115
	gradient.Color = ColorSequence.new({
		ColorSequenceKeypoint.new(0, COLORS.Foundation),
		ColorSequenceKeypoint.new(1, COLORS.Steel),
	})
	gradient.Parent = metrics

	makeLabel(
		metrics,
		"HeightCaption",
		"HEIGHT",
		UDim2.fromOffset(76, 22),
		UDim2.fromOffset(14, 9),
		Enum.Font.GothamBold,
		11,
		COLORS.Muted
	)

	local heightValue = makeLabel(
		metrics,
		"HeightValue",
		"0",
		UDim2.fromOffset(102, 42),
		UDim2.fromOffset(14, 28),
		Enum.Font.GothamBold,
		30,
		COLORS.Text
	)

	makeLabel(
		metrics,
		"PBCaption",
		"PB",
		UDim2.fromOffset(32, 20),
		UDim2.fromOffset(128, 11),
		Enum.Font.GothamBold,
		10,
		COLORS.Muted
	)

	local pbValue = makeLabel(
		metrics,
		"PBValue",
		"0",
		UDim2.fromOffset(52, 30),
		UDim2.fromOffset(128, 29),
		Enum.Font.GothamBold,
		18,
		COLORS.Warm
	)

	local streak = Instance.new("Frame")
	streak.Name = "Streak"
	streak.Position = UDim2.fromOffset(18, 114)
	streak.Size = UDim2.fromOffset(124, 38)
	streak.BackgroundColor3 = COLORS.Steel
	streak.BackgroundTransparency = 0.14
	streak.BorderSizePixel = 0
	streak.Parent = gui
	addCorner(streak, 12)
	addStroke(streak, COLORS.Warm, 0.62)

	local streakValue = makeLabel(
		streak,
		"Value",
		"STREAK  0",
		UDim2.new(1, -20, 1, 0),
		UDim2.fromOffset(10, 0),
		Enum.Font.GothamBold,
		14,
		COLORS.Text
	)

	local gradeLabel = Instance.new("TextLabel")
	gradeLabel.Name = "LandingGrade"
	gradeLabel.AnchorPoint = Vector2.new(0.5, 0.5)
	gradeLabel.Position = UDim2.fromScale(0.5, 0.36)
	gradeLabel.Size = UDim2.fromOffset(280, 58)
	gradeLabel.BackgroundTransparency = 1
	gradeLabel.Font = Enum.Font.GothamBlack
	gradeLabel.Text = ""
	gradeLabel.TextSize = 34
	gradeLabel.TextTransparency = 1
	gradeLabel.TextStrokeTransparency = 0.72
	gradeLabel.TextColor3 = COLORS.Active
	gradeLabel.Parent = gui

	local result = Instance.new("Frame")
	result.Name = "Result"
	result.AnchorPoint = Vector2.new(0.5, 0.5)
	result.Position = UDim2.fromScale(0.5, 0.47)
	result.Size = UDim2.fromOffset(300, 112)
	result.BackgroundColor3 = COLORS.Foundation
	result.BackgroundTransparency = 0.08
	result.BorderSizePixel = 0
	result.Visible = false
	result.Parent = gui
	addCorner(result, 16)
	addStroke(result, COLORS.Fail, 0.36)

	local resultTitle = makeLabel(
		result,
		"Title",
		"RUN OVER",
		UDim2.new(1, -32, 0, 42),
		UDim2.fromOffset(16, 12),
		Enum.Font.GothamBlack,
		22,
		COLORS.Text
	)
	resultTitle.TextXAlignment = Enum.TextXAlignment.Center

	local resultDetail = makeLabel(
		result,
		"Detail",
		"Height 0  •  PB 0",
		UDim2.new(1, -32, 0, 28),
		UDim2.fromOffset(16, 60),
		Enum.Font.GothamMedium,
		14,
		COLORS.Muted
	)
	resultDetail.TextXAlignment = Enum.TextXAlignment.Center

	local function updateResponsiveScale()
		local camera = Workspace.CurrentCamera
		if not camera then
			return
		end
		local width = camera.ViewportSize.X
		scale.Scale = math.clamp(width / 900, 0.82, 1.08)
	end

	local function updateMetrics(height: number, streakCount: number, pb: number)
		heightValue.Text = tostring(height)
		pbValue.Text = tostring(pb)
		streakValue.Text = ("STREAK  %d"):format(streakCount)
	end

	local gradeTween: Tween? = nil
	local function showGrade(grade: string)
		if gradeTween then
			gradeTween:Cancel()
		end

		if grade == "Perfect" then
			gradeLabel.TextColor3 = Color3.fromRGB(236, 255, 255)
		elseif grade == "Clean" then
			gradeLabel.TextColor3 = COLORS.Warm
		else
			gradeLabel.TextColor3 = COLORS.Fail
		end

		gradeLabel.Text = string.upper(grade)
		gradeLabel.Position = UDim2.fromScale(0.5, 0.36)
		gradeLabel.TextTransparency = 0
		gradeLabel.TextStrokeTransparency = 0.72

		gradeTween =
			TweenService:Create(gradeLabel, TweenInfo.new(0.62, Enum.EasingStyle.Quint, Enum.EasingDirection.Out), {
				Position = UDim2.fromScale(0.5, 0.325),
				TextTransparency = 1,
				TextStrokeTransparency = 1,
			})
		gradeTween:Play()
	end

	local function showFailure(height: number, pb: number)
		resultDetail.Text = ("Height %d  •  PB %d"):format(height, pb)
		result.Visible = true
		result.BackgroundTransparency = 0.08
	end

	local remoteFolder = root:WaitForChild("Remotes")
	local stateSnapshot = remoteFolder:WaitForChild("StateSnapshot")
	assert(stateSnapshot:IsA("RemoteEvent"), "StateSnapshot must be a RemoteEvent")

	stateSnapshot.OnClientEvent:Connect(function(snapshot)
		if type(snapshot) ~= "table" then
			return
		end

		local height = if type(snapshot.Height) == "number" then snapshot.Height else 0
		local streakCount = if type(snapshot.Streak) == "number" then snapshot.Streak else 0
		local pb = if type(snapshot.PB) == "number" then snapshot.PB else 0
		updateMetrics(height, streakCount, pb)

		if snapshot.Kind == "Progress" and type(snapshot.Grade) == "string" then
			showGrade(snapshot.Grade)
		elseif snapshot.Kind == "FallBoundary" or snapshot.Kind == "HumanoidDied" then
			showFailure(height, pb)
		elseif snapshot.Kind == "RunReset" then
			result.Visible = false
		end
	end)

	local function syncFromAttributes()
		local height = LOCAL_PLAYER:GetAttribute("RisingStepsHeight")
		local streakCount = LOCAL_PLAYER:GetAttribute("RisingStepsStreak")
		local pb = LOCAL_PLAYER:GetAttribute("RisingStepsPB")
		updateMetrics(
			if type(height) == "number" then height else 0,
			if type(streakCount) == "number" then streakCount else 0,
			if type(pb) == "number" then pb else 0
		)
	end

	syncFromAttributes()
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsHeight"):Connect(syncFromAttributes)
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsStreak"):Connect(syncFromAttributes)
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsPB"):Connect(syncFromAttributes)

	updateResponsiveScale()
	Workspace:GetPropertyChangedSignal("CurrentCamera"):Connect(updateResponsiveScale)
	if Workspace.CurrentCamera then
		Workspace.CurrentCamera:GetPropertyChangedSignal("ViewportSize"):Connect(updateResponsiveScale)
	end
end

function HudController.start()
	createHud()
end

return HudController
