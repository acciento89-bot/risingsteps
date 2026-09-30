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

	local coinsLabel = makeLabel(
		gui,
		"Coins",
		"◈ 0",
		UDim2.fromOffset(120, 38),
		UDim2.new(1, -138, 0, 20),
		Enum.Font.GothamBold,
		16,
		COLORS.Warm
	)
	coinsLabel.TextXAlignment = Enum.TextXAlignment.Right

	local tutorial = Instance.new("Frame")
	tutorial.Name = "Tutorial"
	tutorial.AnchorPoint = Vector2.new(0.5, 0)
	tutorial.Position = UDim2.new(0.5, 0, 0, 20)
	tutorial.Size = UDim2.fromOffset(360, 92)
	tutorial.BackgroundColor3 = COLORS.Foundation
	tutorial.BackgroundTransparency = 0.08
	tutorial.BorderSizePixel = 0
	tutorial.Visible = false
	tutorial.Parent = gui
	addCorner(tutorial, 16)
	addStroke(tutorial, COLORS.Active, 0.42)

	local tutorialTitle = makeLabel(
		tutorial,
		"Title",
		"CLIMB UP ↑",
		UDim2.new(1, -28, 0, 34),
		UDim2.fromOffset(14, 8),
		Enum.Font.GothamBlack,
		19,
		COLORS.Text
	)
	tutorialTitle.TextXAlignment = Enum.TextXAlignment.Center

	local tutorialBody = makeLabel(
		tutorial,
		"Body",
		"Land on the next step. Lower steps warn, then retract behind you.",
		UDim2.new(1, -28, 0, 38),
		UDim2.fromOffset(14, 42),
		Enum.Font.GothamMedium,
		13,
		COLORS.Muted
	)
	tutorialBody.TextWrapped = true
	tutorialBody.TextXAlignment = Enum.TextXAlignment.Center

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

	local retryButton = Instance.new("TextButton")
	retryButton.Name = "RetryButton"
	retryButton.AnchorPoint = Vector2.new(0.5, 1)
	retryButton.Position = UDim2.new(0.5, 0, 1, -10)
	retryButton.Size = UDim2.fromOffset(164, 48)
	retryButton.BackgroundColor3 = COLORS.Active
	retryButton.BorderSizePixel = 0
	retryButton.Font = Enum.Font.GothamBold
	retryButton.Text = "RETRY"
	retryButton.TextSize = 16
	retryButton.TextColor3 = COLORS.Foundation
	retryButton.Parent = result
	addCorner(retryButton, 12)

	result.Size = UDim2.fromOffset(300, 166)

	local celebration = Instance.new("TextLabel")
	celebration.Name = "Celebration"
	celebration.AnchorPoint = Vector2.new(0.5, 0.5)
	celebration.Position = UDim2.fromScale(0.5, 0.26)
	celebration.Size = UDim2.fromOffset(360, 50)
	celebration.BackgroundTransparency = 1
	celebration.Font = Enum.Font.GothamBlack
	celebration.Text = ""
	celebration.TextSize = 26
	celebration.TextColor3 = COLORS.Warm
	celebration.TextTransparency = 1
	celebration.Parent = gui

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
		local coins = LOCAL_PLAYER:GetAttribute("RisingStepsCoins")
		coinsLabel.Text = ("◈ %d"):format(if type(coins) == "number" then coins else 0)
	end

	local function celebrate(text: string, color: Color3)
		celebration.Text = text
		celebration.TextColor3 = color
		celebration.TextTransparency = 0
		TweenService:Create(celebration, TweenInfo.new(1.1, Enum.EasingStyle.Quint), {
			TextTransparency = 1,
			Position = UDim2.fromScale(0.5, 0.22),
		}):Play()
		task.delay(1.15, function()
			celebration.Position = UDim2.fromScale(0.5, 0.26)
		end)
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
	local playerCommand = remoteFolder:WaitForChild("PlayerCommand")
	local requestBootstrap = remoteFolder:WaitForChild("RequestBootstrap")
	assert(stateSnapshot:IsA("RemoteEvent"), "StateSnapshot must be a RemoteEvent")
	assert(playerCommand:IsA("RemoteEvent"), "PlayerCommand must be a RemoteEvent")
	assert(requestBootstrap:IsA("RemoteFunction"), "RequestBootstrap must be a RemoteFunction")

	retryButton.Activated:Connect(function()
		playerCommand:FireServer({ Action = "Retry" })
	end)

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
			tutorial.Visible = false
			if snapshot.NewPB == true then
				celebrate("NEW PB", COLORS.Warm)
			elseif type(snapshot.UnlockedAchievements) == "table" and #snapshot.UnlockedAchievements > 0 then
				celebrate("ACHIEVEMENT", COLORS.Active)
			elseif type(snapshot.ChallengeCoins) == "number" and snapshot.ChallengeCoins > 0 then
				celebrate("DAILY COMPLETE", COLORS.Warm)
			end
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
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsCoins"):Connect(syncFromAttributes)

	task.spawn(function()
		local ok, profile = pcall(function()
			return requestBootstrap:InvokeServer()
		end)
		if ok and type(profile) == "table" then
			tutorial.Visible = profile.TutorialComplete ~= true
		end
	end)

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
