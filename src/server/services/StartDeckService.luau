--!strict

local Players = game:GetService("Players")
local Workspace = game:GetService("Workspace")

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local root = ReplicatedStorage:WaitForChild("RisingSteps")
local WorldConfig = require(root.config.WorldConfig)

local StartDeckService = {}

local function hex(hexColor: string): Color3
	local value = hexColor:gsub("#", "")
	assert(#value == 6, "hex color must contain six digits")

	local r = tonumber(value:sub(1, 2), 16)
	local g = tonumber(value:sub(3, 4), 16)
	local b = tonumber(value:sub(5, 6), 16)
	assert(r and g and b, "hex color contains invalid digits")

	return Color3.fromRGB(r, g, b)
end

local function makePart(
	parent: Instance,
	name: string,
	size: Vector3,
	cframe: CFrame,
	material: Enum.Material,
	color: Color3
): Part
	local part = Instance.new("Part")
	part.Name = name
	part.Anchored = true
	part.CanCollide = true
	part.CanTouch = true
	part.CanQuery = true
	part.Material = material
	part.Color = color
	part.Size = size
	part.CFrame = cframe
	part.TopSurface = Enum.SurfaceType.Smooth
	part.BottomSurface = Enum.SurfaceType.Smooth
	part.Parent = parent
	return part
end

local function buildVisualDeck(worldFolder: Folder): SpawnLocation
	local existing = worldFolder:FindFirstChild(WorldConfig.StartDeck.Name)
	if existing then
		existing:Destroy()
	end

	local model = Instance.new("Model")
	model.Name = WorldConfig.StartDeck.Name
	model:SetAttribute("AuthoredAsset", true)
	model.Parent = worldFolder

	local y = WorldConfig.StartDeck.CenterY
	local deckSize = WorldConfig.StartDeck.DeckSize
	local deckThickness = WorldConfig.StartDeck.DeckThickness

	local base = makePart(
		model,
		"StructuralDeck",
		Vector3.new(deckSize, deckThickness, deckSize),
		CFrame.new(0, y, 0),
		Enum.Material.Metal,
		hex(WorldConfig.Palette.Steel)
	)

	local inset = makePart(
		model,
		"LandingInset",
		Vector3.new(
			WorldConfig.StartDeck.LandingInsetSize,
			WorldConfig.StartDeck.LandingInsetThickness,
			WorldConfig.StartDeck.LandingInsetSize
		),
		CFrame.new(0, y + deckThickness / 2 + WorldConfig.StartDeck.LandingInsetThickness / 2, 0),
		Enum.Material.Rubber,
		hex(WorldConfig.Palette.Concrete)
	)
	inset.CanCollide = false

	local trimThickness = 0.24
	local trimHeight = 0.18
	local trimOffset = deckSize / 2 - trimThickness / 2
	local trimY = y + deckThickness / 2 + trimHeight / 2

	makePart(
		model,
		"TrimNorth",
		Vector3.new(deckSize, trimHeight, trimThickness),
		CFrame.new(0, trimY, -trimOffset),
		Enum.Material.Metal,
		hex(WorldConfig.Palette.WarmAccent)
	).CanCollide = false
	makePart(
		model,
		"TrimSouth",
		Vector3.new(deckSize, trimHeight, trimThickness),
		CFrame.new(0, trimY, trimOffset),
		Enum.Material.Metal,
		hex(WorldConfig.Palette.WarmAccent)
	).CanCollide = false
	makePart(
		model,
		"TrimWest",
		Vector3.new(trimThickness, trimHeight, deckSize),
		CFrame.new(-trimOffset, trimY, 0),
		Enum.Material.Metal,
		hex(WorldConfig.Palette.WarmAccent)
	).CanCollide = false
	makePart(
		model,
		"TrimEast",
		Vector3.new(trimThickness, trimHeight, deckSize),
		CFrame.new(trimOffset, trimY, 0),
		Enum.Material.Metal,
		hex(WorldConfig.Palette.WarmAccent)
	).CanCollide = false

	for index, x in { -4.4, -1.5, 1.5, 4.4 } do
		local rib = makePart(
			model,
			("UndersideRib%02d"):format(index),
			Vector3.new(0.34, 1.2, 9.6),
			CFrame.new(x, y - deckThickness / 2 - 0.6, 0),
			Enum.Material.Metal,
			hex(WorldConfig.Palette.Foundation)
		)
		rib.CanCollide = false
	end

	local spawn = Instance.new("SpawnLocation")
	spawn.Name = "SafeSpawn"
	spawn.Anchored = true
	spawn.CanCollide = false
	spawn.CanTouch = false
	spawn.CanQuery = false
	spawn.Transparency = 1
	spawn.Neutral = true
	spawn.Duration = 0
	spawn.Size = Vector3.new(5, 1, 5)
	spawn.CFrame = CFrame.new(
		0,
		y + deckThickness / 2 + WorldConfig.StartDeck.SpawnHeightAboveDeck,
		0
	)
	spawn.Parent = model

	model.PrimaryPart = base
	return spawn
end

function StartDeckService.start(): SpawnLocation
	local worldFolder = Workspace:FindFirstChild(WorldConfig.WorldFolderName)
	if not worldFolder then
		worldFolder = Instance.new("Folder")
		worldFolder.Name = WorldConfig.WorldFolderName
		worldFolder.Parent = Workspace
	end
	assert(worldFolder:IsA("Folder"), "RisingStepsWorld must be a Folder")

	local spawn = buildVisualDeck(worldFolder)

	local function bindPlayer(player: Player)
		player.RespawnLocation = spawn
	end

	for _, player in Players:GetPlayers() do
		bindPlayer(player)
	end
	Players.PlayerAdded:Connect(bindPlayer)

	return spawn
end

return StartDeckService
