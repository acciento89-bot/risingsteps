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
	local item = Instance.new("Part")
	item.Name = name
	item.Anchored = true
	item.CanCollide = canCollide
	item.CanTouch = canCollide
	item.CanQuery = canCollide
	item.Material = material
	item.Color = color
	item.Size = size
	item.CFrame = cframe
	item.TopSurface = Enum.SurfaceType.Smooth
	item.BottomSurface = Enum.SurfaceType.Smooth
	item.Parent = parent
	return item
end

local function ball(parent: Instance, name: string, position: Vector3, size: Vector3, color: Color3): Part
	local item = makePart(parent, name, size, CFrame.new(position), Enum.Material.Slate, color, false)
	item.Shape = Enum.PartType.Ball
	return item
end

function StartDeckService.create(parent: Instance, player: Player, laneOrigin: Vector3): SpawnLocation
	local model = Instance.new("Model")
	model.Name = "StartDeck"
	model:SetAttribute("AuthoredAsset", true)
	model:SetAttribute("VisualIdentity", "fantasy-sky-realms")
	model:SetAttribute("OwnerUserId", player.UserId)
	model.Parent = parent

	local y = WorldConfig.StartDeck.CenterY
	local deckSize = WorldConfig.StartDeck.DeckSize
	local deckThickness = WorldConfig.StartDeck.DeckThickness
	local center = laneOrigin + Vector3.new(0, y, 0)
	local stone = hex(WorldConfig.Palette.Steel)
	local grass = hex(WorldConfig.Palette.Concrete)
	local gold = hex(WorldConfig.Palette.WarmAccent)
	local cyan = hex(WorldConfig.Palette.GameplayAccent)

	local base = makePart(
		model,
		"SkyIslandDeck",
		Vector3.new(deckSize, deckThickness, deckSize),
		CFrame.new(center),
		Enum.Material.Slate,
		stone,
		true
	)

	makePart(
		model,
		"GrassLanding",
		Vector3.new(
			WorldConfig.StartDeck.LandingInsetSize,
			WorldConfig.StartDeck.LandingInsetThickness,
			WorldConfig.StartDeck.LandingInsetSize
		),
		CFrame.new(center + Vector3.new(0, deckThickness / 2 + WorldConfig.StartDeck.LandingInsetThickness / 2, 0)),
		Enum.Material.Grass,
		grass,
		false
	)

	for index, offset in
		{
			Vector3.new(-3.8, -1.3, -3.5),
			Vector3.new(3.4, -1.6, -2.6),
			Vector3.new(-2.2, -2.1, 3.4),
			Vector3.new(3.6, -2.3, 3.1),
			Vector3.new(0.2, -2.8, 0.4),
		}
	do
		ball(
			model,
			("IslandRock%02d"):format(index),
			center + offset,
			Vector3.new(4.5 - index * 0.28, 3.5 + index * 0.22, 4.0 - index * 0.2),
			if index % 2 == 0 then stone:Lerp(Color3.new(1, 1, 1), 0.08) else stone
		)
	end

	local edgeY = center.Y + deckThickness / 2 + 0.18
	for _, definition in
		{
			{ "RuneNorth", Vector3.new(deckSize - 1.0, 0.13, 0.18), Vector3.new(0, 0, -deckSize / 2 + 0.34) },
			{ "RuneSouth", Vector3.new(deckSize - 1.0, 0.13, 0.18), Vector3.new(0, 0, deckSize / 2 - 0.34) },
			{ "RuneWest", Vector3.new(0.18, 0.13, deckSize - 1.0), Vector3.new(-deckSize / 2 + 0.34, 0, 0) },
			{ "RuneEast", Vector3.new(0.18, 0.13, deckSize - 1.0), Vector3.new(deckSize / 2 - 0.34, 0, 0) },
		}
	do
		local rune = makePart(
			model,
			definition[1],
			definition[2],
			CFrame.new(Vector3.new(center.X, edgeY, center.Z) + definition[3]),
			Enum.Material.Neon,
			gold,
			false
		)
		rune.Transparency = 0.08
	end

	local guideY = center.Y + deckThickness / 2 + 0.19
	for index = 1, WorldConfig.StartDeck.OnboardingGuideCount do
		local alpha = (index - 1) / math.max(1, WorldConfig.StartDeck.OnboardingGuideCount - 1)
		local guide = makePart(
			model,
			("MagicGuide%02d"):format(index),
			Vector3.new(3.7 - alpha * 1.2, 0.08, 0.34),
			CFrame.new(center.X, guideY, center.Z + 1.8 - alpha * 4.0),
			Enum.Material.Neon,
			cyan,
			false
		)
		guide.Transparency = 0.12 + alpha * 0.12
	end

	for _, side in { -1, 1 } do
		local post = makePart(
			model,
			"RunePost",
			Vector3.new(0.38, 2.6, 0.38),
			CFrame.new(center + Vector3.new(side * 4.6, 1.35, 3.4)),
			Enum.Material.Marble,
			stone:Lerp(Color3.new(1, 1, 1), 0.22),
			false
		)
		local crystal = makePart(
			model,
			"GuideCrystal",
			Vector3.new(0.62, 0.62, 0.62),
			post.CFrame + Vector3.new(0, 1.65, 0),
			Enum.Material.Neon,
			gold,
			false
		)
		crystal.Shape = Enum.PartType.Ball
		local light = Instance.new("PointLight")
		light.Color = gold
		light.Brightness = 1.1
		light.Range = 9
		light.Shadows = false
		light.Parent = crystal
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
