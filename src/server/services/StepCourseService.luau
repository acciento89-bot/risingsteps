--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local TweenService = game:GetService("TweenService")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local CourseConfig = require(root.config.CourseConfig)
local GameConfig = require(root.config.GameConfig)
local SecurityConfig = require(root.config.SecurityConfig)
local WorldConfig = require(root.config.WorldConfig)
local ScoreRules = require(root.rules.ScoreRules)
local SecurityRules = require(root.rules.SecurityRules)
local StepRules = require(root.rules.StepRules)
local StepSequence = require(root.rules.StepSequence)

local DiagnosticsService = require(script.Parent.DiagnosticsService)
local PlayerProfileService = require(script.Parent.PlayerProfileService)
local StartDeckService = require(script.Parent.StartDeckService)
local WorldArtService = require(script.Parent.WorldArtService)

type StepRuntime = {
	Model: Model,
	Collision: Part,
	Inset: Part,
	Accents: { BasePart },
	Decorations: { BasePart },
	WarningSignals: { BasePart },
	BaseCollisionColor: Color3,
	BaseInsetColor: Color3,
	BaseAccentColor: Color3,
	Definition: any,
	State: string,
}

type Course = {
	Player: Player,
	Model: Model,
	Spawn: SpawnLocation,
	LaneIndex: number,
	Origin: Vector3,
	Steps: { [number]: StepRuntime },
	Height: number,
	Streak: number,
	HighestY: number,
	Failed: boolean,
	Ready: boolean,
	Assisted: boolean,
	RevivesUsed: number,
	PendingRevive: boolean,
	LastLandingAt: number,
	RunToken: number,
	CharacterDiedConnection: RBXScriptConnection?,
}

local StepCourseService = {}

local courses: { [Player]: Course } = {}
local freeLaneIndices: { number } = {}
local nextLaneIndex = 0
local fallAccumulator = 0
local lastPlayerCommandAt: { [Player]: number } = {}

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
	Expiring = Color3.fromRGB(255, 93, 108),
}

local function paletteForBand(bandId: string)
	if bandId == "service-decks" then
		return {
			Collision = Color3.fromRGB(42, 54, 70),
			Inset = Color3.fromRGB(58, 70, 84),
			Accent = COLORS.Active,
			Detail = COLORS.Warm,
			CollisionMaterial = Enum.Material.DiamondPlate,
			InsetMaterial = Enum.Material.Rubber,
		}
	elseif bandId == "cloudline-works" then
		return {
			Collision = Color3.fromRGB(45, 63, 82),
			Inset = Color3.fromRGB(64, 82, 99),
			Accent = Color3.fromRGB(94, 235, 255),
			Detail = Color3.fromRGB(184, 237, 246),
			CollisionMaterial = Enum.Material.Metal,
			InsetMaterial = Enum.Material.Metal,
		}
	end

	return {
		Collision = Color3.fromRGB(57, 72, 91),
		Inset = Color3.fromRGB(78, 94, 111),
		Accent = Color3.fromRGB(185, 246, 255),
		Detail = Color3.fromRGB(220, 251, 255),
		CollisionMaterial = Enum.Material.Slate,
		InsetMaterial = Enum.Material.SmoothPlastic,
	}
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

local function makeWedge(parent: Instance, name: string, size: Vector3, cframe: CFrame, color: Color3): WedgePart
	local part = Instance.new("WedgePart")
	part.Name = name
	part.Anchored = true
	part.CanCollide = false
	part.CanTouch = false
	part.CanQuery = true
	part.Material = Enum.Material.Metal
	part.Color = color
	part.Size = size
	part.CFrame = cframe
	part.Parent = parent
	return part
end

