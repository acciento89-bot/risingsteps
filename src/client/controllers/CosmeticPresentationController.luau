--!strict

local Players = game:GetService("Players")
local Workspace = game:GetService("Workspace")

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local root = ReplicatedStorage:WaitForChild("RisingSteps")
local WorldConfig = require(root.config.WorldConfig)

local CosmeticPresentationController = {}

local LOCAL_PLAYER = Players.LocalPlayer

local STEP_THEMES = {
	steel = {
		Collision = Color3.fromRGB(112, 94, 126),
		Inset = Color3.fromRGB(118, 158, 92),
		Accent = Color3.fromRGB(255, 211, 106),
		Detail = Color3.fromRGB(245, 226, 190),
	},
	forge = {
		Collision = Color3.fromRGB(145, 94, 91),
		Inset = Color3.fromRGB(169, 133, 88),
		Accent = Color3.fromRGB(255, 180, 92),
		Detail = Color3.fromRGB(255, 224, 151),
	},
	frost = {
		Collision = Color3.fromRGB(116, 142, 169),
		Inset = Color3.fromRGB(150, 180, 181),
		Accent = Color3.fromRGB(128, 237, 255),
		Detail = Color3.fromRGB(221, 251, 255),
	},
}

local ENVIRONMENT_THEMES = {
	default = {
		Foundation = Color3.fromRGB(18, 23, 34),
		Steel = Color3.fromRGB(32, 41, 56),
		Accent = Color3.fromRGB(94, 235, 255),
		Warm = Color3.fromRGB(255, 180, 84),
	},
	dusk = {
		Foundation = Color3.fromRGB(24, 18, 31),
		Steel = Color3.fromRGB(55, 39, 67),
		Accent = Color3.fromRGB(211, 128, 255),
		Warm = Color3.fromRGB(255, 151, 93),
	},
	storm = {
		Foundation = Color3.fromRGB(15, 22, 31),
		Steel = Color3.fromRGB(38, 52, 68),
		Accent = Color3.fromRGB(126, 197, 255),
		Warm = Color3.fromRGB(220, 232, 255),
	},
}

local TRAIL_THEMES = {
	none = nil,
	ion = {
		Color = ColorSequence.new(Color3.fromRGB(94, 235, 255), Color3.fromRGB(193, 251, 255)),
		Transparency = NumberSequence.new({
			NumberSequenceKeypoint.new(0, 0.12),
			NumberSequenceKeypoint.new(1, 1),
		}),
		Width = 0.28,
	},
	ember = {
		Color = ColorSequence.new(Color3.fromRGB(255, 146, 77), Color3.fromRGB(255, 212, 118)),
		Transparency = NumberSequence.new({
			NumberSequenceKeypoint.new(0, 0.08),
			NumberSequenceKeypoint.new(1, 1),
		}),
		Width = 0.34,
	},
}

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

local function recolorStepModel(step: Model, theme: any)
	local collision = step:FindFirstChild("Collision")
	if collision and collision:IsA("BasePart") then
		collision.Color = theme.Collision
	end
	local inset = step:FindFirstChild("LandingInset")
	if inset and inset:IsA("BasePart") then
		inset.Color = theme.Inset
	end
	for _, descendant in step:GetDescendants() do
		if descendant:IsA("BasePart") then
			if
				descendant.Name == "AccentA"
				or descendant.Name == "AccentB"
				or descendant.Name == "AccentC"
				or descendant.Name == "AccentD"
				or descendant.Name == "UnderGlow"
				or descendant.Name == "LandingMark"
				or descendant.Name == "BonusCore"
			then
				descendant.Color = theme.Accent
			elseif descendant.Name == "DeckBolt" or descendant.Name == "WarmMarker" then
				descendant.Color = theme.Detail
			end
		end
	end
end

local function applyStepTheme()
	local course = currentCourse()
	if not course then
		return
	end
	local steps = course:FindFirstChild("Steps")
	if not steps then
		return
	end

	local themeId = LOCAL_PLAYER:GetAttribute("RisingStepsStepTheme")
	local theme = STEP_THEMES[if type(themeId) == "string" then themeId else "steel"] or STEP_THEMES.steel
	for _, child in steps:GetChildren() do
		if child:IsA("Model") then
			recolorStepModel(child, theme)
		end
	end
end

