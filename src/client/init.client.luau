--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local RemoteConfig = require(root.config.RemoteConfig)
local remoteFolder = root:WaitForChild(RemoteConfig.FolderName)

local controllersFolder = script:WaitForChild("controllers")
local CameraController = require(controllersFolder.CameraController)
local CharacterMotionController = require(controllersFolder.CharacterMotionController)
local CosmeticPresentationController = require(controllersFolder.CosmeticPresentationController)
local CourseVisibilityController = require(controllersFolder.CourseVisibilityController)
local FeedbackController = require(controllersFolder.FeedbackController)
local HudController = require(controllersFolder.HudController)

for remoteName in RemoteConfig.Definitions do
	remoteFolder:WaitForChild(remoteName)
end

CameraController.start()
CharacterMotionController.start()
CosmeticPresentationController.start()
CourseVisibilityController.start()
FeedbackController.start()
HudController.start()