local function buildStep(parent: Instance, definition: any, laneOrigin: Vector3): StepRuntime
	local model = Instance.new("Model")
	model.Name = ("Step_%03d"):format(definition.Id)
	model:SetAttribute("StepId", definition.Id)
	model:SetAttribute("BandId", definition.BandId)
	model:SetAttribute("Variant", definition.Variant)
	model:SetAttribute("State", StepRules.State.Active)
	model.Parent = parent

	local center = laneOrigin + Vector3.new(definition.X, definition.Y, definition.Z)
	local size = GameConfig.Step.BaseFootprint
	local thickness = GameConfig.Step.CollisionThickness
	local palette = paletteForBand(definition.BandId)

	local collision = makePart(
		model,
		"Collision",
		Vector3.new(size, thickness, size),
		CFrame.new(center),
		palette.CollisionMaterial,
		palette.Collision,
		true
	)
	collision:SetAttribute("StepId", definition.Id)

	local inset = makePart(
		model,
		"LandingInset",
		Vector3.new(size - 0.72, 0.16, size - 0.72),
		CFrame.new(center + Vector3.new(0, thickness / 2 + 0.08, 0)),
		palette.InsetMaterial,
		palette.Inset,
		false
	)

	local accents: { BasePart } = {}
	local decorations: { BasePart } = { inset }
	local edgeY = center.Y + thickness / 2 + 0.13
	local edgeOffset = size / 2 - 0.14
	local edgeThickness = 0.20

	local accentA = makePart(
		model,
		"AccentA",
		Vector3.new(size - 0.5, 0.12, edgeThickness),
		CFrame.new(center.X, edgeY, center.Z - edgeOffset),
		Enum.Material.Metal,
		palette.Accent,
		false
	)
	local accentB = makePart(
		model,
		"AccentB",
		Vector3.new(size - 0.5, 0.12, edgeThickness),
		CFrame.new(center.X, edgeY, center.Z + edgeOffset),
		Enum.Material.Metal,
		palette.Accent,
		false
	)
	table.insert(accents, accentA)
	table.insert(accents, accentB)
	table.insert(decorations, accentA)
	table.insert(decorations, accentB)

	local support = makePart(
		model,
		"SupportSpine",
		Vector3.new(0.55, 1.45, size - 1.0),
		CFrame.new(center + Vector3.new(0, -thickness / 2 - 0.72, 0)),
		Enum.Material.Metal,
		COLORS.Foundation,
		false
	)
	table.insert(decorations, support)

	local landingMark = makePart(
		model,
		"LandingMark",
		Vector3.new(1.55, 0.04, 1.55),
		CFrame.new(center + Vector3.new(0, thickness / 2 + 0.18, 0)),
		Enum.Material.Neon,
		palette.Accent,
		false
	)
	landingMark.Transparency = 0.48
	table.insert(decorations, landingMark)

	for _, zSide in { -1, 1 } do
		local underGlow = makePart(
			model,
			"UnderGlow",
			Vector3.new(size - 1.2, 0.10, 0.12),
			CFrame.new(center + Vector3.new(0, -thickness / 2 - 0.10, zSide * (size / 2 - 0.20))),
			Enum.Material.Neon,
			palette.Accent,
			false
		)
		underGlow.Transparency = 0.38
		table.insert(decorations, underGlow)
	end

	local boltDiagonal = if definition.Id % 2 == 0 then 1 else -1
	for _, xSide in { -1, 1 } do
		local zSide = xSide * boltDiagonal
		local bolt = makePart(
			model,
			"DeckBolt",
			Vector3.new(0.20, 0.20, 0.20),
			CFrame.new(center + Vector3.new(xSide * (size / 2 - 0.42), thickness / 2 + 0.16, zSide * (size / 2 - 0.42))),
			Enum.Material.Metal,
			palette.Detail,
			false
		)
		bolt.Shape = Enum.PartType.Ball
		table.insert(decorations, bolt)
	end

	local warningSignals: { BasePart } = {}
	for _, side in { -1, 1 } do
		local fin = makePart(
			model,
			"WarningFin",
			Vector3.new(0.20, 0.76, 1.5),
			CFrame.new(center + Vector3.new(side * (size / 2 - 0.12), 0.62, 0)),
			Enum.Material.Metal,
			COLORS.Warning,
			false
		)
		fin.Transparency = 1
		table.insert(warningSignals, fin)
		table.insert(decorations, fin)
	end

	if definition.Variant == 2 or definition.Variant == 5 then
		local brace = makeWedge(
			model,
			"SideBrace",
			Vector3.new(1.2, 1.0, 3.2),
			CFrame.new(center + Vector3.new(size / 2 + 0.6, -0.55, 0)) * CFrame.Angles(0, math.rad(90), 0),
			COLORS.Foundation
		)
		table.insert(decorations, brace)
	elseif definition.Variant == 3 or definition.Variant == 6 then
		local brace = makeWedge(
			model,
			"SideBrace",
			Vector3.new(1.2, 1.0, 3.2),
			CFrame.new(center + Vector3.new(-size / 2 - 0.6, -0.55, 0)) * CFrame.Angles(0, math.rad(-90), 0),
			COLORS.Foundation
		)
		table.insert(decorations, brace)
	else
		local crossbar = makePart(
			model,
			"Crossbar",
			Vector3.new(size - 1.4, 0.28, 0.45),
			CFrame.new(center + Vector3.new(0, -thickness / 2 - 0.48, 0)),
			Enum.Material.Metal,
			COLORS.Foundation,
			false
		)
		table.insert(decorations, crossbar)
	end

	if definition.Variant == 4 then
		for _, side in { -1, 1 } do
			local rail = makePart(
				model,
				"RailFragment",
				Vector3.new(0.18, 0.68, 2.1),
				CFrame.new(center + Vector3.new(side * (size / 2 + 0.16), 0.10, 0)),
				Enum.Material.Metal,
				COLORS.Foundation,
				false
			)
			table.insert(decorations, rail)
		end
	elseif definition.Variant == 5 then
		for _, side in { -1, 1 } do
			local anchor = makePart(
				model,
				"CableAnchor",
				Vector3.new(0.38, 1.15, 0.38),
				CFrame.new(center + Vector3.new(side * 2.35, -0.18, -2.35)),
				Enum.Material.Metal,
				COLORS.Foundation,
				false
			)
			table.insert(decorations, anchor)
		end
	elseif definition.Variant == 6 then
		local nose = makePart(
			model,
			"CantileverNose",
			Vector3.new(2.8, 0.34, 0.72),
			CFrame.new(center + Vector3.new(0, -0.42, -size / 2 - 0.36)),
			Enum.Material.Metal,
			COLORS.Foundation,
			false
		)
		table.insert(decorations, nose)
	end

	if definition.Variant >= 4 then
		local marker = makePart(
			model,
			"WarmMarker",
			Vector3.new(0.28, 0.18, 1.65),
			CFrame.new(center.X, edgeY + 0.03, center.Z),
			Enum.Material.Metal,
			COLORS.Warm,
			false
		)
		table.insert(decorations, marker)
	end

	-- Break the square generated silhouette without cloning decorative hardware onto every step.
	-- This keeps the authored asymmetry while staying inside the per-lane presentation budget.
	local cornerSignX = if definition.Variant % 2 == 0 then 1 else -1
	local cornerSignZ = if definition.Variant <= 3 then -1 else 1

	if definition.Id % 2 == 0 then
		local cornerPlate = makePart(
			model,
			"CornerPlate",
			Vector3.new(1.15, 0.16, 1.15),
			CFrame.new(
				center
					+ Vector3.new(
						cornerSignX * (size / 2 - 0.58),
						thickness / 2 + 0.12,
						cornerSignZ * (size / 2 - 0.58)
					)
			) * CFrame.Angles(0, math.rad(45), 0),
			Enum.Material.Metal,
			palette.Detail,
			false
		)
		cornerPlate.Transparency = 0.08
		table.insert(decorations, cornerPlate)
	end

	if definition.Id % 3 == 0 then
		local hangingBracket = makePart(
			model,
			"HangingBracket",
			Vector3.new(0.28, 1.5 + (definition.Variant % 3) * 0.32, 0.72),
			CFrame.new(center + Vector3.new(-cornerSignX * (size / 2 - 0.52), -thickness / 2 - 0.82, cornerSignZ * 0.9)),
			Enum.Material.Metal,
			COLORS.Foundation,
			false
		)
		table.insert(decorations, hangingBracket)
	end

	for _, part in decorations do
		part:SetAttribute("BaseTransparency", part.Transparency)
	end

	model.PrimaryPart = collision

	return {
		Model = model,
		Collision = collision,
		Inset = inset,
		Accents = accents,
		Decorations = decorations,
		WarningSignals = warningSignals,
		BaseCollisionColor = palette.Collision,
		BaseInsetColor = palette.Inset,
		BaseAccentColor = palette.Accent,
		Definition = definition,
		State = StepRules.State.Active,
	}
