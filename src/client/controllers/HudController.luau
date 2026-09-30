--!strict

local GuiService = game:GetService("GuiService")
local MarketplaceService = game:GetService("MarketplaceService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local TweenService = game:GetService("TweenService")
local UserInputService = game:GetService("UserInputService")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local CosmeticConfig = require(root.config.CosmeticConfig)
local MonetizationConfig = require(root.config.MonetizationConfig)
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
	Concrete = hex(WorldConfig.Palette.Concrete),
	Warm = hex(WorldConfig.Palette.WarmAccent),
	Active = hex(WorldConfig.Palette.GameplayAccent),
	Warning = Color3.fromRGB(255, 159, 67),
	Fail = Color3.fromRGB(255, 93, 108),
	Text = Color3.fromRGB(242, 247, 252),
	Muted = Color3.fromRGB(168, 180, 196),
	Dim = Color3.fromRGB(8, 11, 16),
}

local PREVIEW_COLORS = {
	none = COLORS.Concrete,
	ion = Color3.fromRGB(94, 235, 255),
	ember = Color3.fromRGB(255, 146, 77),
	basic = COLORS.Concrete,
	spark = Color3.fromRGB(255, 194, 84),
	pulse = Color3.fromRGB(118, 161, 255),
	steel = COLORS.Steel,
	forge = Color3.fromRGB(184, 93, 42),
	frost = Color3.fromRGB(114, 214, 236),
	default = Color3.fromRGB(82, 111, 137),
	dusk = Color3.fromRGB(117, 79, 125),
	storm = Color3.fromRGB(74, 90, 119),
}

local function addCorner(parent: Instance, radius: number)
	local corner = Instance.new("UICorner")
	corner.CornerRadius = UDim.new(0, radius)
	corner.Parent = parent
end

local function addStroke(parent: Instance, color: Color3, transparency: number, thickness: number?)
	local stroke = Instance.new("UIStroke")
	stroke.Color = color
	stroke.Transparency = transparency
	stroke.Thickness = thickness or 1
	stroke.Parent = parent
end

local function addGradient(parent: Instance, first: Color3, second: Color3, rotation: number)
	local gradient = Instance.new("UIGradient")
	gradient.Rotation = rotation
	gradient.Color = ColorSequence.new({
		ColorSequenceKeypoint.new(0, first),
		ColorSequenceKeypoint.new(1, second),
	})
	gradient.Parent = parent
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

local function makeButton(
	parent: Instance,
	name: string,
	text: string,
	size: UDim2,
	position: UDim2,
	background: Color3,
	textColor: Color3
): TextButton
	local button = Instance.new("TextButton")
	button.Name = name
	button.Size = size
	button.Position = position
	button.BackgroundColor3 = background
	button.BorderSizePixel = 0
	button.Font = Enum.Font.GothamBold
	button.Text = text
	button.TextSize = 14
	button.TextColor3 = textColor
	button.AutoButtonColor = true
	button.Selectable = true
	button.Parent = parent
	addCorner(button, 12)
	return button
end

