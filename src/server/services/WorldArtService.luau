--!strict

local Lighting = game:GetService("Lighting")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local ArtConfig = require(root.config.ArtConfig)
local WorldConfig = require(root.config.WorldConfig)

local WorldArtService = {}

local lightingConfigured = false

local function hex(hexColor: string): Color3
	local value = hexColor:gsub("#", "")
	local r = tonumber(value:sub(1, 2), 16)
	local g = tonumber(value:sub(3, 4), 16)
	local b = tonumber(value:sub(5, 6), 16)
	assert(r and g and b, "invalid art hex")
	return Color3.fromRGB(r, g, b)
end

local function decorativePart(
	parent: Instance,
	name: string,
	size: Vector3,
	cframe: CFrame,
	material: Enum.Material,
	color: Color3,
	transparency: number?
): Part
	local part = Instance.new("Part")
	part.Name = name
	part.Size = size
	part.CFrame = cframe
	part.Anchored = true
	part.CanCollide = false
	part.CanTouch = false
	part.CanQuery = false
	part.CastShadow = true
	part.Material = material
	part.Color = color
	part.Transparency = transparency or 0
	part.TopSurface = Enum.SurfaceType.Smooth
	part.BottomSurface = Enum.SurfaceType.Smooth
	part.Parent = parent
	return part
end

local function addRod(
	parent: Instance,
	name: string,
	fromPosition: Vector3,
	toPosition: Vector3,
	thickness: number,
	color: Color3
)
	local delta = toPosition - fromPosition
	if delta.Magnitude < 0.01 then
		return
	end

	decorativePart(
		parent,
		name,
		Vector3.new(thickness, thickness, delta.Magnitude),
		CFrame.lookAt((fromPosition + toPosition) / 2, toPosition),
		Enum.Material.Metal,
		color
	)
end

local function cableAnchor(parent: Instance): BasePart
	local existing = parent:FindFirstChild("CableRigAnchor")
	if existing and existing:IsA("BasePart") then
		return existing
	end

	local anchor = decorativePart(
		parent,
		"CableRigAnchor",
		Vector3.new(0.05, 0.05, 0.05),
		CFrame.identity,
		Enum.Material.SmoothPlastic,
		Color3.new(0, 0, 0),
		1
	)
	anchor.CastShadow = false
	return anchor
end

local function addCable(
	parent: Instance,
	name: string,
	fromPosition: Vector3,
	toPosition: Vector3,
	color: Color3,
	width: number
)
	local anchorPart = cableAnchor(parent)

	local attachmentA = Instance.new("Attachment")
	attachmentA.Name = name .. "AttachmentA"
	attachmentA.Position = anchorPart.CFrame:PointToObjectSpace(fromPosition)
	attachmentA.Parent = anchorPart

	local attachmentB = Instance.new("Attachment")
	attachmentB.Name = name .. "AttachmentB"
	attachmentB.Position = anchorPart.CFrame:PointToObjectSpace(toPosition)
	attachmentB.Parent = anchorPart

	local beam = Instance.new("Beam")
	beam.Name = name
	beam.Attachment0 = attachmentA
	beam.Attachment1 = attachmentB
	beam.Color = ColorSequence.new(color)
	beam.Width0 = width
	beam.Width1 = width
	beam.Transparency = NumberSequence.new(0.28)
	beam.LightEmission = 0.25
	beam.FaceCamera = true
	beam.Segments = 6
	beam.Parent = anchorPart
end

local function addPracticalLight(parent: Instance, position: Vector3, color: Color3)
	local post = decorativePart(
		parent,
		"PracticalPost",
		Vector3.new(0.34, 2.4, 0.34),
		CFrame.new(position + Vector3.new(0, 1.2, 0)),
		Enum.Material.Metal,
		hex(WorldConfig.Palette.Foundation)
	)
	local cap = decorativePart(
		parent,
		"PracticalLamp",
		Vector3.new(0.66, 0.22, 0.66),
		post.CFrame + Vector3.new(0, 1.34, 0),
		Enum.Material.Neon,
		color
	)
	cap.CastShadow = false

	local light = Instance.new("PointLight")
	light.Color = color
	light.Brightness = 1.15
	light.Range = 11
	light.Shadows = false
	light.Parent = cap
end

local function addBuilding(parent: Instance, position: Vector3, size: Vector3, shade: number)
	local building = decorativePart(
		parent,
		"CityMass",
		size,
		CFrame.new(position + Vector3.new(0, size.Y / 2, 0)),
		Enum.Material.Concrete,
		Color3.fromRGB(shade, shade + 6, shade + 14)
	)

	local bands = math.clamp(math.floor(size.Y / 11), 2, 6)
	for row = 1, bands do
		local bandColor = if row % 2 == 0 then Color3.fromRGB(255, 180, 96) else Color3.fromRGB(103, 155, 184)
		local zSide = if row % 2 == 0 then -1 else 1
		local window = decorativePart(
			parent,
			"WindowBand",
			Vector3.new(math.max(4, size.X - 1.8), 0.18, 0.08),
			CFrame.new(position + Vector3.new(0, row * (size.Y / (bands + 1)), zSide * (size.Z / 2 + 0.05))),
			Enum.Material.Neon,
			bandColor,
			0.28
		)
		window.CastShadow = false
	end

	local roofLight = decorativePart(
		parent,
		"RoofBeacon",
		Vector3.new(math.max(2.2, size.X - 2.8), 0.12, 0.22),
		CFrame.new(position + Vector3.new(0, size.Y + 0.10, 0)),
		Enum.Material.Neon,
		Color3.fromRGB(91, 196, 220),
		0.32
	)
	roofLight.CastShadow = false

	return building