end

local function setStepState(runtime: StepRuntime, nextState: string)
	if runtime.State == nextState then
		return
	end

	if nextState ~= StepRules.State.Active then
		assert(StepRules.canTransition(runtime.State, nextState), "illegal step-state transition")
	end

	runtime.State = nextState
	runtime.Model:SetAttribute("State", nextState)

	if nextState == StepRules.State.Active then
		runtime.Collision.CanCollide = true
		runtime.Collision.CanTouch = true
		runtime.Collision.Transparency = 0
		runtime.Collision.Color = runtime.BaseCollisionColor
		runtime.Inset.Transparency = 0
		runtime.Inset.Color = runtime.BaseInsetColor
		for _, part in runtime.Accents do
			part.Transparency = 0
			part.Color = runtime.BaseAccentColor
		end
		for _, part in runtime.Decorations do
			if
				not table.find(runtime.Accents, part)
				and not table.find(runtime.WarningSignals, part)
				and part ~= runtime.Inset
			then
				local baseTransparency = part:GetAttribute("BaseTransparency")
				part.Transparency = if type(baseTransparency) == "number" then baseTransparency else 0
			end
		end
		for _, signal in runtime.WarningSignals do
			signal.Transparency = 1
			signal.Color = COLORS.Warning
		end
	elseif nextState == StepRules.State.Warning then
		for _, part in runtime.Accents do
			part.Color = COLORS.Warning
		end
		for _, signal in runtime.WarningSignals do
			signal.Transparency = 0.04
		end
	elseif nextState == StepRules.State.Expiring then
		for _, part in runtime.Accents do
			part.Color = COLORS.Expiring
		end
		for _, signal in runtime.WarningSignals do
			signal.Color = COLORS.Expiring
		end
		for _, part in runtime.Decorations do
			TweenService:Create(part, TweenInfo.new(GameConfig.Step.ExpiryTransitionSeconds), {
				Transparency = math.max(part.Transparency, 0.55),
			}):Play()
		end
	elseif nextState == StepRules.State.Expired then
		runtime.Collision.CanCollide = false
		runtime.Collision.CanTouch = false
		runtime.Collision.Transparency = 1
		for _, part in runtime.Decorations do
			part.Transparency = 1
		end
	end
