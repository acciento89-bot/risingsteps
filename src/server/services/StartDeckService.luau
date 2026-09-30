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

	-- The first seconds must read as a deliberate runway into the climb, not a spawn beside a block pile.
	local guideY = center.Y + deckThickness / 2 + 0.17
	for index = 1, WorldConfig.StartDeck.OnboardingGuideCount do
		local alpha = (index - 1) / math.max(1, WorldConfig.StartDeck.OnboardingGuideCount - 1)
		local z = 2.0 - alpha * 4.2
		local width = 3.8 - alpha * 1.2
		local guide = makePart(
			model,
			("ForwardGuide%02d"):format(index),
			Vector3.new(width, 0.08, 0.34),
			CFrame.new(center.X, guideY, center.Z + z),
			Enum.Material.Neon,
			hex(WorldConfig.Palette.GameplayAccent),
			false
		)
		guide.Transparency = 0.22 + alpha * 0.18
	end

	makePart(
		model,
		"LaunchLip",
		Vector3.new(4.6, 0.16, 0.38),
		CFrame.new(center.X, guideY + 0.03, center.Z - deckSize / 2 + 0.72),
		Enum.Material.Metal,
		hex(WorldConfig.Palette.WarmAccent),
		false
	)

	-- Frame the spawn as a designed launch bay while keeping the forward edge visually open.
	for _, side in { -1, 1 } do
		makePart(
			model,
			"LaunchRail",
			Vector3.new(0.18, 0.78, 5.2),
			CFrame.new(center + Vector3.new(side * (deckSize / 2 - 0.22), 0.48, 1.6)),
			Enum.Material.Metal,
			hex(WorldConfig.Palette.Foundation),
			false
		)

		makePart(
			model,
			"RearGuard",
			Vector3.new(1.8, 0.18, 0.24),
			CFrame.new(center + Vector3.new(side * 4.6, 0.78, deckSize / 2 - 0.34)),
			Enum.Material.Metal,
			hex(WorldConfig.Palette.GameplayAccent),
			false
		)

		local brace = makePart(
			model,
			"LaunchBrace",
			Vector3.new(0.22, 1.7, 0.22),
			CFrame.new(center + Vector3.new(side * 5.02, -0.02, 2.9)) * CFrame.Angles(0, 0, math.rad(side * 18)),
			Enum.Material.Metal,
			hex(WorldConfig.Palette.Foundation),
			false
		)
		brace.CastShadow = true
	end

	for _, z in { 3.55, 1.1 } do
		makePart(
			model,
			"DeckSeam",
			Vector3.new(deckSize - 1.5, 0.035, 0.08),
			CFrame.new(center + Vector3.new(0, deckThickness / 2 + 0.16, z)),
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
	local spawnPosition = center
		+ Vector3.new(
			0,
			deckThickness / 2 + WorldConfig.StartDeck.SpawnHeightAboveDeck,
			WorldConfig.StartDeck.SpawnBackOffset
		)
	spawn.CFrame = CFrame.lookAt(spawnPosition, spawnPosition + Vector3.new(0, 0, -1))
	spawn.Parent = model

	model.PrimaryPart = base
	return spawn
end

return StartDeckService
