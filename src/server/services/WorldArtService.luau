--!strict

local Lighting = game:GetService("Lighting")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local ArtConfig = require(root.config.ArtConfig)

local WorldArtService = {}
local lightingConfigured = false

local COLORS = {
	Rock = Color3.fromRGB(91, 78, 112),
	RockLight = Color3.fromRGB(125, 105, 139),
	Grass = Color3.fromRGB(116, 158, 91),
	GrassLight = Color3.fromRGB(154, 190, 108),
	Wood = Color3.fromRGB(106, 71, 55),
	Leaves = Color3.fromRGB(92, 152, 92),
	LeavesLight = Color3.fromRGB(133, 184, 105),
	Gold = Color3.fromRGB(255, 211, 106),
	Cyan = Color3.fromRGB(116, 232, 255),
	Lavender = Color3.fromRGB(200, 178, 255),
	Water = Color3.fromRGB(111, 220, 255),
	Cloud = Color3.fromRGB(246, 235, 255),
	Stone = Color3.fromRGB(160, 144, 170),
}

local function decorativePart(
	parent: Instance,
	name: string,
	size: Vector3,
	cframe: CFrame,
	material: Enum.Material,
	color: Color3,
	transparency: number?
): Part
	local item = Instance.new("Part")
	item.Name = name
	item.Size = size
	item.CFrame = cframe
	item.Anchored = true
	item.CanCollide = false
	item.CanTouch = false
	item.CanQuery = false
	item.CastShadow = true
	item.Material = material
	item.Color = color
	item.Transparency = transparency or 0
	item.TopSurface = Enum.SurfaceType.Smooth
	item.BottomSurface = Enum.SurfaceType.Smooth
	item.Parent = parent
	return item
end

local function ball(
	parent: Instance,
	name: string,
	position: Vector3,
	size: Vector3,
	material: Enum.Material,
	color: Color3,
	transparency: number?
): Part
	local item = decorativePart(parent, name, size, CFrame.new(position), material, color, transparency)
	item.Shape = Enum.PartType.Ball
	return item
end

local function disc(
	parent: Instance,
	name: string,
	position: Vector3,
	diameter: number,
	height: number,
	material: Enum.Material,
	color: Color3,
	transparency: number?
): Part
	local item = decorativePart(
		parent,
		name,
		Vector3.new(height, diameter, diameter),
		CFrame.new(position) * CFrame.Angles(0, 0, math.rad(90)),
		material,
		color,
		transparency
	)
	item.Shape = Enum.PartType.Cylinder
	return item
end

local function rod(
	parent: Instance,
	name: string,
	fromPosition: Vector3,
	toPosition: Vector3,
	thickness: number,
	color: Color3,
	material: Enum.Material?
): Part?
	local delta = toPosition - fromPosition
	if delta.Magnitude < 0.05 then
		return nil
	end
	return decorativePart(
		parent,
		name,
		Vector3.new(thickness, thickness, delta.Magnitude),
		CFrame.lookAt((fromPosition + toPosition) / 2, toPosition),
		material or Enum.Material.Slate,
		color
	)
end

local function addGlow(parent: BasePart, color: Color3, brightness: number, range: number)
	local light = Instance.new("PointLight")
	light.Color = color
	light.Brightness = brightness
	light.Range = range
	light.Shadows = false
	light.Parent = parent
end

local function addTree(parent: Instance, position: Vector3, scale: number)
	decorativePart(
		parent,
		"SkyTreeTrunk",
		Vector3.new(0.8 * scale, 4.2 * scale, 0.8 * scale),
		CFrame.new(position + Vector3.new(0, 2.1 * scale, 0)),
		Enum.Material.Wood,
		COLORS.Wood
	)
	ball(
		parent,
		"SkyTreeCrown",
		position + Vector3.new(0, 4.7 * scale, 0),
		Vector3.new(4.2, 3.0, 4.2) * scale,
		Enum.Material.Grass,
		COLORS.Leaves
	)
	ball(
		parent,
		"SkyTreeHighlight",
		position + Vector3.new(1.1 * scale, 5.3 * scale, -0.5 * scale),
		Vector3.new(2.2, 1.7, 2.2) * scale,
		Enum.Material.Grass,
		COLORS.LeavesLight
	)
end

local function addFloatingIsland(parent: Instance, position: Vector3, radius: number, seed: number)
	local depth = radius * 0.82
	ball(
		parent,
		"FloatingRock",
		position - Vector3.new(0, depth * 0.36, 0),
		Vector3.new(radius * 1.9, depth, radius * 1.65),
		Enum.Material.Slate,
		if seed % 2 == 0 then COLORS.Rock else COLORS.RockLight
	)
	disc(
		parent,
		"GrassCrown",
		position + Vector3.new(0, 0.24, 0),
		radius * 1.72,
		0.55,
		Enum.Material.Grass,
		COLORS.Grass
	)
	disc(
		parent,
		"MossRim",
		position - Vector3.new(0, 0.08, 0),
		radius * 1.82,
		0.25,
		Enum.Material.Ground,
		COLORS.GrassLight
	)

	for index = 1, 2 do
		local angle = (seed * 1.7 + index * 2.35) % (math.pi * 2)
		local offset = Vector3.new(
			math.cos(angle) * radius * 0.44,
			-depth * (0.58 + index * 0.13),
			math.sin(angle) * radius * 0.44
		)
		ball(
			parent,
			"HangingRock",
			position + offset,
			Vector3.new(radius * 0.55, depth * 0.55, radius * 0.5),
			Enum.Material.Rock,
			COLORS.Rock
		)
	end