end

local function restoreStep(runtime: StepRuntime)
	runtime.State = StepRules.State.Expired
	setStepState(runtime, StepRules.State.Active)
end

local function scheduleExpiry(course: Course, stepId: number)
	local runtime = course.Steps[stepId]
	if not runtime or runtime.State ~= StepRules.State.Active then
		return
	end

	local token = course.RunToken
	local band = StepSequence.bandForHeight(stepId - 1)
	local warningSeconds = band.WarningSeconds
	local stabilityUntil = course.Player:GetAttribute("RisingStepsStabilityUntil")
	if type(stabilityUntil) == "number" and stabilityUntil > os.time() then
		course.Assisted = true
		warningSeconds *= 1.35
	end
	setStepState(runtime, StepRules.State.Warning)

	local remoteFolder = root:FindFirstChild("Remotes")
	local snapshotRemote = remoteFolder and remoteFolder:FindFirstChild("StateSnapshot")
	if snapshotRemote and snapshotRemote:IsA("RemoteEvent") then
		snapshotRemote:FireClient(course.Player, {
			Kind = "StepWarning",
			StepId = stepId,
			Height = course.Height,
			Streak = course.Streak,
			PB = PlayerProfileService.getPB(course.Player),
			Coins = course.Player:GetAttribute("RisingStepsCoins") or 0,
			Assisted = course.Assisted,
			Failed = course.Failed,
		})
	end

	task.delay(warningSeconds, function()
		if course.RunToken ~= token or runtime.State ~= StepRules.State.Warning then
			return
		end
		setStepState(runtime, StepRules.State.Expiring)

		task.delay(GameConfig.Step.ExpiryTransitionSeconds, function()
			if course.RunToken ~= token or runtime.State ~= StepRules.State.Expiring then
				return
			end
			setStepState(runtime, StepRules.State.Expired)
		end)
	end)
end

local function sendSnapshot(course: Course, kind: string, grade: string?, reward: any?)
	local remoteFolder = root:FindFirstChild("Remotes")
	if not remoteFolder then
		return
	end
	local snapshot = remoteFolder:FindFirstChild("StateSnapshot")
	if snapshot and snapshot:IsA("RemoteEvent") then
		snapshot:FireClient(course.Player, {
			Kind = kind,
			Height = course.Height,
			Streak = course.Streak,
			PB = PlayerProfileService.getPB(course.Player),
			Coins = course.Player:GetAttribute("RisingStepsCoins") or 0,
			Grade = grade,
			RewardCoins = if reward then reward.LandingCoins or 0 else 0,
			ChallengeCoins = if reward then reward.ChallengeCoins or 0 else 0,
			AchievementCoins = if reward then reward.AchievementCoins or 0 else 0,
			NewPB = if reward then reward.NewPB == true else false,
			UnlockedAchievements = if reward then reward.UnlockedAchievements or {} else {},
			Assisted = course.Assisted,
			Revives = course.Player:GetAttribute("RisingStepsRevives") or 0,
			RevivesUsed = course.RevivesUsed,
			Failed = course.Failed,
		})
	end
