--!strict

local Lighting = game:GetService("Lighting")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local GraphicsQualityRules = require(root.rules.GraphicsQualityRules)

local GraphicsQualityController = {}

local LOCAL_PLAYER = Players.LocalPlayer
local userGameSettings = UserSettings():GetService("UserGameSettings") :: any

local currentLow = false

local function savedQualityValue(): number
	local setting = userGameSettings.SavedQualityLevel
	local value = (setting :: any).Value
	if type(value) == "number" then
		return value
	end
	return 0
end

local function isEnvironmentArt(instance: Instance): boolean
	local current: Instance? = instance
	while current do
		if current.Name == "EnvironmentArt" then
			return true
		end
		current = current.Parent
	end
	return false
end

local function applyInstance(instance: Instance, profile)
	if instance:IsA("BloomEffect") then
		instance.Enabled = profile.BloomEnabled
		return
	end

	if not isEnvironmentArt(instance) then
		return
	end

	if instance:IsA("Beam") then
		instance.Enabled = profile.DecorativeBeamsEnabled
	elseif instance:IsA("PointLight") then
		instance.Enabled = profile.DecorativePointLightsEnabled
	end
end

local function apply()
	local profile = GraphicsQualityRules.profile(savedQualityValue())
	currentLow = profile.Low
	LOCAL_PLAYER:SetAttribute("RisingStepsLowGraphics", currentLow)

	for _, instance in Lighting:GetDescendants() do
		applyInstance(instance, profile)
	end

	for _, instance in Workspace:GetDescendants() do
		applyInstance(instance, profile)
	end
end

function GraphicsQualityController.start()
	apply()

	userGameSettings:GetPropertyChangedSignal("SavedQualityLevel"):Connect(apply)

	Lighting.DescendantAdded:Connect(function(instance)
		local profile = GraphicsQualityRules.profile(savedQualityValue())
		applyInstance(instance, profile)
	end)

	Workspace.DescendantAdded:Connect(function(instance)
		local profile = GraphicsQualityRules.profile(savedQualityValue())
		applyInstance(instance, profile)
	end)
end

return GraphicsQualityController
