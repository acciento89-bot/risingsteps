--!strict

local Debris = game:GetService("Debris")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local TweenService = game:GetService("TweenService")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local AudioConfig = require(root.config.AudioConfig)
local WorldConfig = require(root.config.WorldConfig)

local FeedbackController = {}

local LOCAL_PLAYER = Players.LocalPlayer
local transientSounds = 0
local lastCueAt: { [string]: number } = {}

local function hex(hexColor: string): Color3
	local value = hexColor:gsub("#", "")
	local r = tonumber(value:sub(1, 2), 16)
	local g = tonumber(value:sub(3, 4), 16)
	local b = tonumber(value:sub(5, 6), 16)
	assert(r and g and b, "invalid feedback hex")
	return Color3.fromRGB(r, g, b)
end

local COLORS = {
	Active = hex(WorldConfig.Palette.GameplayAccent),
	Warm = hex(WorldConfig.Palette.WarmAccent),
	Warning = Color3.fromRGB(255, 159, 67),
	Fail = Color3.fromRGB(255, 93, 108),
	Perfect = Color3.fromRGB(236, 255, 255),
}

local function reducedMotion(): boolean
	return LOCAL_PLAYER:GetAttribute("RisingStepsReducedMotion") == true
end

local function playTone(cue: string, pitch: number, volume: number)
	if transientSounds >= AudioConfig.MaxConcurrentTransientSounds then
		return
	end

	local minInterval = AudioConfig.CueMinIntervalSeconds[cue]
	local now = os.clock()
	local lastAt = lastCueAt[cue]
	if type(minInterval) == "number" and lastAt and now - lastAt < minInterval then
		return
	end

	local camera = Workspace.CurrentCamera
	if not camera then
		return
	end

	lastCueAt[cue] = now
	transientSounds += 1
	local sound = Instance.new("Sound")
	sound.Name = "RisingStepsTransient_" .. cue
	sound.SoundId = AudioConfig.Source
	sound.Volume = math.min(volume, AudioConfig.MaxSingleTransientVolume)
	sound.PlaybackSpeed = pitch
	sound.RollOffMaxDistance = 80
	sound.Parent = camera
	sound.Ended:Connect(function()
		transientSounds = math.max(0, transientSounds - 1)
	end)
	Debris:AddItem(sound, 3)
	sound:Play()
end

local function currentRoot(): BasePart?
	local character = LOCAL_PLAYER.Character
	local rootPart = character and character:FindFirstChild("HumanoidRootPart")
	if rootPart and rootPart:IsA("BasePart") then
		return rootPart
	end
	return nil
end

local function makeRing(color: Color3, scale: number, lifetime: number)
	if reducedMotion() then
		return
	end

	local rootPart = currentRoot()
	if not rootPart then
		return
	end

	local ring = Instance.new("Part")
	ring.Name = "LandingRing"
	ring.Shape = Enum.PartType.Cylinder
	ring.Size = Vector3.new(0.08, 2.4, 2.4)
	ring.CFrame = CFrame.new(rootPart.Position - Vector3.new(0, 2.8, 0)) * CFrame.Angles(0, 0, math.rad(90))
	ring.Anchored = true
	ring.CanCollide = false
	ring.CanTouch = false
	ring.CanQuery = false
	ring.CastShadow = false
	ring.Material = Enum.Material.Neon
	ring.Color = color
	ring.Transparency = 0.18
	ring.Parent = Workspace

	TweenService:Create(ring, TweenInfo.new(lifetime, Enum.EasingStyle.Quint, Enum.EasingDirection.Out), {
		Size = Vector3.new(0.04, scale, scale),
		Transparency = 1,
	}):Play()
	Debris:AddItem(ring, lifetime + 0.1)
end

local function sparkBurst(color: Color3, count: number)
	if reducedMotion() then
		return
	end

	local rootPart = currentRoot()
	if not rootPart then
		return
	end

	local cappedCount = math.clamp(count, 3, 10)
	for index = 1, cappedCount do
		local angle = index / cappedCount * math.pi * 2
		local shard = Instance.new("Part")
		shard.Name = "LandingSpark"
		shard.Size = Vector3.new(0.10, 0.42, 0.10)
		shard.CFrame = CFrame.new(rootPart.Position - Vector3.new(0, 2.4, 0))
		shard.Anchored = false
		shard.CanCollide = false
		shard.CanTouch = false
		shard.CanQuery = false
		shard.CastShadow = false
		shard.Material = Enum.Material.Neon
		shard.Color = color
		shard.AssemblyLinearVelocity = Vector3.new(math.cos(angle) * 4.5, 5.5 + (index % 3), math.sin(angle) * 4.5)
		shard.Parent = Workspace
		Debris:AddItem(shard, 0.8)
	end
end