end

local function resetCourse(course: Course)
	course.RunToken += 1
	course.Height = 0
	course.Streak = 0
	course.Failed = false
	course.Assisted = false
	course.RevivesUsed = 0
	course.PendingRevive = false
	course.HighestY = WorldConfig.StartDeck.CenterY + WorldConfig.StartDeck.DeckThickness / 2
	course.Player:SetAttribute("RisingStepsHeight", 0)
	course.Player:SetAttribute("RisingStepsStreak", 0)
	course.Player:SetAttribute("RisingStepsRunFailed", false)

	for _, runtime in course.Steps do
		restoreStep(runtime)
	end

	sendSnapshot(course, "RunReset")
end

local function failCourse(course: Course, reason: string)
	if course.Failed or not course.Ready then
		return
	end

	course.Failed = true
	course.Player:SetAttribute("RisingStepsRunFailed", true)
	task.spawn(PlayerProfileService.flushPlayer, course.Player)
	sendSnapshot(course, reason)

	local character = course.Player.Character
	local humanoid = character and character:FindFirstChildOfClass("Humanoid")
	if humanoid and humanoid.Health > 0 then
		humanoid.Health = 0
	end
end

local function onLanding(course: Course, runtime: StepRuntime, hit: BasePart)
	if course.Failed or not course.Ready then
		return
	end

	local character = course.Player.Character
	if not character or not hit:IsDescendantOf(character) then
		return
	end

	local rootPart = character:FindFirstChild("HumanoidRootPart")
	if not rootPart or not rootPart:IsA("BasePart") then
		return
	end

	if
		not SecurityRules.componentsWithinLimit(
			rootPart.Position.X,
			rootPart.Position.Y,
			rootPart.Position.Z,
			SecurityConfig.MaxAbsPosition
		)
		or not SecurityRules.componentsWithinLimit(
			rootPart.AssemblyLinearVelocity.X,
			rootPart.AssemblyLinearVelocity.Y,
			rootPart.AssemblyLinearVelocity.Z,
			SecurityConfig.MaxAbsVelocity
		)
	then
		DiagnosticsService.warn("InvalidCharacterKinematics", {
			UserId = course.Player.UserId,
			Height = course.Height,
		})
		failCourse(course, "InvalidPosition")
		return
	end

	local now = os.clock()
	if
		course.Height > 0
		and not SecurityRules.intervalAllowed(course.LastLandingAt, now, SecurityConfig.MinimumLandingIntervalSeconds)
	then
		return
	end

	local definition = runtime.Definition
	local expectedStepId = course.Height + 1
	local stepTopY = runtime.Collision.Position.Y + runtime.Collision.Size.Y / 2
	if
		not StepRules.isExpectedLanding(
			expectedStepId,
			definition.Id,
			rootPart.Position.Y,
			stepTopY,
			rootPart.AssemblyLinearVelocity.Y
		)
	then
		return
	end

	local grade = ScoreRules.gradeLanding(
		rootPart.Position.X,
		rootPart.Position.Z,
		runtime.Collision.Position.X,
		runtime.Collision.Position.Z,
		GameConfig.Step.BaseFootprint,
		GameConfig.Score.PerfectToleranceRatio,
		GameConfig.Score.CleanToleranceRatio
	)

	course.Height = definition.Id
	course.LastLandingAt = now
	course.Streak = ScoreRules.nextStreak(course.Streak, grade)
	course.HighestY = math.max(course.HighestY, stepTopY)
	course.Player:SetAttribute("RisingStepsHeight", course.Height)
	course.Player:SetAttribute("RisingStepsStreak", course.Streak)
	local reward =
		PlayerProfileService.awardLanding(course.Player, grade, course.Streak, course.Height, not course.Assisted)
	sendSnapshot(course, "Progress", grade, reward)

	local expiryStepId = course.Height - CourseConfig.ExpiryBehindCurrent
	if expiryStepId >= 1 then
		scheduleExpiry(course, expiryStepId)
	end
end

local function laneIndexForJoin(): number
	if #freeLaneIndices > 0 then
		return table.remove(freeLaneIndices) :: number
	end

	local laneIndex = nextLaneIndex
	nextLaneIndex += 1
	return laneIndex