local function classifyEnvironmentPart(part: BasePart, theme: any)
	local baseColor = part:GetAttribute("RisingStepsBaseColor")
	if typeof(baseColor) ~= "Color3" then
		baseColor = part.Color
		part:SetAttribute("RisingStepsBaseColor", baseColor)
	end
	part.Color = baseColor

	local name = part.Name
	if string.find(name, "Beacon") or string.find(name, "Lamp") or string.find(name, "Guide") then
		part.Color = theme.Accent
	elseif string.find(name, "Window") or string.find(name, "Practical") then
		part.Color = theme.Warm
	elseif
		string.find(name, "Spine")
		or string.find(name, "Mast")
		or string.find(name, "Arm")
		or string.find(name, "Rib")
	then
		part.Color = theme.Steel
	elseif part.Material == Enum.Material.Metal or part.Material == Enum.Material.Concrete then
		part.Color = (baseColor :: Color3):Lerp(theme.Foundation, 0.46)
	end
end

local function applyEnvironmentTheme()
	local course = currentCourse()
	if not course then
		return
	end
	local environment = course:FindFirstChild("EnvironmentArt")
	if not environment then
		return
	end

	local themeId = LOCAL_PLAYER:GetAttribute("RisingStepsEnvironmentTheme")
	local theme = ENVIRONMENT_THEMES[if type(themeId) == "string" then themeId else "default"]
		or ENVIRONMENT_THEMES.default
	for _, descendant in environment:GetDescendants() do
		if descendant:IsA("BasePart") then
			classifyEnvironmentPart(descendant, theme)
		elseif descendant:IsA("Beam") then
			descendant.Color = ColorSequence.new(theme.Accent)
		elseif descendant:IsA("PointLight") then
			descendant.Color = theme.Accent
		end
	end
end

local function clearTrail(character: Model)
	for _, descendant in character:GetDescendants() do
		if
			descendant.Name == "RisingStepsTrail"
			or descendant.Name == "RisingStepsTrailA"
			or descendant.Name == "RisingStepsTrailB"
		then
			descendant:Destroy()
		end
	end
end

local function applyTrail(character: Model)
	clearTrail(character)

	local trailId = LOCAL_PLAYER:GetAttribute("RisingStepsTrail")
	local definition = TRAIL_THEMES[if type(trailId) == "string" then trailId else "none"]
	if not definition then
		return
	end

	local rootPart = character:FindFirstChild("HumanoidRootPart")
	if not rootPart or not rootPart:IsA("BasePart") then
		return
	end

	local a = Instance.new("Attachment")
	a.Name = "RisingStepsTrailA"
	a.Position = Vector3.new(-0.55, 0.1, 0.45)
	a.Parent = rootPart

	local b = Instance.new("Attachment")
	b.Name = "RisingStepsTrailB"
	b.Position = Vector3.new(0.55, 0.1, 0.45)
	b.Parent = rootPart

	local trail = Instance.new("Trail")
	trail.Name = "RisingStepsTrail"
	trail.Attachment0 = a
	trail.Attachment1 = b
	trail.FaceCamera = true
	trail.Lifetime = 0.28
	trail.MinLength = 0.12
	trail.Color = definition.Color
	trail.Transparency = definition.Transparency
	trail.WidthScale = NumberSequence.new({
		NumberSequenceKeypoint.new(0, definition.Width),
		NumberSequenceKeypoint.new(1, 0),
	})
	trail.LightEmission = 0.55
	trail.Parent = rootPart
end

local function refreshAll()
	task.defer(function()
		applyStepTheme()
		applyEnvironmentTheme()
		local character = LOCAL_PLAYER.Character
		if character then
			applyTrail(character)
		end
	end)
end

function CosmeticPresentationController.start()
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsStepTheme"):Connect(applyStepTheme)
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsEnvironmentTheme"):Connect(applyEnvironmentTheme)
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsTrail"):Connect(function()
		local character = LOCAL_PLAYER.Character
		if character then
			applyTrail(character)
		end
	end)
	LOCAL_PLAYER:GetAttributeChangedSignal("RisingStepsHeight"):Connect(function()
		local height = LOCAL_PLAYER:GetAttribute("RisingStepsHeight")
		if height == 0 then
			task.delay(0.08, applyStepTheme)
		end
	end)

	LOCAL_PLAYER.CharacterAdded:Connect(function(character)
		task.delay(0.25, function()
			if character.Parent then
				applyTrail(character)
			end
		end)
	end)

	Workspace.ChildAdded:Connect(function(child)
		if child.Name == WorldConfig.WorldFolderName then
			task.delay(0.15, refreshAll)
		end
	end)

	task.delay(0.3, refreshAll)
end

return CosmeticPresentationController