local function findStep(stepId: number): Model?
	local world = Workspace:FindFirstChild(WorldConfig.WorldFolderName)
	if not world then
		return nil
	end

	local course = world:FindFirstChild(("Course_%d"):format(LOCAL_PLAYER.UserId))
	local steps = course and course:FindFirstChild("Steps")
	local step = steps and steps:FindFirstChild(("Step_%03d"):format(stepId))
	if step and step:IsA("Model") then
		return step
	end
	return nil
end

local function highlightWarningStep(stepId: number)
	local step = findStep(stepId)
	if not step then
		return
	end

	local highlight = Instance.new("Highlight")
	highlight.Name = "WarningPulse"
	highlight.Adornee = step
	highlight.DepthMode = Enum.HighlightDepthMode.Occluded
	highlight.FillTransparency = 1
	highlight.OutlineColor = COLORS.Warning
	highlight.OutlineTransparency = 0.08
	highlight.Parent = step

	if not reducedMotion() then
		TweenService:Create(highlight, TweenInfo.new(0.62, Enum.EasingStyle.Quad), {
			OutlineTransparency = 1,
		}):Play()
	end
	Debris:AddItem(highlight, 0.75)
end

local function cameraFailureKick()
	if reducedMotion() then
		return
	end

	local camera = Workspace.CurrentCamera
	if not camera then
		return
	end

	local original = camera.FieldOfView
	TweenService:Create(camera, TweenInfo.new(0.10, Enum.EasingStyle.Quad), {
		FieldOfView = original + 3,
	}):Play()
	task.delay(0.12, function()
		if camera.Parent then
			TweenService:Create(camera, TweenInfo.new(0.26, Enum.EasingStyle.Quint), {
				FieldOfView = original,
			}):Play()
		end
	end)
end

local function onSnapshot(snapshot: any)
	if type(snapshot) ~= "table" then
		return
	end

	if snapshot.Kind == "Progress" then
		local grade = snapshot.Grade
		local streak = if type(snapshot.Streak) == "number" then snapshot.Streak else 0
		local landingEffect = LOCAL_PLAYER:GetAttribute("RisingStepsLandingEffect")
		local effectId = if type(landingEffect) == "string" then landingEffect else "basic"

		if grade == "Perfect" then
			playTone("Landing", AudioConfig.Landing.PerfectPitch, AudioConfig.Landing.BaseVolume)
			if effectId == "spark" then
				makeRing(COLORS.Warm, 5.8, 0.30)
				sparkBurst(COLORS.Warm, 8 + math.floor(streak / 4))
			elseif effectId == "pulse" then
				makeRing(COLORS.Active, 8.4, 0.50)
				makeRing(COLORS.Perfect, 6.5, 0.38)
				makeRing(COLORS.Active, 4.4, 0.28)
			else
				makeRing(COLORS.Perfect, 7.2, 0.44)
				makeRing(COLORS.Active, 5.4, 0.34)
			end
			if streak >= 5 and effectId ~= "spark" then
				sparkBurst(COLORS.Active, 4 + math.floor(streak / 5))
			end
		elseif grade == "Clean" then
			playTone("Landing", AudioConfig.Landing.CleanPitch, AudioConfig.Landing.BaseVolume * 0.85)
			if effectId == "spark" then
				sparkBurst(COLORS.Warm, 5)
				makeRing(COLORS.Warm, 4.5, 0.28)
			elseif effectId == "pulse" then
				makeRing(COLORS.Active, 5.8, 0.38)
				makeRing(COLORS.Perfect, 4.2, 0.27)
			else
				makeRing(COLORS.Warm, 5.0, 0.34)
			end
		end

		if
			snapshot.NewPB == true or (type(snapshot.AchievementCoins) == "number" and snapshot.AchievementCoins > 0)
		then
			task.delay(0.10, function()
				playTone("Celebration", AudioConfig.Celebration.Pitch, AudioConfig.Celebration.Volume)
			end)
		end
	elseif snapshot.Kind == "StepWarning" then
		playTone("Warning", AudioConfig.Warning.Pitch, AudioConfig.Warning.Volume)
		if type(snapshot.StepId) == "number" then
			highlightWarningStep(snapshot.StepId)
		end
	elseif snapshot.Kind == "FallBoundary" or snapshot.Kind == "HumanoidDied" then
		playTone("Failure", AudioConfig.Failure.Pitch, AudioConfig.Failure.Volume)
		cameraFailureKick()
	elseif snapshot.Kind == "Revived" then
		playTone("Celebration", AudioConfig.Celebration.Pitch, AudioConfig.Celebration.Volume)
		makeRing(COLORS.Active, 8.0, 0.48)
	end
end

function FeedbackController.start()
	local remoteFolder = root:WaitForChild("Remotes")
	local stateSnapshot = remoteFolder:WaitForChild("StateSnapshot")
	assert(stateSnapshot:IsA("RemoteEvent"), "StateSnapshot must be a RemoteEvent")
	stateSnapshot.OnClientEvent:Connect(onSnapshot)
end

return FeedbackController