end

local function buildCourse(player: Player): Course
	local worldFolder = Workspace:FindFirstChild(WorldConfig.WorldFolderName)
	if not worldFolder then
		worldFolder = Instance.new("Folder")
		worldFolder.Name = WorldConfig.WorldFolderName
		worldFolder.Parent = Workspace
	end
	assert(worldFolder:IsA("Folder"), "RisingStepsWorld must be a Folder")

	local laneIndex = laneIndexForJoin()
	local laneOrigin = Vector3.new(laneIndex * CourseConfig.LaneSpacing, 0, 0)

	local courseModel = Instance.new("Model")
	courseModel.Name = ("Course_%d"):format(player.UserId)
	courseModel:SetAttribute("OwnerUserId", player.UserId)
	courseModel:SetAttribute("LaneIndex", laneIndex)
	courseModel.Parent = worldFolder

	local spawn = StartDeckService.create(courseModel, player, laneOrigin)
	player.RespawnLocation = spawn
	WorldArtService.createLane(courseModel, laneOrigin)

	local stepFolder = Instance.new("Folder")
	stepFolder.Name = "Steps"
	stepFolder.Parent = courseModel

	local startY = WorldConfig.StartDeck.CenterY + WorldConfig.StartDeck.DeckThickness / 2
	local seed = math.max(1, math.abs(player.UserId))
	local definitions =
		StepSequence.generate(seed, CourseConfig.InitialStepCount, startY, CourseConfig.VisualVariantCount)

	local course: Course = {
		Player = player,
		Model = courseModel,
		Spawn = spawn,
		LaneIndex = laneIndex,
		Origin = laneOrigin,
		Steps = {},
		Height = 0,
		Streak = 0,
		HighestY = startY,
		Failed = false,
		Ready = false,
		Assisted = false,
		RevivesUsed = 0,
		PendingRevive = false,
		LastLandingAt = -math.huge,
		RunToken = 1,
		CharacterDiedConnection = nil,
	}

	for _, definition in definitions do
		local runtime = buildStep(stepFolder, definition, laneOrigin)
		course.Steps[definition.Id] = runtime
		runtime.Collision.Touched:Connect(function(hit)
			if hit:IsA("BasePart") then
				onLanding(course, runtime, hit)
			end
		end)
	end

	local basePartCount = 0
	local beamAndLightCount = 0
	local stepPartCount = 0
	local startDeckPartCount = 0
	local environmentPartCount = 0
	local startDeck = courseModel:FindFirstChild("StartDeck")
	local environmentArt = courseModel:FindFirstChild("EnvironmentArt")
	for _, descendant in courseModel:GetDescendants() do
		if descendant:IsA("BasePart") then
			basePartCount += 1
			if descendant:IsDescendantOf(stepFolder) then
				stepPartCount += 1
			elseif startDeck and descendant:IsDescendantOf(startDeck) then
				startDeckPartCount += 1
			elseif environmentArt and descendant:IsDescendantOf(environmentArt) then
				environmentPartCount += 1
			end
		elseif descendant:IsA("Beam") or descendant:IsA("Light") then
			beamAndLightCount += 1
		end
	end
	courseModel:SetAttribute("BasePartCount", basePartCount)
	courseModel:SetAttribute("BeamAndLightCount", beamAndLightCount)
	if basePartCount > CourseConfig.MaxLaneBaseParts or beamAndLightCount > CourseConfig.MaxLaneBeamsAndLights then
		DiagnosticsService.warn("LanePresentationBudgetExceeded", {
			UserId = player.UserId,
			BaseParts = basePartCount,
			BasePartBudget = CourseConfig.MaxLaneBaseParts,
			BeamsAndLights = beamAndLightCount,
			BeamAndLightBudget = CourseConfig.MaxLaneBeamsAndLights,
			StepParts = stepPartCount,
			StartDeckParts = startDeckPartCount,
			EnvironmentParts = environmentPartCount,
			PresentationRevision = "2026-09-30-budget-breakdown-v1",
		})
	else
		DiagnosticsService.info("LanePresentationBudget", {
			UserId = player.UserId,
			BaseParts = basePartCount,
			BeamsAndLights = beamAndLightCount,
			StepParts = stepPartCount,
			StartDeckParts = startDeckPartCount,
			EnvironmentParts = environmentPartCount,
			PresentationRevision = "2026-09-30-budget-breakdown-v1",
		})
	end

	courses[player] = course
	resetCourse(course)
	return course
