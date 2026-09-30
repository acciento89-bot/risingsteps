--!strict

local Players = game:GetService("Players")
local RunService = game:GetService("RunService")

local CharacterMotionController = {}

local LOCAL_PLAYER = Players.LocalPlayer
local renderConnection: RBXScriptConnection? = nil
local stateConnection: RBXScriptConnection? = nil
local activeMotor: Motor6D? = nil
local currentTransform = CFrame.identity
local targetTransform = CFrame.identity
local landingImpulse = 0
local wasFreefall = false

local function reducedMotion(): boolean
	return LOCAL_PLAYER:GetAttribute("RisingStepsReducedMotion") == true
end

local function findPresentationMotor(character: Model): Motor6D?
	local lowerTorso = character:FindFirstChild("LowerTorso")
	if lowerTorso then
		local waist = lowerTorso:FindFirstChild("Waist")
		if waist and waist:IsA("Motor6D") then
			return waist
		end
	end

	local rootPart = character:FindFirstChild("HumanoidRootPart")
	if rootPart then
		local rootJoint = rootPart:FindFirstChild("RootJoint")
		if rootJoint and rootJoint:IsA("Motor6D") then
			return rootJoint
		end
	end

	local torso = character:FindFirstChild("Torso")
	if torso then
		local rootJoint = torso:FindFirstChild("RootJoint")
		if rootJoint and rootJoint:IsA("Motor6D") then
			return rootJoint
		end
	end

	return nil
end

local function clearPresentation()
	if activeMotor then
		activeMotor.Transform = CFrame.identity
	end
	activeMotor = nil
	currentTransform = CFrame.identity
	targetTransform = CFrame.identity
	landingImpulse = 0
	wasFreefall = false
end

local function bindCharacter(character: Model)
	clearPresentation()

	local humanoid = character:WaitForChild("Humanoid", 10)
	local rootPart = character:WaitForChild("HumanoidRootPart", 10)
	if not humanoid or not humanoid:IsA("Humanoid") or not rootPart or not rootPart:IsA("BasePart") then
		return
	end

	activeMotor = findPresentationMotor(character)
	if not activeMotor then
		return
	end

	stateConnection = humanoid.StateChanged:Connect(function(_, newState)
		if newState == Enum.HumanoidStateType.Freefall then
			wasFreefall = true
		elseif newState == Enum.HumanoidStateType.Landed then
			if wasFreefall then
				local verticalSpeed = math.abs(rootPart.AssemblyLinearVelocity.Y)
				landingImpulse = math.clamp(0.16 + verticalSpeed / 90, 0.16, 0.34)
			end
			wasFreefall = false
		elseif newState == Enum.HumanoidStateType.Running
			or newState == Enum.HumanoidStateType.RunningNoPhysics
			or newState == Enum.HumanoidStateType.Jumping
		then
			if newState ~= Enum.HumanoidStateType.Jumping then
				wasFreefall = false
			end
		end
	end)

	if renderConnection then
		renderConnection:Disconnect()
	end

	renderConnection = RunService.RenderStepped:Connect(function(deltaTime)
		if not character.Parent or LOCAL_PLAYER.Character ~= character or not activeMotor then
			return
		end

		if reducedMotion() then
			targetTransform = CFrame.identity
		else
			local velocity = rootPart.AssemblyLinearVelocity
			local horizontalSpeed = Vector3.new(velocity.X, 0, velocity.Z).Magnitude
			local verticalSpeed = velocity.Y

			local lean = math.clamp(horizontalSpeed / 44, 0, 1) * math.rad(6)
			local pitch = 0
			local yOffset = 0

			if verticalSpeed > 4 then
				pitch = -math.rad(4.5)
			elseif verticalSpeed < -5 then
				pitch = math.rad(5.5)
			elseif horizontalSpeed > 1 then
				pitch = -lean
			end

			if landingImpulse > 0.001 then
				local squash = landingImpulse
				yOffset = -0.24 * squash / 0.34
				pitch += math.rad(5) * squash / 0.34
				landingImpulse *= math.exp(-15 * math.max(deltaTime, 0))
			else
				landingImpulse = 0
			end

			targetTransform = CFrame.new(0, yOffset, 0) * CFrame.Angles(pitch, 0, 0)
		end

		local response = if landingImpulse > 0 then 18 else 10
		local alpha = 1 - math.exp(-response * math.max(deltaTime, 0))
		currentTransform = currentTransform:Lerp(targetTransform, alpha)
		activeMotor.Transform = currentTransform
	end)
end

function CharacterMotionController.start()
	if LOCAL_PLAYER.Character then
		task.spawn(bindCharacter, LOCAL_PLAYER.Character)
	end

	LOCAL_PLAYER.CharacterAdded:Connect(bindCharacter)
	LOCAL_PLAYER.CharacterRemoving:Connect(function(character)
		if LOCAL_PLAYER.Character == character or not LOCAL_PLAYER.Character then
			clearPresentation()
		end
	end)
end

return CharacterMotionController