local function makePreviewArt(parent: Frame, category: string, itemId: string)
	local accent = PREVIEW_COLORS[itemId] or COLORS.Active
	local inset = Instance.new("Frame")
	inset.Name = "Artwork"
	inset.Position = UDim2.fromOffset(5, 5)
	inset.Size = UDim2.new(1, -10, 1, -10)
	inset.BackgroundColor3 = COLORS.Foundation
	inset.BackgroundTransparency = 0.18
	inset.BorderSizePixel = 0
	inset.ZIndex = parent.ZIndex + 1
	inset.Parent = parent
	addCorner(inset, 9)
	addGradient(inset, COLORS.Foundation, COLORS.Steel, 135)

	local function bar(name: string, size: UDim2, position: UDim2, rotation: number, transparency: number)
		local frame = Instance.new("Frame")
		frame.Name = name
		frame.Size = size
		frame.Position = position
		frame.AnchorPoint = Vector2.new(0.5, 0.5)
		frame.BackgroundColor3 = accent
		frame.BackgroundTransparency = transparency
		frame.BorderSizePixel = 0
		frame.Rotation = rotation
		frame.ZIndex = inset.ZIndex + 1
		frame.Parent = inset
		addCorner(frame, 6)
		return frame
	end

	if category == "Trails" then
		bar("TrailBack", UDim2.fromOffset(26, 4), UDim2.fromScale(0.43, 0.68), -18, 0.58)
		bar("TrailMid", UDim2.fromOffset(30, 5), UDim2.fromScale(0.46, 0.54), -18, 0.32)
		bar("TrailFront", UDim2.fromOffset(24, 6), UDim2.fromScale(0.52, 0.39), -18, 0.08)
		local orb = bar("TrailHead", UDim2.fromOffset(12, 12), UDim2.fromScale(0.72, 0.28), 0, 0)
		addCorner(orb, 99)
	elseif category == "LandingEffects" then
		for index, diameter in { 32, 23, 14 } do
			local ring = Instance.new("Frame")
			ring.Name = ("Ring%02d"):format(index)
			ring.AnchorPoint = Vector2.new(0.5, 0.5)
			ring.Position = UDim2.fromScale(0.5, 0.54)
			ring.Size = UDim2.fromOffset(diameter, diameter * 0.48)
			ring.BackgroundTransparency = 1
			ring.BorderSizePixel = 0
			ring.ZIndex = inset.ZIndex + 1
			ring.Parent = inset
			addCorner(ring, 99)
			addStroke(ring, accent, 0.12 + index * 0.14, 2)
		end
		bar("ImpactCore", UDim2.fromOffset(9, 9), UDim2.fromScale(0.5, 0.52), 0, 0.02)
	elseif category == "StepThemes" then
		bar("Deck", UDim2.fromOffset(34, 15), UDim2.fromScale(0.5, 0.54), 0, 0.05)
		local surface = bar("Surface", UDim2.fromOffset(27, 7), UDim2.fromScale(0.5, 0.43), 0, 0.44)
		surface.BackgroundColor3 = COLORS.Text
		bar("SupportLeft", UDim2.fromOffset(5, 18), UDim2.fromScale(0.37, 0.71), 12, 0.18)
		bar("SupportRight", UDim2.fromOffset(5, 18), UDim2.fromScale(0.63, 0.71), -12, 0.18)
	elseif category == "EnvironmentThemes" then
		local heights = { 15, 24, 19, 31, 21 }
		for index, height in heights do
			local tower = bar(
				("Tower%02d"):format(index),
				UDim2.fromOffset(5, height),
				UDim2.fromScale(0.24 + (index - 1) * 0.13, 0.70 - height / 180),
				0,
				0.20 + index * 0.05
			)
			tower.AnchorPoint = Vector2.new(0.5, 1)
		end
		local horizon = bar("Horizon", UDim2.fromOffset(34, 2), UDim2.fromScale(0.5, 0.76), 0, 0.25)
		horizon.BackgroundColor3 = COLORS.Text
	else
		bar("MarkA", UDim2.fromOffset(28, 6), UDim2.fromScale(0.5, 0.42), 25, 0.05)
		bar("MarkB", UDim2.fromOffset(28, 6), UDim2.fromScale(0.5, 0.60), -25, 0.35)
	end
end