end

local function bindCharacter(course: Course, character: Model)
	course.Ready = false
	if course.CharacterDiedConnection then
		course.CharacterDiedConnection:Disconnect()
		course.CharacterDiedConnection = nil
	end

	if course.PendingRevive then
		course.PendingRevive = false
		task.defer(function()
			local safeStep = course.Steps[math.max(1, course.Height)]
			if character.Parent and safeStep then
				character:PivotTo(safeStep.Collision.CFrame + Vector3.new(0, 4, 0))
			end
			sendSnapshot(course, "Revived")
		end)
	elseif course.Failed then
		task.delay(CourseConfig.RespawnResetDelaySeconds, function()
			if courses[course.Player] == course then
				resetCourse(course)
			end
		end)
	end

	local humanoid = character:WaitForChild("Humanoid", 10)
	if humanoid and humanoid:IsA("Humanoid") then
		course.CharacterDiedConnection = humanoid.Died:Connect(function()
			failCourse(course, "HumanoidDied")
		end)
	end

	task.delay(0.20, function()
		if courses[course.Player] == course and character.Parent and not course.Failed then
			course.Ready = true
		end
	end)
end

local function bindPlayer(player: Player)
	local course = buildCourse(player)

	if player.Character then
		local character = player.Character
		task.defer(function()
			if character.Parent and courses[player] == course then
				character:PivotTo(course.Spawn.CFrame)
				bindCharacter(course, character)
			end
		end)
	end

	player.CharacterAdded:Connect(function(character)
		bindCharacter(course, character)
	end)
end

local function unbindPlayer(player: Player)
	local course = courses[player]
	if not course then
		return
	end

	if course.CharacterDiedConnection then
		course.CharacterDiedConnection:Disconnect()
	end
	course.Model:Destroy()
	table.insert(freeLaneIndices, course.LaneIndex)
	courses[player] = nil
	lastPlayerCommandAt[player] = nil
end

local function checkFalls()
	for player, course in courses do
		if course.Failed then
			continue
		end

		local character = player.Character
		local rootPart = character and character:FindFirstChild("HumanoidRootPart")
		if rootPart and rootPart:IsA("BasePart") then
			local failY = course.HighestY - GameConfig.Failure.FallDistanceBelowHighestValidated
			if rootPart.Position.Y < failY then
				failCourse(course, "FallBoundary")
			end
		end
	end
end


local function requireStudioCourse(player: Player): Course
	assert(RunService:IsStudio(), "QA helpers are Studio-only")
	local course = courses[player]
	assert(course, "QA course unavailable")
	return course
end

function StepCourseService.qaSnapshot(player: Player)
	local course = requireStudioCourse(player)
	local states = {}
	for stepId, runtime in course.Steps do
		states[stepId] = runtime.State
	end
	return {
		Height = course.Height,
		Streak = course.Streak,
		Failed = course.Failed,
		Ready = course.Ready,
		Assisted = course.Assisted,
		RevivesUsed = course.RevivesUsed,
		RunToken = course.RunToken,
		StepStates = states,
		BaseParts = course.Model:GetAttribute("BasePartCount"),
		BeamsAndLights = course.Model:GetAttribute("BeamAndLightCount"),
	}
end

function StepCourseService.qaLand(player: Player, stepId: number, landingOffsetRatio: number?): string
	local course = requireStudioCourse(player)
	local runtime = course.Steps[stepId]
	assert(runtime, ("QA step %d unavailable"):format(stepId))
	assert(course.Ready and not course.Failed, "QA course not ready")

	local character = player.Character
	assert(character, "QA character unavailable")
	local rootPart = character:FindFirstChild("HumanoidRootPart")
	assert(rootPart and rootPart:IsA("BasePart"), "QA root unavailable")

	local ratio = math.clamp(landingOffsetRatio or 0, -0.48, 0.48)
	local topY = runtime.Collision.Position.Y + runtime.Collision.Size.Y / 2
	rootPart.AssemblyLinearVelocity = Vector3.new(0, -1, 0)
	rootPart.CFrame = CFrame.new(
		runtime.Collision.Position.X + GameConfig.Step.BaseFootprint * ratio,
		topY + 2.9,
		runtime.Collision.Position.Z
	)
	course.LastLandingAt = -math.huge
	onLanding(course, runtime, rootPart)

	return if course.Height == stepId then ScoreRules.gradeLanding(
		rootPart.Position.X,
		rootPart.Position.Z,
		runtime.Collision.Position.X,
		runtime.Collision.Position.Z,
		GameConfig.Step.BaseFootprint,
		GameConfig.Score.PerfectToleranceRatio,
		GameConfig.Score.CleanToleranceRatio
	) else "Rejected"
