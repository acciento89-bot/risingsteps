--!strict

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
	color: Color3,
	canCollide: boolean
): Part
	local part = Instance.new("Part")
	part.Name = name
	part.Anchored = true
	part.CanCollide = canCollide
	part.CanTouch = canCollide
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

function StartDeckService.create(parent: Instance, player: Player, laneOrigin: Vector3): SpawnLocation
	local model = Instance.new("Model")
	model.Name = "StartDeck"
	model:SetAttribute("AuthoredAsset", true)
	model:SetAttribute("OwnerUserId", player.UserId)
	model.Parent = parent

	local y = WorldConfig.StartDeck.CenterY
	local deckSize = WorldConfig.StartDeck.DeckSize
	local deckThickness = WorldConfig.StartDeck.DeckThickness
	local center = laneOrigin + Vector3.new(0, y, 0)

	local base = makePart(
		model,
		"StructuralDeck",
		Vector3.new(deckSize, deckThickness, deckSize),
		CFrame.new(center),
		Enum.Material.Metal,
		hex(WorldConfig.Palette.Steel),
		true
	)

	makePart(
		model,
		"LandingInset",
		Vector3.new(
			WorldConfig.StartDeck.LandingInsetSize,
			WorldConfig.StartDeck.LandingInsetThickness,
			WorldConfig.StartDeck.LandingInsetSize
		),
		CFrame.new(center + Vector3.new(0, deckThickness / 2 + WorldConfig.StartDeck.LandingInsetThickness / 2, 0)),
		Enum.Material.Rubber,
		hex(WorldConfig.Palette.Concrete),
		false
	)

	local trimThickness = 0.24
	local trimHeight = 0.18
	local trimOffset = deckSize / 2 - trimThickness / 2
	local trimY = center.Y + deckThickness / 2 + trimHeight / 2
	local accent = hex(WorldConfig.Palette.WarmAccent)

	local trimDefinitions = {
		{ "TrimNorth", Vector3.new(deckSize, trimHeight, trimThickness), Vector3.new(0, 0, -trimOffset) },
		{ "TrimSouth", Vector3.new(deckSize, trimHeight, trimThickness), Vector3.new(0, 0, trimOffset) },
		{ "TrimWest", Vector3.new(trimThickness, trimHeight, deckSize), Vector3.new(-trimOffset, 0, 0) },
		{ "TrimEast", Vector3.new(trimThickness, trimHeight, deckSize), Vector3.new(trimOffset, 0, 0) },
	}

	for _, definition in trimDefinitions do
		makePart(
			model,
			definition[1],
			definition[2],
			CFrame.new(Vector3.new(center.X, trimY, center.Z) + definition[3]),
			Enum.Material.Metal,
			accent,
			false
		)
	end

	for index, x in { -4.4, -1.5, 1.5, 4.4 } do
		makePart(
			model,
			("UndersideRib%02d"):format(index),
			Vector3.new(0.34, 1.2, 9.6),
			CFrame.new(center + Vector3.new(x, -deckThickness / 2 - 0.6, 0)),
			Enum.Material.Metal,
			hex(WorldConfig.Palette.Foundation),
			false
		)
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
	spawn.CFrame =
		CFrame.new(center + Vector3.new(0, deckThickness / 2 + WorldConfig.StartDeck.SpawnHeightAboveDeck, 0))
	spawn.Parent = model

	model.PrimaryPart = base
	return spawn
end

return StartDeckService