local function createHud()
	local playerGui = LOCAL_PLAYER:WaitForChild("PlayerGui")
	local existing = playerGui:FindFirstChild("RisingStepsHUD")
	if existing then
		existing:Destroy()
	end

	local remoteFolder = root:WaitForChild("Remotes")
	local stateSnapshot = remoteFolder:WaitForChild("StateSnapshot")
	local playerCommand = remoteFolder:WaitForChild("PlayerCommand")
	local shopCommand = remoteFolder:WaitForChild("ShopCommand")
	local requestBootstrap = remoteFolder:WaitForChild("RequestBootstrap")
	assert(stateSnapshot:IsA("RemoteEvent"), "StateSnapshot must be a RemoteEvent")
	assert(playerCommand:IsA("RemoteEvent"), "PlayerCommand must be a RemoteEvent")
	assert(shopCommand:IsA("RemoteEvent"), "ShopCommand must be a RemoteEvent")
	assert(requestBootstrap:IsA("RemoteFunction"), "RequestBootstrap must be a RemoteFunction")

	local profile: any = nil
	local itemButtons: { [string]: { [string]: TextButton } } = {}
	local gradeTween: Tween? = nil
	local warningTween: Tween? = nil
	local celebrationTween: Tween? = nil

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
	metrics.Position = UDim2.fromOffset(18, 18)
	metrics.Size = UDim2.fromOffset(202, 88)
	metrics.BackgroundColor3 = COLORS.Foundation
	metrics.BackgroundTransparency = 0.08
	metrics.BorderSizePixel = 0
	metrics.Parent = gui
	addCorner(metrics, 14)
	addStroke(metrics, COLORS.Active, 0.68)
	addGradient(metrics, COLORS.Foundation, COLORS.Steel, 115)

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
		UDim2.fromOffset(106, 42),
		UDim2.fromOffset(14, 28),
		Enum.Font.GothamBlack,
		30,
		COLORS.Text
	)

	makeLabel(
		metrics,
		"PBCaption",
		"PB",
		UDim2.fromOffset(32, 20),
		UDim2.fromOffset(134, 11),
		Enum.Font.GothamBold,
		10,
		COLORS.Muted
	)

	local pbValue = makeLabel(
		metrics,
		"PBValue",
		"0",
		UDim2.fromOffset(52, 30),
		UDim2.fromOffset(134, 29),
		Enum.Font.GothamBold,
		18,
		COLORS.Warm
	)

	local streak = Instance.new("Frame")
	streak.Name = "Streak"
	streak.Position = UDim2.fromOffset(18, 114)
	streak.Size = UDim2.fromOffset(128, 40)
	streak.BackgroundColor3 = COLORS.Steel
	streak.BackgroundTransparency = 0.10
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

	local topRight = Instance.new("Frame")
	topRight.Name = "TopRight"
	topRight.AnchorPoint = Vector2.new(1, 0)
	topRight.Position = UDim2.new(1, -18, 0, 132)
	topRight.Size = UDim2.fromOffset(212, 48)
	topRight.BackgroundTransparency = 1
	topRight.Parent = gui

	local coinsPill = Instance.new("Frame")
	coinsPill.Name = "CoinsPill"
	coinsPill.Size = UDim2.fromOffset(92, 48)
	coinsPill.BackgroundColor3 = COLORS.Foundation
	coinsPill.BackgroundTransparency = 0.08
	coinsPill.BorderSizePixel = 0
	coinsPill.Parent = topRight
	addCorner(coinsPill, 14)
	addStroke(coinsPill, COLORS.Warm, 0.58)

	local coinsLabel = makeLabel(
		coinsPill,
		"Coins",
		"◈ 0",
		UDim2.new(1, -18, 1, 0),
		UDim2.fromOffset(9, 0),
		Enum.Font.GothamBold,
		15,
		COLORS.Warm
	)
	coinsLabel.TextXAlignment = Enum.TextXAlignment.Center

	local shopButton = makeButton(
		topRight,
		"ShopButton",
		"SHOP",
		UDim2.fromOffset(108, 48),
		UDim2.fromOffset(104, 0),
		COLORS.Active,
		COLORS.Foundation
	)

	local tutorial = Instance.new("Frame")
	tutorial.Name = "Tutorial"
	tutorial.AnchorPoint = Vector2.new(0.5, 0)
	tutorial.Position = UDim2.new(0.5, 0, 0, 78)
	tutorial.Size = UDim2.fromOffset(370, 94)
	tutorial.BackgroundColor3 = COLORS.Foundation
	tutorial.BackgroundTransparency = 0.06
	tutorial.BorderSizePixel = 0
	tutorial.Visible = false
	tutorial.Parent = gui
	addCorner(tutorial, 16)
	addStroke(tutorial, COLORS.Active, 0.36, 1.4)

	local tutorialTitle = makeLabel(
		tutorial,
		"Title",
		"CLIMB UP  ↑",
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
		"Land on the next step. Lower steps WARN, then RETRACT behind you.",
		UDim2.new(1, -28, 0, 38),
		UDim2.fromOffset(14, 44),
		Enum.Font.GothamMedium,
		13,
		COLORS.Muted
	)
	tutorialBody.TextWrapped = true
	tutorialBody.TextXAlignment = Enum.TextXAlignment.Center

	local warning = Instance.new("Frame")
	warning.Name = "Warning"
	warning.AnchorPoint = Vector2.new(0.5, 0)
	warning.Position = UDim2.new(0.5, 0, 0, 22)
	warning.Size = UDim2.fromOffset(266, 46)
	warning.BackgroundColor3 = COLORS.Foundation
	warning.BackgroundTransparency = 0.05
	warning.BorderSizePixel = 0
	warning.Visible = false
	warning.Parent = gui
	addCorner(warning, 13)
	addStroke(warning, COLORS.Warning, 0.20, 1.5)

	local warningText = makeLabel(
		warning,
		"Text",
		"!   LOWER STEP RETRACTING",
		UDim2.new(1, -20, 1, 0),
		UDim2.fromOffset(10, 0),
		Enum.Font.GothamBold,
		13,
		COLORS.Warning
	)
	warningText.TextXAlignment = Enum.TextXAlignment.Center

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

	local result = Instance.new("Frame")
	result.Name = "Result"
	result.AnchorPoint = Vector2.new(0.5, 0.5)
	result.Position = UDim2.fromScale(0.5, 0.47)
	result.Size = UDim2.fromOffset(324, 188)
	result.BackgroundColor3 = COLORS.Foundation
	result.BackgroundTransparency = 0.04
	result.BorderSizePixel = 0
	result.Visible = false
	result.Parent = gui
	addCorner(result, 18)
	addStroke(result, COLORS.Fail, 0.28, 1.4)
	addGradient(result, COLORS.Foundation, COLORS.Steel, 120)

	local resultTitle = makeLabel(
		result,
		"Title",
		"RUN OVER",
		UDim2.new(1, -32, 0, 42),
		UDim2.fromOffset(16, 14),
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
		UDim2.fromOffset(16, 58),
		Enum.Font.GothamMedium,
		14,
		COLORS.Muted
	)
	resultDetail.TextXAlignment = Enum.TextXAlignment.Center

	local retryButton = makeButton(
		result,
		"RetryButton",
		"RETRY",
		UDim2.fromOffset(136, 48),
		UDim2.fromOffset(18, 118),
		COLORS.Active,
		COLORS.Foundation
	)

	local reviveButton = makeButton(
		result,
		"ReviveButton",
		"REVIVE",
		UDim2.fromOffset(136, 48),
		UDim2.fromOffset(170, 118),
		COLORS.Warm,
		COLORS.Foundation
	)
	reviveButton.Visible = false

	local dimmer = Instance.new("Frame")
	dimmer.Name = "ShopDimmer"
	dimmer.Size = UDim2.fromScale(1, 1)
	dimmer.BackgroundColor3 = COLORS.Dim
	dimmer.BackgroundTransparency = 0.28
	dimmer.BorderSizePixel = 0
	dimmer.Visible = false
	dimmer.ZIndex = 50
	dimmer.Parent = gui

	local shop = Instance.new("Frame")
	shop.Name = "ShopPanel"
	shop.AnchorPoint = Vector2.new(0.5, 0.5)
	shop.Position = UDim2.fromScale(0.5, 0.5)
	shop.Size = UDim2.fromScale(0.88, 0.86)
	shop.BackgroundColor3 = COLORS.Foundation
	shop.BorderSizePixel = 0
	shop.Visible = false
	shop.ZIndex = 51
	shop.Parent = gui
	addCorner(shop, 20)
	addStroke(shop, COLORS.Active, 0.50, 1.2)
	addGradient(shop, COLORS.Foundation, COLORS.Steel, 115)

	local shopConstraint = Instance.new("UISizeConstraint")
	shopConstraint.MinSize = Vector2.new(310, 480)
	shopConstraint.MaxSize = Vector2.new(500, 680)
	shopConstraint.Parent = shop

	local shopTitle = makeLabel(
		shop,
		"Title",
		"RISING SUPPLY",
		UDim2.new(1, -92, 0, 52),
		UDim2.fromOffset(22, 10),
		Enum.Font.GothamBlack,
		21,
		COLORS.Text
	)
	shopTitle.ZIndex = 52

	local closeButton = makeButton(
		shop,
		"CloseButton",
		"×",
		UDim2.fromOffset(48, 48),
		UDim2.new(1, -62, 0, 12),
		COLORS.Steel,
		COLORS.Text
	)
	closeButton.TextSize = 24
	closeButton.ZIndex = 52

	local scroll = Instance.new("ScrollingFrame")
	scroll.Name = "Content"
	scroll.Position = UDim2.fromOffset(14, 68)
	scroll.Size = UDim2.new(1, -28, 1, -82)
	scroll.BackgroundTransparency = 1
	scroll.BorderSizePixel = 0
	scroll.ScrollBarThickness = 4
	scroll.ScrollBarImageColor3 = COLORS.Active
	scroll.AutomaticCanvasSize = Enum.AutomaticSize.Y
	scroll.CanvasSize = UDim2.new()
	scroll.ScrollingDirection = Enum.ScrollingDirection.Y
	scroll.ZIndex = 52
	scroll.Parent = shop

	local layout = Instance.new("UIListLayout")
	layout.Padding = UDim.new(0, 10)
	layout.SortOrder = Enum.SortOrder.LayoutOrder
	layout.Parent = scroll

	local padding = Instance.new("UIPadding")
	padding.PaddingBottom = UDim.new(0, 14)
	padding.PaddingLeft = UDim.new(0, 4)
	padding.PaddingRight = UDim.new(0, 4)
	padding.Parent = scroll

	local function sectionTitle(text: string, order: number)
		local label = makeLabel(
			scroll,
			"Section_" .. tostring(order),
			text,
			UDim2.new(1, -4, 0, 30),
			UDim2.new(),
			Enum.Font.GothamBold,
			12,
			COLORS.Muted
		)
		label.LayoutOrder = order
		label.ZIndex = 53
		return label
	end

	local dailyCard = Instance.new("Frame")
	dailyCard.Name = "DailyCard"
	dailyCard.Size = UDim2.new(1, -4, 0, 104)
	dailyCard.BackgroundColor3 = COLORS.Steel
	dailyCard.BackgroundTransparency = 0.10
	dailyCard.BorderSizePixel = 0
	dailyCard.LayoutOrder = 1
	dailyCard.ZIndex = 53
	dailyCard.Parent = scroll
	addCorner(dailyCard, 14)
	addStroke(dailyCard, COLORS.Warm, 0.62)

	local dailyTitle = makeLabel(
		dailyCard,
		"Title",
		"DAILY SUPPLY",
		UDim2.new(1, -154, 0, 30),
		UDim2.fromOffset(14, 10),
		Enum.Font.GothamBold,
		14,
		COLORS.Text
	)
	dailyTitle.ZIndex = 54

	local dailyDetail = makeLabel(
		dailyCard,
		"Detail",
		"Claim your daily Coins",
		UDim2.new(1, -154, 0, 44),
		UDim2.fromOffset(14, 42),
		Enum.Font.GothamMedium,
		12,
		COLORS.Muted
	)
	dailyDetail.TextWrapped = true
	dailyDetail.ZIndex = 54

	local dailyButton = makeButton(
		dailyCard,
		"ClaimButton",
		"CLAIM",
		UDim2.fromOffset(118, 48),
		UDim2.new(1, -132, 0.5, -24),
		COLORS.Warm,
		COLORS.Foundation
	)
	dailyButton.ZIndex = 54

	local challengeCard = Instance.new("Frame")
	challengeCard.Name = "ChallengeCard"
	challengeCard.Size = UDim2.new(1, -4, 0, 86)
	challengeCard.BackgroundColor3 = COLORS.Steel
	challengeCard.BackgroundTransparency = 0.10
	challengeCard.BorderSizePixel = 0
	challengeCard.LayoutOrder = 2
	challengeCard.ZIndex = 53
	challengeCard.Parent = scroll
	addCorner(challengeCard, 14)
	addStroke(challengeCard, COLORS.Active, 0.64)

	local challengeTitle = makeLabel(
		challengeCard,
		"Title",
		"DAILY CHALLENGE",
		UDim2.new(1, -28, 0, 28),
		UDim2.fromOffset(14, 8),
		Enum.Font.GothamBold,
		13,
		COLORS.Text
	)
	challengeTitle.ZIndex = 54

	local challengeDetail = makeLabel(
		challengeCard,
		"Detail",
		"Loading…",
		UDim2.new(1, -28, 0, 38),
		UDim2.fromOffset(14, 38),
		Enum.Font.GothamMedium,
		12,
		COLORS.Muted
	)
	challengeDetail.ZIndex = 54

	sectionTitle("ASSISTS — assisted runs never improve PB", 10)

	local assistCard = Instance.new("Frame")
	assistCard.Name = "AssistCard"
	assistCard.Size = UDim2.new(1, -4, 0, 112)
	assistCard.BackgroundColor3 = COLORS.Steel
	assistCard.BackgroundTransparency = 0.10
	assistCard.BorderSizePixel = 0
	assistCard.LayoutOrder = 11
	assistCard.ZIndex = 53
	assistCard.Parent = scroll
	addCorner(assistCard, 14)

	local reviveProduct = MonetizationConfig.DeveloperProducts.Revive
	local stabilityProduct = MonetizationConfig.DeveloperProducts.Stability

	local productDetail = makeLabel(
		assistCard,
		"Detail",
		("Revive %d R$  •  Stability %d R$ / %d min"):format(
			reviveProduct.Robux,
			stabilityProduct.Robux,
			math.floor(stabilityProduct.DurationSeconds / 60)
		),
		UDim2.new(1, -28, 0, 34),
		UDim2.fromOffset(14, 10),
		Enum.Font.GothamMedium,
		12,
		COLORS.Muted
	)
	productDetail.ZIndex = 54

	local buyRevive = makeButton(
		assistCard,
		"BuyRevive",
		"BUY REVIVE",
		UDim2.new(0.48, -10, 0, 48),
		UDim2.fromOffset(14, 52),
		COLORS.Warm,
		COLORS.Foundation
	)
	buyRevive.ZIndex = 54

	local buyStability = makeButton(
		assistCard,
		"BuyStability",
		"STABILITY",
		UDim2.new(0.48, -10, 0, 48),
		UDim2.new(0.52, 0, 0, 52),
		COLORS.Active,
		COLORS.Foundation
	)
	buyStability.ZIndex = 54

	local categoryOrder = {
		{ Key = "Trails", Title = "TRAILS" },
		{ Key = "LandingEffects", Title = "LANDING EFFECTS" },
		{ Key = "StepThemes", Title = "STEP THEMES" },
		{ Key = "EnvironmentThemes", Title = "ENVIRONMENT THEMES" },
	}

	local layoutOrder = 20
	for _, category in categoryOrder do
		sectionTitle(category.Title, layoutOrder)
		layoutOrder += 1
		itemButtons[category.Key] = {}

		local config = CosmeticConfig.Categories[category.Key]
		local itemIds = {}
		for itemId in config.Items do
			table.insert(itemIds, itemId)
		end
		table.sort(itemIds)

		for _, itemId in itemIds do
			local item = config.Items[itemId]
			local row = Instance.new("Frame")
			row.Name = category.Key .. "_" .. itemId
			row.Size = UDim2.new(1, -4, 0, 76)
			row.BackgroundColor3 = COLORS.Steel
			row.BackgroundTransparency = 0.12
			row.BorderSizePixel = 0
			row.LayoutOrder = layoutOrder
			row.ZIndex = 53
			row.Parent = scroll
			addCorner(row, 13)
			layoutOrder += 1

			local preview = Instance.new("Frame")
			preview.Name = "Preview"
			preview.Position = UDim2.fromOffset(12, 12)
			preview.Size = UDim2.fromOffset(52, 52)
			preview.BackgroundColor3 = PREVIEW_COLORS[itemId] or COLORS.Active
			preview.BorderSizePixel = 0
			preview.ZIndex = 54
			preview.Parent = row
			addCorner(preview, 12)
			addStroke(preview, COLORS.Text, 0.68)
			addGradient(preview, PREVIEW_COLORS[itemId] or COLORS.Active, COLORS.Foundation, 135)
			makePreviewArt(preview, category.Key, itemId)

			local title = makeLabel(
				row,
				"Title",
				item.DisplayName,
				UDim2.new(1, -196, 0, 28),
				UDim2.fromOffset(76, 10),
				Enum.Font.GothamBold,
				13,
				COLORS.Text
			)
			title.ZIndex = 54

			local priceText = if item.Price == 0 then "Included" else ("◈ %d"):format(item.Price)
			local price = makeLabel(
				row,
				"Price",
				priceText,
				UDim2.new(1, -196, 0, 24),
				UDim2.fromOffset(76, 39),
				Enum.Font.GothamMedium,
				12,
				COLORS.Muted
			)
			price.ZIndex = 54

			local actionButton = makeButton(
				row,
				"Action",
				"BUY",
				UDim2.fromOffset(100, 44),
				UDim2.new(1, -112, 0.5, -22),
				COLORS.Active,
				COLORS.Foundation
			)
			actionButton.ZIndex = 54
			itemButtons[category.Key][itemId] = actionButton

			actionButton.Activated:Connect(function()
				if not profile then
					return
				end
				local owned = profile.Owned and profile.Owned[category.Key]
				if owned and owned[itemId] == true then
					shopCommand:FireServer({
						Action = "Equip",
						Category = category.Key,
						ItemId = itemId,
					})
				else
					shopCommand:FireServer({
						Action = "Purchase",
						Category = category.Key,
						ItemId = itemId,
					})
				end
			end)
		end
	end

	sectionTitle("ACCESSIBILITY", layoutOrder)
	layoutOrder += 1

	local accessibilityCard = Instance.new("Frame")
	accessibilityCard.Name = "Accessibility"
	accessibilityCard.Size = UDim2.new(1, -4, 0, 74)
	accessibilityCard.BackgroundColor3 = COLORS.Steel
	accessibilityCard.BackgroundTransparency = 0.12
	accessibilityCard.BorderSizePixel = 0
	accessibilityCard.LayoutOrder = layoutOrder
	accessibilityCard.ZIndex = 53
	accessibilityCard.Parent = scroll
	addCorner(accessibilityCard, 13)

	local accessLabel = makeLabel(
		accessibilityCard,
		"Label",
		"Reduced motion",
		UDim2.new(1, -154, 1, 0),
		UDim2.fromOffset(14, 0),
		Enum.Font.GothamBold,
		13,
		COLORS.Text
	)
	accessLabel.ZIndex = 54

	local reducedMotionButton = makeButton(
		accessibilityCard,
		"ReducedMotion",
		"OFF",
		UDim2.fromOffset(100, 44),
		UDim2.new(1, -114, 0.5, -22),
		COLORS.Steel,
		COLORS.Text
	)
	reducedMotionButton.ZIndex = 54

	local function reducedMotionEnabled(): boolean
		return LOCAL_PLAYER:GetAttribute("RisingStepsReducedMotion") == true
	end

	local function updateResponsiveScale()
		local camera = Workspace.CurrentCamera
		if not camera then
			return
		end
		local width = camera.ViewportSize.X
		scale.Scale = math.clamp(width / 900, 0.82, 1.08)
		if width <= 760 then
			metrics.Position = UDim2.fromOffset(12, 12)
			topRight.Position = UDim2.new(1, -12, 0, 126)
			tutorial.Size = UDim2.new(1, -28, 0, 94)
		else
			metrics.Position = UDim2.fromOffset(18, 18)
			topRight.Position = UDim2.new(1, -18, 0, 132)
			tutorial.Size = UDim2.fromOffset(370, 94)
		end
	end

	local function updateMetrics(height: number, streakCount: number, pb: number)
		heightValue.Text = tostring(height)
		pbValue.Text = tostring(pb)
		streakValue.Text = ("STREAK  %d"):format(streakCount)
		local coins = LOCAL_PLAYER:GetAttribute("RisingStepsCoins")
		coinsLabel.Text = ("◈ %d"):format(if type(coins) == "number" then coins else 0)
	end

	local function celebrate(text: string, color: Color3)
		if celebrationTween then
			celebrationTween:Cancel()
		end
		celebration.Text = text
		celebration.TextColor3 = color
		celebration.TextTransparency = 0
		celebration.Position = UDim2.fromScale(0.5, 0.26)

		if reducedMotionEnabled() then
			task.delay(0.8, function()
				celebration.TextTransparency = 1
			end)
			return
		end

		celebrationTween = TweenService:Create(celebration, TweenInfo.new(1.1, Enum.EasingStyle.Quint), {
			TextTransparency = 1,
			Position = UDim2.fromScale(0.5, 0.22),
		})
		celebrationTween:Play()
	end

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

		if reducedMotionEnabled() then
			task.delay(0.45, function()
				gradeLabel.TextTransparency = 1
				gradeLabel.TextStrokeTransparency = 1
			end)
			return
		end

		gradeTween =
			TweenService:Create(gradeLabel, TweenInfo.new(0.62, Enum.EasingStyle.Quint, Enum.EasingDirection.Out), {
				Position = UDim2.fromScale(0.5, 0.325),
				TextTransparency = 1,
				TextStrokeTransparency = 1,
			})
		gradeTween:Play()
	end

	local function showWarning()
		if warningTween then
			warningTween:Cancel()
		end
		warning.Visible = true
		warning.BackgroundTransparency = 0.05

		if reducedMotionEnabled() then
			task.delay(0.65, function()
				warning.Visible = false
			end)
			return
		end

		warningTween = TweenService:Create(warning, TweenInfo.new(0.65, Enum.EasingStyle.Quad), {
			BackgroundTransparency = 0.38,
		})
		warningTween:Play()
		task.delay(0.72, function()
			warning.Visible = false
		end)
	end

	local function showFailure(height: number, pb: number, revives: number, revivesUsed: number)
		resultDetail.Text = ("Height %d  •  PB %d"):format(height, pb)
		result.Visible = true
		reviveButton.Visible = revives > 0 and revivesUsed < 1 and height > 0
		if reviveButton.Visible then
			reviveButton.Text = ("REVIVE  (%d)"):format(revives)
		end
		if UserInputService.GamepadEnabled then
			GuiService.SelectedObject = retryButton
		end
	end

	local function refreshShop(nextProfile: any)
		profile = nextProfile
		if not profile then
			return
		end

		local daily = profile.Daily
		if type(daily) == "table" then
			dailyButton.Active = daily.Claimable == true
			dailyButton.Text = if daily.Claimable == true then "CLAIM" else "CLAIMED"
			dailyButton.BackgroundColor3 = if daily.Claimable == true then COLORS.Warm else COLORS.Concrete
			dailyDetail.Text = ("Daily Coins  •  streak %d"):format(
				if type(daily.ClaimStreak) == "number" then daily.ClaimStreak else 0
			)
		end

		local challenge = profile.Challenge
		if type(challenge) == "table" then
			local suffix = if challenge.Claimed == true then "  ✓" else ""
			challengeDetail.Text = ("%s  %d/%d  •  +%d ◈%s"):format(
				tostring(challenge.Kind),
				tonumber(challenge.Progress) or 0,
				tonumber(challenge.Target) or 0,
				tonumber(challenge.RewardCoins) or 0,
				suffix
			)
		end

		for category, buttons in itemButtons do
			local owned = profile.Owned and profile.Owned[category]
			local equipped = profile.Equipped and profile.Equipped[category]
			for itemId, button in buttons do
				if equipped == itemId then
					button.Text = "EQUIPPED"
					button.BackgroundColor3 = COLORS.Concrete
					button.Active = false
				elseif owned and owned[itemId] == true then
					button.Text = "EQUIP"
					button.BackgroundColor3 = COLORS.Active
					button.Active = true
				else
					button.Text = "BUY"
					button.BackgroundColor3 = COLORS.Warm
					button.Active = true
				end
			end
		end

		local reduced = profile.Settings and profile.Settings.ReducedMotion == true
		reducedMotionButton.Text = if reduced then "ON" else "OFF"
		reducedMotionButton.BackgroundColor3 = if reduced then COLORS.Active else COLORS.Steel
	end

	local function setShopVisible(visible: boolean)
		shop.Visible = visible
		dimmer.Visible = visible
		if visible then
			refreshShop(profile)
			if UserInputService.GamepadEnabled then
				GuiService.SelectedObject = closeButton
			end
		elseif GuiService.SelectedObject and GuiService.SelectedObject:IsDescendantOf(shop) then
			GuiService.SelectedObject = shopButton
		end
	end

	shopButton.Activated:Connect(function()
		setShopVisible(true)
	end)
	closeButton.Activated:Connect(function()
		setShopVisible(false)
	end)
	dimmer.InputBegan:Connect(function(input)
		if input.UserInputType == Enum.UserInputType.MouseButton1 then
			setShopVisible(false)
		end
	end)

	retryButton.Activated:Connect(function()
		playerCommand:FireServer({ Action = "Retry" })
	end)
	reviveButton.Activated:Connect(function()
		playerCommand:FireServer({ Action = "UseRevive" })
	end)

	dailyButton.Activated:Connect(function()
		shopCommand:FireServer({ Action = "ClaimDaily" })
	end)

	reducedMotionButton.Activated:Connect(function()
		shopCommand:FireServer({
			Action = "ReducedMotion",
			Enabled = not reducedMotionEnabled(),
		})
	end)

	local function promptProduct(product: any)
		if product.ProductId <= 0 then
			celebrate("PRODUCT ID PENDING", COLORS.Warning)
			return
		end
		MarketplaceService:PromptProductPurchase(LOCAL_PLAYER, product.ProductId)
	end

	buyRevive.Activated:Connect(function()
		promptProduct(reviveProduct)
	end)
	buyStability.Activated:Connect(function()
		promptProduct(stabilityProduct)
	end)

	stateSnapshot.OnClientEvent:Connect(function(snapshot)
		if type(snapshot) ~= "table" then
			return
		end

		if snapshot.Kind == "Profile" and type(snapshot.Profile) == "table" then
			refreshShop(snapshot.Profile)
			local resultPayload = snapshot.Result
			if type(resultPayload) == "table" and resultPayload.Ok == false then
				celebrate(string.upper(tostring(resultPayload.Reason)), COLORS.Fail)
			elseif type(resultPayload) == "table" and resultPayload.Action == "ClaimDaily" then
				celebrate(("DAILY +%d ◈"):format(tonumber(resultPayload.RewardCoins) or 0), COLORS.Warm)
			end
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
		elseif snapshot.Kind == "StepWarning" then
			showWarning()
		elseif snapshot.Kind == "FallBoundary" or snapshot.Kind == "HumanoidDied" then
			showFailure(
				height,
				pb,
				if type(snapshot.Revives) == "number" then snapshot.Revives else 0,
				if type(snapshot.RevivesUsed) == "number" then snapshot.RevivesUsed else 0
			)
		elseif snapshot.Kind == "RunReset" then
			result.Visible = false
			warning.Visible = false
		elseif snapshot.Kind == "Revived" then
			result.Visible = false
			celebrate("REVIVED", COLORS.Active)
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
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsReducedMotion"):Connect(function()
		if profile and profile.Settings then
			profile.Settings.ReducedMotion = reducedMotionEnabled()
			refreshShop(profile)
		end
	end)

	if RunService:IsStudio() then
		local function applyQaView()
			local view = LOCAL_PLAYER:GetAttribute("RisingStepsQaView")
			if view == "Shop" then
				setShopVisible(true)
			elseif view == "Gameplay" or view == "Result" then
				setShopVisible(false)
			end
		end
		LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsQaView"):Connect(applyQaView)
		applyQaView()
	end

	task.spawn(function()
		local ok, bootstrap = pcall(function()
			return requestBootstrap:InvokeServer()
		end)
		if ok and type(bootstrap) == "table" then
			refreshShop(bootstrap)
			tutorial.Visible = bootstrap.TutorialComplete ~= true
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