end

function StepCourseService.qaFail(player: Player, reason: string?)
	local course = requireStudioCourse(player)
	failCourse(course, reason or "StudioQA")
end

function StepCourseService.qaRetry(player: Player)
	local course = requireStudioCourse(player)
	assert(course.Failed, "QA retry requires failed course")
	course.RunToken += 1
	player:LoadCharacter()
end

function StepCourseService.qaUseRevive(player: Player): boolean
	local course = requireStudioCourse(player)
	if not course.Failed or course.RevivesUsed >= GameConfig.Failure.MaxPaidRevivesPerRun or course.Height < 1 then
		return false
	end
	if not PlayerProfileService.consumeRevive(player) then
		return false
	end
	course.RunToken += 1
	course.Failed = false
	course.Streak = 0
	course.Assisted = true
	course.RevivesUsed += 1
	course.PendingRevive = true
	player:SetAttribute("RisingStepsRunFailed", false)
	player:LoadCharacter()
	return true
end

function StepCourseService.qaSetInvalidPosition(player: Player)
	local course = requireStudioCourse(player)
	local character = player.Character
	local rootPart = character and character:FindFirstChild("HumanoidRootPart")
	assert(rootPart and rootPart:IsA("BasePart"), "QA root unavailable")
	rootPart.CFrame = CFrame.new(SecurityConfig.MaxAbsPosition * 2, rootPart.Position.Y, rootPart.Position.Z)
	checkFalls()
	if not course.Failed then
		local nextStep = course.Steps[course.Height + 1]
		if nextStep then
			onLanding(course, nextStep, rootPart)
		end
	end
end

function StepCourseService.qaReset(player: Player)
	local course = requireStudioCourse(player)
	resetCourse(course)
	local character = player.Character
	if character then
		character:PivotTo(course.Spawn.CFrame)
	end
	task.delay(0.21, function()
		if courses[player] == course and not course.Failed then
			course.Ready = true
		end
	end)
end

function StepCourseService.qaForceStepState(player: Player, stepId: number, state: string)
	local course = requireStudioCourse(player)
	local runtime = course.Steps[stepId]
	assert(runtime, "QA step unavailable")
	setStepState(runtime, state)
end

function StepCourseService.start()
	local remoteFolder = root:WaitForChild("Remotes")
	local playerCommand = remoteFolder:WaitForChild("PlayerCommand")
	assert(playerCommand:IsA("RemoteEvent"), "PlayerCommand must be a RemoteEvent")
	playerCommand.OnServerEvent:Connect(function(player, payload)
		local now = os.clock()
		local previous = lastPlayerCommandAt[player] or -math.huge
		if not SecurityRules.intervalAllowed(previous, now, SecurityConfig.RemoteIntervals.PlayerCommand) then
			return
		end
		lastPlayerCommandAt[player] = now

		if type(payload) ~= "table" then
			return
		end
		local course = courses[player]
		if not course or not course.Failed then
			return
		end

		if payload.Action == "Retry" then
			course.RunToken += 1
			player:LoadCharacter()
		elseif payload.Action == "UseRevive" then
			if course.RevivesUsed >= GameConfig.Failure.MaxPaidRevivesPerRun or course.Height < 1 then
				return
			end
			if not PlayerProfileService.consumeRevive(player) then
				return
			end
			course.RunToken += 1
			course.Failed = false
			course.Streak = 0
			course.Assisted = true
			course.RevivesUsed += 1
			course.PendingRevive = true
			player:SetAttribute("RisingStepsRunFailed", false)
			player:LoadCharacter()
		end
	end)

	for _, player in Players:GetPlayers() do
		bindPlayer(player)
	end

	Players.PlayerAdded:Connect(bindPlayer)
	Players.PlayerRemoving:Connect(unbindPlayer)

	RunService.Heartbeat:Connect(function(deltaTime)
		fallAccumulator += deltaTime
		if fallAccumulator < CourseConfig.FallCheckIntervalSeconds then
			return
		end
		fallAccumulator = 0
		checkFalls()
	end)
end

return StepCourseService