end

local function buildServiceDecks(parent: Instance, origin: Vector3)
	local folder = Instance.new("Folder")
	folder.Name = "ServiceDecksArt"
	folder.Parent = parent

	local steel = hex("#202938")
	local foundation = hex("#121722")
	local amber = hex("#FFB454")

	for side in { -1, 1 } do
		local spineX = origin.X + side * (if side < 0 then 32 else 35)
		decorativePart(
			folder,
			"ServiceSpine",
			Vector3.new(2.4, 52, 2.4),
			CFrame.new(spineX, 26, origin.Z + 2),
			Enum.Material.Metal,
			steel
		)

		for y = 8, 48, 10 do
			local stagger = if side < 0 then (y % 20) * 0.08 else ((y + 10) % 20) * 0.08
			decorativePart(
				folder,
				"ServiceCrossbeam",
				Vector3.new(7.4 + stagger, 0.7, 0.8),
				CFrame.new(origin.X + side * (28.0 + stagger), y, origin.Z + 2 + side * stagger),
				Enum.Material.Metal,
				foundation
			)
		end
	end

	for index = 1, 9 do
		local x = origin.X - 56 + index * 12 + (index % 2) * 2.5
		local height = 18 + ((index * 3) % 5) * 7
		addBuilding(
			folder,
			Vector3.new(x, -18, origin.Z - 98 - ((index * 2) % 4) * 8),
			Vector3.new(9 + ((index + 1) % 3) * 2.5, height, 10 + ((index * 2) % 3) * 2),
			44 + index
		)
	end

	for _, offset in
		{
			Vector3.new(-13, 4.7, -10),
			Vector3.new(13, 4.7, -10),
			Vector3.new(-13, 4.7, 10),
			Vector3.new(13, 4.7, 10),
		}
	do
		addPracticalLight(folder, origin + offset, amber)
	end

	for _, y in { 13, 29, 45 } do
		addCable(
			folder,
			"ServiceCable",
			Vector3.new(origin.X - 32, y, origin.Z + 2),
			Vector3.new(origin.X - 43, y - 5, origin.Z + 24),
			Color3.fromRGB(84, 98, 116),
			0.055
		)
		addCable(
			folder,
			"ServiceCable",
			Vector3.new(origin.X + 35, y, origin.Z + 2),
			Vector3.new(origin.X + 46, y - 5, origin.Z + 24),
			Color3.fromRGB(84, 98, 116),
			0.055
		)
	end
end

local function buildCloudline(parent: Instance, origin: Vector3)
	local folder = Instance.new("Folder")
	folder.Name = "CloudlineArt"
	folder.Parent = parent

	local steel = hex("#263447")
	local cyan = hex("#5EEBFF")
	local glass = Color3.fromRGB(116, 148, 168)

	for side in { -1, 1 } do
		local x = origin.X + side * (if side < 0 then 33 else 36)
		decorativePart(
			folder,
			"CloudlineMast",
			Vector3.new(1.4, 58, 1.4),
			CFrame.new(x, 86, origin.Z - 3),
			Enum.Material.Metal,
			steel
		)

		for y = 62, 112, 12 do
			local armShift = if side < 0 then (y % 24) * 0.06 else ((y + 12) % 24) * 0.06
			decorativePart(
				folder,
				"CloudlineArm",
				Vector3.new(8.1 + armShift, 0.42, 0.58),
				CFrame.new(origin.X + side * (29.2 + armShift), y, origin.Z - 3 + side * armShift),
				Enum.Material.Metal,
				steel
			)
		end
	end

	for index = 1, 4 do
		local panel = decorativePart(
			folder,
			"GlassServicePanel",
			Vector3.new(7, 0.24, 3.2),
			CFrame.new(origin.X + (if index % 2 == 0 then -27 else 27), 70 + index * 12, origin.Z + (index - 2.5) * 8),
			Enum.Material.Glass,
			glass,
			0.54
		)
		panel.CastShadow = false

		decorativePart(
			folder,
			"PanelGuide",
			Vector3.new(5.8, 0.12, 0.16),
			panel.CFrame + Vector3.new(0, 0.22, 0),
			Enum.Material.Neon,
			cyan,
			0.12
		).CastShadow =
			false
	end

	for _, y in { 70, 88, 106 } do
		addCable(
			folder,
			"CloudlineTensionCable",
			Vector3.new(origin.X - 34, y, origin.Z - 3),
			Vector3.new(origin.X + 34, y + 4, origin.Z + 17),
			Color3.fromRGB(116, 203, 220),
			0.05
		)
	end
