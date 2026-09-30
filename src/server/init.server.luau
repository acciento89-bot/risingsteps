--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local RemoteConfig = require(root.config.RemoteConfig)

local servicesFolder = script:WaitForChild("services")
local StartDeckService = require(servicesFolder.StartDeckService)
local PlayerMovementService = require(servicesFolder.PlayerMovementService)

StartDeckService.start()
PlayerMovementService.start()

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
