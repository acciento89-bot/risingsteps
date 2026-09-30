--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local RemoteConfig = require(root.config.RemoteConfig)
local remoteFolder = root:WaitForChild(RemoteConfig.FolderName)

for remoteName in RemoteConfig.Definitions do
	remoteFolder:WaitForChild(remoteName)
end