end

local function addWaterfall(parent: Instance, position: Vector3, length: number)
	local water = decorativePart(
		parent,
		"SkyWaterfall",
		Vector3.new(2.4, length, 0.28),
		CFrame.new(position - Vector3.new(0, length / 2, 0)),
		Enum.Material.Glass,
		COLORS.Water,
		0.34
	)
	water.CastShadow = false
	local glow = decorativePart(
		parent,
		"WaterfallGlow",
		Vector3.new(0.6, length * 0.94, 0.12),
		water.CFrame + Vector3.new(0.45, 0, -0.08),
		Enum.Material.Neon,
		COLORS.Cyan,
		0.58
	)
	glow.CastShadow = false
end

local function addPortal(parent: Instance, center: Vector3, radius: number, color: Color3)
	local segments = 10
	for index = 1, segments do
		local a = ((index - 1) / segments) * math.pi * 2
		local b = (index / segments) * math.pi * 2
		local from = center + Vector3.new(math.cos(a) * radius, math.sin(a) * radius, 0)
		local to = center + Vector3.new(math.cos(b) * radius, math.sin(b) * radius, 0)
		local segment = rod(parent, "PortalRune", from, to, 0.32, color, Enum.Material.Neon)
		if segment then
			segment.CastShadow = false
		end
	end
	local core = decorativePart(
		parent,
		"PortalCore",
		Vector3.new(radius * 1.42, radius * 1.42, 0.08),
		CFrame.new(center),
		Enum.Material.ForceField,
		color,
		0.72
	)
	core.CastShadow = false
	addGlow(core, color, 1.4, 18)
end

local function addRuin(parent: Instance, center: Vector3, scale: number)
	for _, side in { -1, 1 } do
		decorativePart(
			parent,
			"RuinColumn",
			Vector3.new(1.2 * scale, 6.6 * scale, 1.2 * scale),
			CFrame.new(center + Vector3.new(side * 3.1 * scale, 3.3 * scale, 0)),
			Enum.Material.Marble,
			COLORS.Stone
		)
	end
	decorativePart(
		parent,
		"RuinLintel",
		Vector3.new(7.4 * scale, 1.0 * scale, 1.35 * scale),
		CFrame.new(center + Vector3.new(0, 6.55 * scale, 0)),
		Enum.Material.Marble,
		COLORS.Stone
	)
end

local function addCloudWisps(parent: Instance, center: Vector3, scale: number)
	for index = 1, 4 do
		local offset =
			Vector3.new((index - 2.5) * 4.3 * scale, math.sin(index) * 0.7, math.cos(index * 1.7) * 2.5 * scale)
		local cloud = ball(
			parent,
			"CloudWisp",
			center + offset,
			Vector3.new(8.5, 2.2, 5.0) * scale,
			Enum.Material.SmoothPlastic,
			COLORS.Cloud,
			0.60
		)
		cloud.CastShadow = false
	end
end

local function buildServiceDecks(parent: Instance, origin: Vector3)
	local folder = Instance.new("Folder")
	folder.Name = "DawnGardensArt"
	folder.Parent = parent

	local islandA = origin + Vector3.new(-27, 16, -23)
	local islandB = origin + Vector3.new(31, 28, -38)
	local islandC = origin + Vector3.new(-34, 42, 18)
	addFloatingIsland(folder, islandA, 12, 1)
	addFloatingIsland(folder, islandB, 9, 2)
	addFloatingIsland(folder, islandC, 8, 3)
	addTree(folder, islandA + Vector3.new(-2, 0.5, 1), 0.85)
	addTree(folder, islandA + Vector3.new(3, 0.5, -2), 0.62)
	addTree(folder, islandB + Vector3.new(1, 0.5, 0), 0.68)
	addWaterfall(folder, islandA + Vector3.new(7.5, 0, 0), 18)
	addRuin(folder, islandC + Vector3.new(0, 0.4, 0), 0.72)
	addCloudWisps(folder, origin + Vector3.new(0, -8, -70), 1.25)
end

