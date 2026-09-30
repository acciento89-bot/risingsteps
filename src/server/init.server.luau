--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local RemoteConfig = require(root.config.RemoteConfig)

local remoteFolder = root:FindFirstChild(RemoteConfig.FolderName)
if not remoteFolder then
	remoteFolder = Instance.new("Folder")
	remoteFolder.Name = RemoteConfig.FolderName
	remoteFolder.Parent = root
end

for remoteName, definition in RemoteConfig.Definitions do
	local existing = remoteFolder:FindFirstChild(remoteName)
	if not existing then
		local remote = Instance.new(definition.ClassName)
		remote.Name = remoteName
		remote:SetAttribute("Direction", definition.Direction)
		remote.Parent = remoteFolder
	end
end

local servicesFolder = script:WaitForChild("services")
local MonetizationService = require(servicesFolder.MonetizationService)
local PlayerMovementService = require(servicesFolder.PlayerMovementService)
local PlayerProfileService = require(servicesFolder.PlayerProfileService)
local ProgressionService = require(servicesFolder.ProgressionService)
local RuntimeQaService = require(servicesFolder.RuntimeQaService)
local StepCourseService = require(servicesFolder.StepCourseService)

PlayerProfileService.start()
MonetizationService.start()
ProgressionService.start(remoteFolder)
PlayerMovementService.start()
StepCourseService.start()
RuntimeQaService.start()