end

local function buildStratosphere(parent: Instance, origin: Vector3)
	local folder = Instance.new("Folder")
	folder.Name = "StratosphereArt"
	folder.Parent = parent

	local deepSteel = hex("#182331")
	local pale = hex("#B9F6FF")
	local foundation = hex("#121722")

	decorativePart(
		folder,
		"HighSpine",
		Vector3.new(3.0, 78, 3.0),
		CFrame.new(origin.X - 36, 145, origin.Z + 10),
		Enum.Material.Metal,
		deepSteel
	)

	for index = 1, 7 do
		local y = 116 + index * 9
		local side = if index % 2 == 0 then 1 else -1
		decorativePart(
			folder,
			"AntennaRib",
			Vector3.new(11, 0.48, 0.62),
			CFrame.new(origin.X - 36 + side * 4.3, y, origin.Z + 10) * CFrame.Angles(0, 0, math.rad(side * 12)),
			Enum.Material.Metal,
			foundation
		)
	end

	for index = 1, 5 do
		local y = 126 + index * 12
		local beacon = decorativePart(
			folder,
			"HighBeacon",
			Vector3.new(0.48, 0.48, 0.48),
			CFrame.new(origin.X + 32, y, origin.Z - 10 + (index % 2) * 14),
			Enum.Material.Neon,
			pale,
			0.08
		)
		beacon.CastShadow = false

		local light = Instance.new("PointLight")
		light.Color = pale
		light.Brightness = 0.8
		light.Range = 8
		light.Shadows = false
		light.Parent = beacon
	end

	local ringCenter = origin + Vector3.new(0, 151, 2)
	local ringRadius = 31
	local ringSegments = 14
	for index = 1, ringSegments do
		local a = (index - 1) / ringSegments * math.pi * 2
		local b = index / ringSegments * math.pi * 2
		local fromPosition = ringCenter
			+ Vector3.new(math.cos(a) * ringRadius, math.sin(a) * 8, math.sin(a) * ringRadius)
		local toPosition = ringCenter + Vector3.new(math.cos(b) * ringRadius, math.sin(b) * 8, math.sin(b) * ringRadius)
		addRod(folder, "StratosphereRing", fromPosition, toPosition, 0.34, deepSteel)
	end

	for _, angle in { 0, math.pi * 0.66, math.pi * 1.33 } do
		local ringPoint = ringCenter
			+ Vector3.new(math.cos(angle) * ringRadius, math.sin(angle) * 8, math.sin(angle) * ringRadius)
		addCable(folder, "HighTensionCable", ringPoint, ringPoint + Vector3.new(0, 36, 0), pale, 0.045)
	end
end

function WorldArtService.configureLighting()
	if lightingConfigured then
		return
	end
	lightingConfigured = true

	Lighting.ClockTime = ArtConfig.Lighting.ClockTime
	Lighting.Brightness = ArtConfig.Lighting.Brightness
	Lighting.ExposureCompensation = ArtConfig.Lighting.ExposureCompensation
	Lighting.GlobalShadows = true
	Lighting.ShadowSoftness = 0.28
	Lighting.Ambient = Color3.fromRGB(74, 83, 101)
	Lighting.OutdoorAmbient = Color3.fromRGB(112, 119, 132)

	local atmosphere = Lighting:FindFirstChildOfClass("Atmosphere") or Instance.new("Atmosphere")
	atmosphere.Density = ArtConfig.Lighting.AtmosphereDensity
	atmosphere.Haze = ArtConfig.Lighting.AtmosphereHaze
	atmosphere.Color = Color3.fromRGB(196, 214, 226)
	atmosphere.Decay = Color3.fromRGB(105, 117, 137)
	atmosphere.Glare = 0.12
	atmosphere.Parent = Lighting

	local bloom = Lighting:FindFirstChildOfClass("BloomEffect") or Instance.new("BloomEffect")
	bloom.Intensity = 0.22
	bloom.Size = 22
	bloom.Threshold = 1.45
	bloom.Parent = Lighting

	local color = Lighting:FindFirstChildOfClass("ColorCorrectionEffect") or Instance.new("ColorCorrectionEffect")
	color.Brightness = 0.01
	color.Contrast = 0.06
	color.Saturation = -0.04
	color.TintColor = Color3.fromRGB(238, 245, 251)
	color.Parent = Lighting

	local clouds = Workspace.Terrain:FindFirstChildOfClass("Clouds") or Instance.new("Clouds")
	clouds.Cover = ArtConfig.Lighting.CloudCover
	clouds.Density = ArtConfig.Lighting.CloudDensity
	clouds.Color = Color3.fromRGB(232, 238, 243)
	clouds.Parent = Workspace.Terrain
end

function WorldArtService.createLane(parent: Instance, laneOrigin: Vector3)
	WorldArtService.configureLighting()

	local environment = Instance.new("Folder")
	environment.Name = "EnvironmentArt"
	environment.Parent = parent

	buildServiceDecks(environment, laneOrigin)
	buildCloudline(environment, laneOrigin)
	buildStratosphere(environment, laneOrigin)

	return environment
end

return WorldArtService