local function buildCloudline(parent: Instance, origin: Vector3)
	local folder = Instance.new("Folder")
	folder.Name = "CloudTemplesArt"
	folder.Parent = parent

	local islandA = origin + Vector3.new(35, 72, 12)
	local islandB = origin + Vector3.new(-38, 91, -24)
	addFloatingIsland(folder, islandA, 10, 4)
	addFloatingIsland(folder, islandB, 11, 5)
	addTree(folder, islandA + Vector3.new(2, 0.5, -1), 0.72)
	addTree(folder, islandB + Vector3.new(-2, 0.5, 1), 0.78)
	addWaterfall(folder, islandB + Vector3.new(-7.5, 0, 0), 22)
	addRuin(folder, islandA + Vector3.new(-1, 0.4, 0), 0.82)
	addPortal(folder, islandB + Vector3.new(0, 6.4, 0), 4.3, COLORS.Cyan)
	addCloudWisps(folder, origin + Vector3.new(3, 58, -54), 1.5)
end

local function buildStratosphere(parent: Instance, origin: Vector3)
	local folder = Instance.new("Folder")
	folder.Name = "StarSanctuaryArt"
	folder.Parent = parent

	local islandA = origin + Vector3.new(-36, 128, 16)
	local islandB = origin + Vector3.new(34, 154, -18)
	addFloatingIsland(folder, islandA, 9, 6)
	addFloatingIsland(folder, islandB, 12, 7)
	addTree(folder, islandA + Vector3.new(0, 0.5, 0), 0.62)
	addTree(folder, islandB + Vector3.new(3, 0.5, 1), 0.78)
	addTree(folder, islandB + Vector3.new(-3, 0.5, -2), 0.58)
	addWaterfall(folder, islandB + Vector3.new(7.8, 0, 0), 26)
	addPortal(folder, islandB + Vector3.new(0, 7.2, 0), 5.0, COLORS.Lavender)
	addCloudWisps(folder, origin + Vector3.new(-2, 118, -62), 1.7)
end

function WorldArtService.configureLighting()
	if lightingConfigured then
		return
	end
	lightingConfigured = true

	Lighting.ClockTime = ArtConfig.Lighting.ClockTime
	Lighting.Brightness = ArtConfig.Lighting.Brightness
	Lighting.ExposureCompensation = ArtConfig.Lighting.ExposureCompensation
	Lighting.EnvironmentDiffuseScale = 0.48
	Lighting.EnvironmentSpecularScale = 0.72
	Lighting.GlobalShadows = true
	Lighting.ShadowSoftness = 0.36
	Lighting.Ambient = Color3.fromRGB(126, 105, 154)
	Lighting.OutdoorAmbient = Color3.fromRGB(196, 169, 195)
	Lighting.ColorShift_Top = Color3.fromRGB(255, 209, 174)
	Lighting.ColorShift_Bottom = Color3.fromRGB(181, 173, 232)
	Lighting.FogColor = Color3.fromRGB(225, 190, 232)
	Lighting.FogStart = 310
	Lighting.FogEnd = 1500

	local atmosphere = Lighting:FindFirstChildOfClass("Atmosphere") or Instance.new("Atmosphere")
	atmosphere.Density = ArtConfig.Lighting.AtmosphereDensity
	atmosphere.Haze = ArtConfig.Lighting.AtmosphereHaze
	atmosphere.Offset = 0.11
	atmosphere.Color = Color3.fromRGB(237, 215, 255)
	atmosphere.Decay = Color3.fromRGB(214, 155, 130)
	atmosphere.Glare = 0.17
	atmosphere.Parent = Lighting

	local bloom = Lighting:FindFirstChildOfClass("BloomEffect") or Instance.new("BloomEffect")
	bloom.Intensity = 0.52
	bloom.Size = 38
	bloom.Threshold = 1.05
	bloom.Parent = Lighting

	local color = Lighting:FindFirstChildOfClass("ColorCorrectionEffect") or Instance.new("ColorCorrectionEffect")
	color.Brightness = 0.025
	color.Contrast = 0.07
	color.Saturation = 0.10
	color.TintColor = Color3.fromRGB(255, 239, 224)
	color.Parent = Lighting

	local rays = Lighting:FindFirstChildOfClass("SunRaysEffect") or Instance.new("SunRaysEffect")
	rays.Intensity = 0.075
	rays.Spread = 0.72
	rays.Parent = Lighting

	local clouds = Workspace.Terrain:FindFirstChildOfClass("Clouds") or Instance.new("Clouds")
	clouds.Cover = ArtConfig.Lighting.CloudCover
	clouds.Density = ArtConfig.Lighting.CloudDensity
	clouds.Color = Color3.fromRGB(255, 240, 255)
	clouds.Parent = Workspace.Terrain
end

function WorldArtService.createLane(parent: Instance, laneOrigin: Vector3)
	WorldArtService.configureLighting()

	local environment = Instance.new("Folder")
	environment.Name = "EnvironmentArt"
	environment:SetAttribute("VisualIdentity", ArtConfig.VisualIdentity)
	environment.Parent = parent

	buildServiceDecks(environment, laneOrigin)
	buildCloudline(environment, laneOrigin)
	buildStratosphere(environment, laneOrigin)

	return environment
end

return WorldArtService
