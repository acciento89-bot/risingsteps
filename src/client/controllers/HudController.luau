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
local ViewportRules = require(root.rules.ViewportRules)

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

	local logo = Instance.new("Frame")
	logo.Name = "RisingStepsLogo"
	logo.Position = UDim2.fromOffset(18, 10)
	logo.Size = UDim2.fromOffset(224, 80)
	logo.BackgroundTransparency = 1
	logo.Parent = gui

	local risingWord = makeLabel(
		logo,
		"RisingWord",
		"RISING",
		UDim2.fromOffset(142, 34),
		UDim2.fromOffset(0, 0),
		Enum.Font.GothamBlack,
		28,
		COLORS.Warm
	)
	risingWord.TextStrokeColor3 = Color3.fromRGB(71, 50, 105)
	risingWord.TextStrokeTransparency = 0.34

	local stepsWord = makeLabel(
		logo,
		"StepsWord",
		"STEPS",
		UDim2.fromOffset(152, 34),
		UDim2.fromOffset(16, 26),
		Enum.Font.GothamBlack,
		30,
		Color3.fromRGB(235, 247, 255)
	)
	stepsWord.TextStrokeColor3 = Color3.fromRGB(84, 70, 151)
	stepsWord.TextStrokeTransparency = 0.28

	local logoTagline = makeLabel(
		logo,
		"Tagline",
		"JUMP HIGHER • DISCOVER NEW SKIES",
		UDim2.fromOffset(224, 18),
		UDim2.fromOffset(2, 62),
		Enum.Font.GothamBold,
		9,
		COLORS.Muted
	)
	logoTagline.TextXAlignment = Enum.TextXAlignment.Left

	local stageProgress = Instance.new("Frame")
	stageProgress.Name = "StageProgress"
	stageProgress.AnchorPoint = Vector2.new(0.5, 0)
	stageProgress.Position = UDim2.new(0.5, 0, 0, 12)
	stageProgress.Size = UDim2.fromOffset(304, 56)
	stageProgress.BackgroundColor3 = COLORS.Foundation
	stageProgress.BackgroundTransparency = 0.03
	stageProgress.BorderSizePixel = 0
	stageProgress.Parent = gui
	addCorner(stageProgress, 17)
	addStroke(stageProgress, Color3.fromRGB(199, 177, 255), 0.34, 1.4)
	addGradient(stageProgress, Color3.fromRGB(61, 48, 101), COLORS.Foundation, 115)

	local stageProgressText = makeLabel(
		stageProgress,
		"ProgressText",
		"HEIGHT 0 / 12",
		UDim2.new(1, -24, 0, 26),
		UDim2.fromOffset(12, 4),
		Enum.Font.GothamBold,
		12,
		COLORS.Text
	)
	stageProgressText.TextXAlignment = Enum.TextXAlignment.Center

	local progressTrack = Instance.new("Frame")
	progressTrack.Name = "ProgressTrack"
	progressTrack.Position = UDim2.fromOffset(12, 34)
	progressTrack.Size = UDim2.new(1, -24, 0, 12)
	progressTrack.BackgroundColor3 = Color3.fromRGB(22, 20, 43)
	progressTrack.BackgroundTransparency = 0.06
	progressTrack.BorderSizePixel = 0
	progressTrack.ClipsDescendants = true
	progressTrack.Parent = stageProgress
	addCorner(progressTrack, 99)

	local progressFill = Instance.new("Frame")
	progressFill.Name = "ProgressFill"
	progressFill.Size = UDim2.fromScale(0, 1)
	progressFill.BackgroundColor3 = Color3.fromRGB(103, 232, 159)
	progressFill.BorderSizePixel = 0
	progressFill.Parent = progressTrack
	addCorner(progressFill, 99)
	addGradient(progressFill, Color3.fromRGB(115, 242, 183), Color3.fromRGB(91, 220, 239), 0)

	local metrics = Instance.new("Frame")
	metrics.Name = "Metrics"
	metrics.Position = UDim2.fromOffset(18, 98)
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
	streak.Position = UDim2.fromOffset(18, 194)
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
	topRight.Position = UDim2.new(1, -18, 0, 18)
	topRight.Size = UDim2.fromOffset(106, 48)
	topRight.BackgroundTransparency = 1
	topRight.Parent = gui

	local coinsPill = Instance.new("Frame")
	coinsPill.Name = "CoinsPill"
	coinsPill.Size = UDim2.fromOffset(106, 48)
	coinsPill.BackgroundColor3 = COLORS.Foundation
	coinsPill.BackgroundTransparency = 0.04
	coinsPill.BorderSizePixel = 0
	coinsPill.Parent = topRight
	addCorner(coinsPill, 15)
	addStroke(coinsPill, COLORS.Warm, 0.42, 1.2)
	addGradient(coinsPill, Color3.fromRGB(61, 48, 101), COLORS.Foundation, 115)

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

	local quickActions = Instance.new("Frame")
	quickActions.Name = "QuickActions"
	quickActions.Position = UDim2.fromOffset(18, 246)
	quickActions.Size = UDim2.fromOffset(92, 148)
	quickActions.BackgroundTransparency = 1
	quickActions.Parent = gui

	local shopButton = makeButton(
		quickActions,
		"ShopButton",
		"SHOP",
		UDim2.fromOffset(88, 42),
		UDim2.fromOffset(0, 0),
		Color3.fromRGB(145, 103, 255),
		COLORS.Text
	)
	local dailyQuickButton = makeButton(
		quickActions,
		"DailyQuickButton",
		"DAILY",
		UDim2.fromOffset(88, 42),
		UDim2.fromOffset(0, 50),
		Color3.fromRGB(255, 171, 92),
		COLORS.Foundation
	)
	local styleQuickButton = makeButton(
		quickActions,
		"StyleQuickButton",
		"STYLE",
		UDim2.fromOffset(88, 42),
		UDim2.fromOffset(0, 100),
		Color3.fromRGB(91, 220, 239),
		COLORS.Foundation
	)

	-- Desktop composition mirrors the approved concept sheet without turning the
	-- actual game into a static mockup. These cards are native Roblox UI and use
	-- live progression/profile state; compact and tablet layouts keep the lean HUD.
	local conceptShowcase = Instance.new("Frame")
	conceptShowcase.Name = "ConceptShowcase"
	conceptShowcase.Size = UDim2.fromScale(1, 1)
	conceptShowcase.BackgroundTransparency = 1
	conceptShowcase.Visible = false
	conceptShowcase.Parent = gui

	local shopShowcase = Instance.new("Frame")
	shopShowcase.Name = "ConceptShopShowcase"
	shopShowcase.AnchorPoint = Vector2.new(0, 1)
	shopShowcase.Position = UDim2.new(0, 18, 1, -18)
	shopShowcase.Size = UDim2.fromOffset(308, 214)
	shopShowcase.BackgroundColor3 = Color3.fromRGB(52, 42, 91)
	shopShowcase.BackgroundTransparency = 0.04
	shopShowcase.BorderSizePixel = 0
	shopShowcase.Parent = conceptShowcase
	addCorner(shopShowcase, 22)
	addStroke(shopShowcase, Color3.fromRGB(215, 197, 255), 0.28, 1.5)
	addGradient(shopShowcase, Color3.fromRGB(97, 72, 151), Color3.fromRGB(42, 37, 78), 115)

	local shopShowcaseTitle = makeLabel(
		shopShowcase,
		"Title",
		"SKY SHOP",
		UDim2.fromOffset(156, 28),
		UDim2.fromOffset(16, 10),
		Enum.Font.GothamBlack,
		18,
		COLORS.Text
	)
	shopShowcaseTitle.TextStrokeColor3 = Color3.fromRGB(92, 66, 137)
	shopShowcaseTitle.TextStrokeTransparency = 0.42

	local shopShowcaseHint = makeLabel(
		shopShowcase,
		"Hint",
		"FEATURED",
		UDim2.fromOffset(92, 22),
		UDim2.fromOffset(198, 13),
		Enum.Font.GothamBold,
		10,
		Color3.fromRGB(224, 214, 255)
	)
	shopShowcaseHint.TextXAlignment = Enum.TextXAlignment.Right

	local conceptShopButtons: { TextButton } = {}
	local showcaseItems = {
		{ Category = "Trails", ItemId = "ion" },
		{ Category = "LandingEffects", ItemId = "pulse" },
		{ Category = "StepThemes", ItemId = "forge" },
	}
	for index, reference in showcaseItems do
		local categoryConfig = CosmeticConfig.Categories[reference.Category]
		local item = categoryConfig.Items[reference.ItemId]
		local card = Instance.new("Frame")
		card.Name = ("Featured%02d"):format(index)
		card.Position = UDim2.fromOffset(14 + (index - 1) * 98, 48)
		card.Size = UDim2.fromOffset(88, 150)
		card.BackgroundColor3 = Color3.fromRGB(62, 52, 108)
		card.BackgroundTransparency = 0.02
		card.BorderSizePixel = 0
		card.Parent = shopShowcase
		addCorner(card, 15)
		addStroke(card, Color3.fromRGB(202, 185, 255), 0.48, 1)
		addGradient(card, Color3.fromRGB(81, 65, 134), Color3.fromRGB(45, 40, 84), 90)

		local preview = Instance.new("Frame")
		preview.Name = "Preview"
		preview.Position = UDim2.fromOffset(8, 8)
		preview.Size = UDim2.fromOffset(72, 58)
		preview.BackgroundColor3 = PREVIEW_COLORS[reference.ItemId] or COLORS.Active
		preview.BorderSizePixel = 0
		preview.Parent = card
		addCorner(preview, 12)
		addStroke(preview, COLORS.Text, 0.68)
		makePreviewArt(preview, reference.Category, reference.ItemId)

		local title = makeLabel(
			card,
			"ItemTitle",
			item.DisplayName,
			UDim2.fromOffset(76, 36),
			UDim2.fromOffset(6, 69),
			Enum.Font.GothamBold,
			10,
			COLORS.Text
		)
		title.TextWrapped = true
		title.TextXAlignment = Enum.TextXAlignment.Center

		local price = makeLabel(
			card,
			"Price",
			if item.Price == 0 then "INCLUDED" else ("◈ %d"):format(item.Price),
			UDim2.fromOffset(76, 16),
			UDim2.fromOffset(6, 105),
			Enum.Font.GothamBold,
			10,
			COLORS.Warm
		)
		price.TextXAlignment = Enum.TextXAlignment.Center

		local openButton = makeButton(
			card,
			"OpenShop",
			"OPEN",
			UDim2.fromOffset(68, 25),
			UDim2.fromOffset(10, 122),
			Color3.fromRGB(95, 224, 130),
			Color3.fromRGB(35, 42, 55)
		)
		openButton.TextSize = 10
		table.insert(conceptShopButtons, openButton)
	end

	local rightShowcase = Instance.new("Frame")
	rightShowcase.Name = "ConceptRightColumn"
	rightShowcase.AnchorPoint = Vector2.new(1, 0)
	rightShowcase.Position = UDim2.new(1, -18, 0, 88)
	rightShowcase.Size = UDim2.fromOffset(232, 374)
	rightShowcase.BackgroundTransparency = 1
	rightShowcase.Parent = conceptShowcase

	local conceptProgressCard = Instance.new("Frame")
	conceptProgressCard.Name = "ConceptProgressCard"
	conceptProgressCard.Size = UDim2.fromOffset(232, 90)
	conceptProgressCard.BackgroundColor3 = Color3.fromRGB(56, 46, 96)
	conceptProgressCard.BackgroundTransparency = 0.04
	conceptProgressCard.BorderSizePixel = 0
	conceptProgressCard.Parent = rightShowcase
	addCorner(conceptProgressCard, 18)
	addStroke(conceptProgressCard, Color3.fromRGB(220, 205, 255), 0.34, 1.3)
	addGradient(conceptProgressCard, Color3.fromRGB(102, 78, 156), Color3.fromRGB(47, 41, 84), 115)

	makeLabel(
		conceptProgressCard,
		"Title",
		"CLEAR PROGRESS",
		UDim2.fromOffset(196, 22),
		UDim2.fromOffset(14, 8),
		Enum.Font.GothamBlack,
		13,
		COLORS.Text
	)
	local conceptProgressText = makeLabel(
		conceptProgressCard,
		"StageText",
		"STAGE 0 / 12",
		UDim2.fromOffset(198, 20),
		UDim2.fromOffset(14, 31),
		Enum.Font.GothamBold,
		10,
		COLORS.Muted
	)
	conceptProgressText.TextXAlignment = Enum.TextXAlignment.Center

	local conceptProgressTrack = Instance.new("Frame")
	conceptProgressTrack.Name = "Track"
	conceptProgressTrack.Position = UDim2.fromOffset(14, 57)
	conceptProgressTrack.Size = UDim2.fromOffset(204, 15)
	conceptProgressTrack.BackgroundColor3 = Color3.fromRGB(24, 22, 46)
	conceptProgressTrack.BorderSizePixel = 0
	conceptProgressTrack.ClipsDescendants = true
	conceptProgressTrack.Parent = conceptProgressCard
	addCorner(conceptProgressTrack, 99)
	local conceptProgressFill = Instance.new("Frame")
	conceptProgressFill.Name = "Fill"
	conceptProgressFill.Size = UDim2.fromScale(0, 1)
	conceptProgressFill.BackgroundColor3 = Color3.fromRGB(103, 232, 159)
	conceptProgressFill.BorderSizePixel = 0
	conceptProgressFill.Parent = conceptProgressTrack
	addCorner(conceptProgressFill, 99)
	addGradient(conceptProgressFill, Color3.fromRGB(113, 243, 164), Color3.fromRGB(122, 232, 255), 0)

	local conceptDailyRewards = Instance.new("Frame")
	conceptDailyRewards.Name = "ConceptDailyRewards"
	conceptDailyRewards.Position = UDim2.fromOffset(0, 100)
	conceptDailyRewards.Size = UDim2.fromOffset(232, 132)
	conceptDailyRewards.BackgroundColor3 = Color3.fromRGB(57, 47, 98)
	conceptDailyRewards.BackgroundTransparency = 0.04
	conceptDailyRewards.BorderSizePixel = 0
	conceptDailyRewards.Parent = rightShowcase
	addCorner(conceptDailyRewards, 18)
	addStroke(conceptDailyRewards, Color3.fromRGB(220, 205, 255), 0.36, 1.3)
	addGradient(conceptDailyRewards, Color3.fromRGB(99, 75, 154), Color3.fromRGB(46, 41, 83), 115)

	makeLabel(
		conceptDailyRewards,
		"Title",
		"DAILY REWARDS",
		UDim2.fromOffset(198, 22),
		UDim2.fromOffset(14, 8),
		Enum.Font.GothamBlack,
		13,
		COLORS.Text
	)
	local conceptDailyCaption = makeLabel(
		conceptDailyRewards,
		"Caption",
		"COME BACK TOMORROW",
		UDim2.fromOffset(196, 16),
		UDim2.fromOffset(14, 29),
		Enum.Font.GothamBold,
		8,
		COLORS.Muted
	)
	conceptDailyCaption.TextXAlignment = Enum.TextXAlignment.Center

	local conceptDailyTiles: { Frame } = {}
	for index = 1, 5 do
		local tile = Instance.new("Frame")
		tile.Name = ("Day%02d"):format(index)
		tile.Position = UDim2.fromOffset(9 + (index - 1) * 44, 50)
		tile.Size = UDim2.fromOffset(39, 70)
		tile.BackgroundColor3 = Color3.fromRGB(70, 59, 116)
		tile.BackgroundTransparency = 0.04
		tile.BorderSizePixel = 0
		tile.Parent = conceptDailyRewards
		addCorner(tile, 10)
		addStroke(tile, Color3.fromRGB(212, 198, 255), 0.58, 1)

		local dayLabel = makeLabel(
			tile,
			"Day",
			("D%d"):format(index),
			UDim2.fromOffset(35, 14),
			UDim2.fromOffset(2, 5),
			Enum.Font.GothamBold,
			8,
			COLORS.Muted
		)
		dayLabel.TextXAlignment = Enum.TextXAlignment.Center
		local rewardLabel = makeLabel(
			tile,
			"Reward",
			if index == 1
				then "✓"
				elseif index == 2 then "◈"
				elseif index == 3 then "✦"
				elseif index == 4 then "♢"
				else "★",
			UDim2.fromOffset(35, 34),
			UDim2.fromOffset(2, 22),
			Enum.Font.GothamBlack,
			16,
			if index == 1 then Color3.fromRGB(103, 232, 159) else COLORS.Warm
		)
		rewardLabel.TextXAlignment = Enum.TextXAlignment.Center
		table.insert(conceptDailyTiles, tile)
	end

	local conceptVistaCard = Instance.new("Frame")
	conceptVistaCard.Name = "ConceptVistaCard"
	conceptVistaCard.Position = UDim2.fromOffset(0, 242)
	conceptVistaCard.Size = UDim2.fromOffset(232, 132)
	conceptVistaCard.BackgroundColor3 = Color3.fromRGB(57, 47, 98)
	conceptVistaCard.BackgroundTransparency = 0.04
	conceptVistaCard.BorderSizePixel = 0
	conceptVistaCard.Parent = rightShowcase
	addCorner(conceptVistaCard, 18)
	addStroke(conceptVistaCard, Color3.fromRGB(220, 205, 255), 0.36, 1.3)
	addGradient(conceptVistaCard, Color3.fromRGB(100, 76, 154), Color3.fromRGB(45, 40, 82), 115)

	makeLabel(
		conceptVistaCard,
		"Title",
		"BETTER VISUALS",
		UDim2.fromOffset(198, 22),
		UDim2.fromOffset(14, 7),
		Enum.Font.GothamBlack,
		13,
		COLORS.Text
	)

	local conceptViewport = Instance.new("ViewportFrame")
	conceptViewport.Name = "SkyPreview"
	conceptViewport.Position = UDim2.fromOffset(12, 32)
	conceptViewport.Size = UDim2.fromOffset(208, 88)
	conceptViewport.BackgroundColor3 = Color3.fromRGB(190, 151, 235)
	conceptViewport.BackgroundTransparency = 0
	conceptViewport.BorderSizePixel = 0
	conceptViewport.Ambient = Color3.fromRGB(190, 170, 235)
	conceptViewport.LightColor = Color3.fromRGB(255, 223, 185)
	conceptViewport.LightDirection = Vector3.new(-1, -1, -0.5)
	conceptViewport.Parent = conceptVistaCard
	addCorner(conceptViewport, 13)

	local conceptWorld = Instance.new("WorldModel")
	conceptWorld.Name = "PreviewWorld"
	conceptWorld.Parent = conceptViewport
	local conceptCamera = Instance.new("Camera")
	conceptCamera.Name = "PreviewCamera"
	conceptCamera.CFrame = CFrame.lookAt(Vector3.new(11, 7, 13), Vector3.new(0, 1, 0))
	conceptCamera.FieldOfView = 40
	conceptCamera.Parent = conceptViewport
	conceptViewport.CurrentCamera = conceptCamera

	local function previewPart(
		name: string,
		size: Vector3,
		cframe: CFrame,
		material: Enum.Material,
		color: Color3,
		shape: Enum.PartType?
	): Part
		local part = Instance.new("Part")
		part.Name = name
		part.Size = size
		part.CFrame = cframe
		part.Anchored = true
		part.CanCollide = false
		part.Material = material
		part.Color = color
		part.Shape = shape or Enum.PartType.Block
		part.Parent = conceptWorld
		return part
	end

	previewPart(
		"IslandRock",
		Vector3.new(8, 3.8, 7),
		CFrame.new(0, -1, 0),
		Enum.Material.Rock,
		Color3.fromRGB(111, 82, 124),
		Enum.PartType.Ball
	)
	previewPart(
		"IslandGrass",
		Vector3.new(7.2, 0.55, 6.6),
		CFrame.new(0, 1.05, 0),
		Enum.Material.Grass,
		Color3.fromRGB(121, 166, 91)
	)
	previewPart(
		"PortalLeft",
		Vector3.new(0.45, 4.4, 0.45),
		CFrame.new(-1.8, 3.3, -0.6),
		Enum.Material.Neon,
		Color3.fromRGB(246, 188, 255)
	)
	previewPart(
		"PortalRight",
		Vector3.new(0.45, 4.4, 0.45),
		CFrame.new(1.8, 3.3, -0.6),
		Enum.Material.Neon,
		Color3.fromRGB(246, 188, 255)
	)
	previewPart(
		"PortalTop",
		Vector3.new(4.05, 0.45, 0.45),
		CFrame.new(0, 5.3, -0.6),
		Enum.Material.Neon,
		Color3.fromRGB(255, 215, 125)
	)
	previewPart(
		"Waterfall",
		Vector3.new(1.0, 5.2, 0.22),
		CFrame.new(3.15, -1.0, 0.2),
		Enum.Material.Neon,
		Color3.fromRGB(113, 229, 255)
	)

	local dailyDimmer = Instance.new("Frame")
	dailyDimmer.Name = "DailyDimmer"
	dailyDimmer.Size = UDim2.fromScale(1, 1)
	dailyDimmer.BackgroundColor3 = COLORS.Dim
	dailyDimmer.BackgroundTransparency = 0.34
	dailyDimmer.BorderSizePixel = 0
	dailyDimmer.Visible = false
	dailyDimmer.ZIndex = 60
	dailyDimmer.Parent = gui

	local dailyPanel = Instance.new("Frame")
	dailyPanel.Name = "DailyRewardsPanel"
	dailyPanel.AnchorPoint = Vector2.new(0.5, 0.5)
	dailyPanel.Position = UDim2.fromScale(0.5, 0.5)
	dailyPanel.Size = UDim2.fromOffset(430, 292)
	dailyPanel.BackgroundColor3 = Color3.fromRGB(55, 45, 96)
	dailyPanel.BackgroundTransparency = 0.02
	dailyPanel.BorderSizePixel = 0
	dailyPanel.Visible = false
	dailyPanel.ZIndex = 61
	dailyPanel.Parent = gui
	addCorner(dailyPanel, 22)
	addStroke(dailyPanel, Color3.fromRGB(220, 205, 255), 0.28, 1.5)
	addGradient(dailyPanel, Color3.fromRGB(105, 81, 164), Color3.fromRGB(44, 39, 82), 115)

	local dailyModalTitle = makeLabel(
		dailyPanel,
		"Title",
		"DAILY REWARDS",
		UDim2.fromOffset(300, 34),
		UDim2.fromOffset(22, 14),
		Enum.Font.GothamBlack,
		22,
		COLORS.Text
	)
	dailyModalTitle.ZIndex = 62

	local dailyCloseButton = makeButton(
		dailyPanel,
		"CloseButton",
		"×",
		UDim2.fromOffset(44, 44),
		UDim2.new(1, -58, 0, 12),
		Color3.fromRGB(85, 69, 132),
		COLORS.Text
	)
	dailyCloseButton.TextSize = 24
	dailyCloseButton.ZIndex = 62

	local dailyModalStatus = makeLabel(
		dailyPanel,
		"Status",
		"COME BACK EVERY DAY FOR A SKY REWARD",
		UDim2.new(1, -44, 0, 22),
		UDim2.fromOffset(22, 52),
		Enum.Font.GothamBold,
		11,
		COLORS.Muted
	)
	dailyModalStatus.TextXAlignment = Enum.TextXAlignment.Center
	dailyModalStatus.ZIndex = 62

	local dailyModalTiles: { Frame } = {}
	for index = 1, 5 do
		local tile = Instance.new("Frame")
		tile.Name = ("Day%02d"):format(index)
		tile.Position = UDim2.fromOffset(18 + (index - 1) * 80, 88)
		tile.Size = UDim2.fromOffset(72, 110)
		tile.BackgroundColor3 = Color3.fromRGB(72, 59, 119)
		tile.BorderSizePixel = 0
		tile.ZIndex = 62
		tile.Parent = dailyPanel
		addCorner(tile, 14)
		addStroke(tile, Color3.fromRGB(210, 194, 255), 0.54, 1)

		local day = makeLabel(
			tile,
			"Day",
			("DAY %d"):format(index),
			UDim2.fromOffset(64, 18),
			UDim2.fromOffset(4, 7),
			Enum.Font.GothamBold,
			9,
			COLORS.Muted
		)
		day.TextXAlignment = Enum.TextXAlignment.Center
		day.ZIndex = 63
		local icon = makeLabel(
			tile,
			"RewardIcon",
			if index == 1
				then "✓"
				elseif index == 2 then "◈"
				elseif index == 3 then "✦"
				elseif index == 4 then "♢"
				else "★",
			UDim2.fromOffset(64, 44),
			UDim2.fromOffset(4, 30),
			Enum.Font.GothamBlack,
			24,
			if index == 1 then Color3.fromRGB(103, 232, 159) else COLORS.Warm
		)
		icon.TextXAlignment = Enum.TextXAlignment.Center
		icon.ZIndex = 63
		local reward = makeLabel(
			tile,
			"Reward",
			if index == 1
				then "TODAY"
				elseif index == 2 then "+250"
				elseif index == 3 then "BONUS"
				elseif index == 4 then "STYLE"
				else "RARE",
			UDim2.fromOffset(64, 20),
			UDim2.fromOffset(4, 82),
			Enum.Font.GothamBold,
			9,
			COLORS.Text
		)
		reward.TextXAlignment = Enum.TextXAlignment.Center
		reward.ZIndex = 63
		table.insert(dailyModalTiles, tile)
	end

	local dailyModalButton = makeButton(
		dailyPanel,
		"ClaimButton",
		"CLAIM REWARD",
		UDim2.fromOffset(180, 46),
		UDim2.new(0.5, -90, 1, -64),
		Color3.fromRGB(95, 224, 130),
		Color3.fromRGB(35, 42, 55)
	)
	dailyModalButton.ZIndex = 62

	local tutorial = Instance.new("Frame")
	tutorial.Name = "Tutorial"
	tutorial.AnchorPoint = Vector2.new(0.5, 0)
	tutorial.Position = UDim2.new(0.5, 0, 0, 82)
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
		"LAND ANYWHERE  ↑",
		UDim2.new(1, -28, 0, 26),
		UDim2.fromOffset(14, 4),
		Enum.Font.GothamBlack,
		19,
		COLORS.Text
	)
	tutorialTitle.TextXAlignment = Enum.TextXAlignment.Center

	local tutorialBody = makeLabel(
		tutorial,
		"Body",
		"Any part of the next platform counts. The glowing center is BONUS precision — not required.",
		UDim2.new(1, -28, 0, 32),
		UDim2.fromOffset(14, 30),
		Enum.Font.GothamMedium,
		11,
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
		local viewportClass = ViewportRules.classify(width)
		scale.Scale = ViewportRules.uiScale(width)

		local rightInset, topOffset = ViewportRules.topRightHudOffsets(width, UserInputService.TouchEnabled)
		conceptShowcase.Visible = false
		metrics.Visible = viewportClass == "compact"
		streak.Visible = viewportClass == "compact"
		if viewportClass ~= "compact" then
			topRight.Position = UDim2.new(0.80, 0, 0, 18)
		else
			topRight.Position = UDim2.new(1, -rightInset, 0, topOffset)
		end

		if viewportClass == "compact" then
			logo.Visible = false
			quickActions.Visible = false
			metrics.Position = UDim2.fromOffset(12, 12)
			streak.Position = UDim2.fromOffset(12, 108)
			stageProgress.Size = UDim2.fromOffset(228, 52)
			stageProgress.Position = UDim2.new(0.58, 0, 0, 12)
			tutorial.Position = UDim2.new(0.5, 0, 0, 164)
			tutorial.Size = UDim2.new(1, -28, 0, 94)
		elseif viewportClass == "tablet" then
			logo.Visible = true
			quickActions.Visible = true
			logo.Position = UDim2.fromOffset(16, 10)
			metrics.Position = UDim2.fromOffset(16, 98)
			streak.Position = UDim2.fromOffset(16, 194)
			quickActions.Position = UDim2.fromOffset(326, 224)
			stageProgress.Size = UDim2.fromOffset(330, 58)
			stageProgress.Position = UDim2.new(0.5, 0, 0, 12)
			tutorial.Position = UDim2.new(0.5, 0, 1, -76)
			tutorial.Size = UDim2.fromOffset(350, 68)
		else
			logo.Visible = true
			quickActions.Visible = true
			logo.Position = UDim2.fromOffset(18, 10)
			metrics.Position = UDim2.fromOffset(18, 98)
			streak.Position = UDim2.fromOffset(18, 194)
			quickActions.Position = UDim2.fromOffset(326, 224)
			stageProgress.Size = UDim2.fromOffset(330, 58)
			stageProgress.Position = UDim2.new(0.5, 0, 0, 12)
			tutorial.Position = UDim2.new(0.5, 0, 0, 82)
			tutorial.Size = UDim2.fromOffset(350, 68)
		end
	end

	local function updateMetrics(height: number, streakCount: number, pb: number)
		heightValue.Text = tostring(height)
		pbValue.Text = tostring(pb)
		streakValue.Text = ("STREAK  %d"):format(streakCount)
		local coins = LOCAL_PLAYER:GetAttribute("RisingStepsCoins")
		coinsLabel.Text = ("◈ %d"):format(if type(coins) == "number" then coins else 0)

		local milestoneStart = 0
		local milestoneTarget = 12
		if height >= 30 then
			milestoneStart = 30 + math.floor((height - 30) / 10) * 10
			milestoneTarget = milestoneStart + 10
		elseif height >= 12 then
			milestoneStart = 12
			milestoneTarget = 30
		end
		local progress = math.clamp((height - milestoneStart) / math.max(1, milestoneTarget - milestoneStart), 0, 1)
		stageProgressText.Text = ("HEIGHT %d / %d"):format(height, milestoneTarget)
		TweenService:Create(progressFill, TweenInfo.new(0.24, Enum.EasingStyle.Quint), {
			Size = UDim2.fromScale(progress, 1),
		}):Play()
		conceptProgressText.Text = ("STAGE %d / %d"):format(height, milestoneTarget)
		TweenService:Create(conceptProgressFill, TweenInfo.new(0.24, Enum.EasingStyle.Quint), {
			Size = UDim2.fromScale(progress, 1),
		}):Play()
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
			gradeLabel.TextColor3 = COLORS.Active
		end

		gradeLabel.Text = if grade == "Miss" then "SAFE LANDING" else string.upper(grade)
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
			local dailyStreak = if type(daily.ClaimStreak) == "number" then daily.ClaimStreak else 0
			dailyDetail.Text = ("Daily Coins  •  streak %d"):format(dailyStreak)
			conceptDailyCaption.Text = if daily.Claimable == true
				then "READY TO CLAIM"
				else ("STREAK %d DAYS"):format(dailyStreak)
			dailyModalButton.Active = daily.Claimable == true
			dailyModalButton.Text = if daily.Claimable == true then "CLAIM REWARD" else "CLAIMED"
			dailyModalButton.BackgroundColor3 = if daily.Claimable == true
				then Color3.fromRGB(95, 224, 130)
				else Color3.fromRGB(92, 91, 119)
			dailyModalStatus.Text = if daily.Claimable == true
				then ("DAY %d IS READY"):format(math.max(1, dailyStreak + 1))
				else ("STREAK %d • COME BACK TOMORROW"):format(dailyStreak)
			for index, tile in conceptDailyTiles do
				local claimed = index <= math.clamp(dailyStreak, 0, 5)
				local current = daily.Claimable == true and index == math.clamp(dailyStreak + 1, 1, 5)
				tile.BackgroundColor3 = if claimed
					then Color3.fromRGB(66, 126, 101)
					elseif current then Color3.fromRGB(125, 86, 166)
					else Color3.fromRGB(70, 59, 116)
			end
			for index, tile in dailyModalTiles do
				local claimed = index <= math.clamp(dailyStreak, 0, 5)
				local current = daily.Claimable == true and index == math.clamp(dailyStreak + 1, 1, 5)
				tile.BackgroundColor3 = if claimed
					then Color3.fromRGB(66, 126, 101)
					elseif current then Color3.fromRGB(125, 86, 166)
					else Color3.fromRGB(72, 59, 119)
			end
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

	local function setDailyVisible(visible: boolean)
		dailyPanel.Visible = visible
		dailyDimmer.Visible = visible
		if visible then
			setShopVisible(false)
			refreshShop(profile)
			if UserInputService.GamepadEnabled then
				GuiService.SelectedObject = dailyCloseButton
			end
		elseif GuiService.SelectedObject and GuiService.SelectedObject:IsDescendantOf(dailyPanel) then
			GuiService.SelectedObject = dailyQuickButton
		end
	end

	for _, conceptButton in conceptShopButtons do
		conceptButton.Activated:Connect(function()
			setShopVisible(true)
		end)
	end

	shopButton.Activated:Connect(function()
		setDailyVisible(false)
		setShopVisible(true)
	end)
	dailyQuickButton.Activated:Connect(function()
		setDailyVisible(true)
	end)
	styleQuickButton.Activated:Connect(function()
		setDailyVisible(false)
		setShopVisible(true)
		task.defer(function()
			scroll.CanvasPosition = Vector2.new(0, 360)
		end)
	end)
	closeButton.Activated:Connect(function()
		setShopVisible(false)
	end)
	dailyCloseButton.Activated:Connect(function()
		setDailyVisible(false)
	end)
	dailyDimmer.InputBegan:Connect(function(input)
		if input.UserInputType == Enum.UserInputType.MouseButton1 then
			setDailyVisible(false)
		end
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
	dailyModalButton.Activated:Connect(function()
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
			if view == "Shop" or view == "ShopCosmetics" then
				setShopVisible(true)
				tutorial.Visible = false
				warning.Visible = false
				result.Visible = false
				task.spawn(function()
					local ok, bootstrap = pcall(function()
						return requestBootstrap:InvokeServer()
					end)
					if ok and type(bootstrap) == "table" then
						refreshShop(bootstrap)
					end
					task.wait(0.15)
					if view == "ShopCosmetics" then
						scroll.CanvasPosition = Vector2.new(0, 620)
					else
						scroll.CanvasPosition = Vector2.zero
					end
				end)
			elseif view == "Tutorial" then
				setShopVisible(false)
				result.Visible = false
				warning.Visible = false
				tutorial.Visible = true
			elseif view == "Warning" then
				setShopVisible(false)
				result.Visible = false
				tutorial.Visible = false
				warning.Visible = true
				warningText.TextTransparency = 0
				warning.BackgroundTransparency = 0.05
			elseif view == "Result" then
				setShopVisible(false)
				tutorial.Visible = false
				warning.Visible = false
				result.Visible = true
			elseif view == "Gameplay" then
				setShopVisible(false)
				tutorial.Visible = false
				warning.Visible = false
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
