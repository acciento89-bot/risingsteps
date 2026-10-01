--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local RemoteConfig = require(root.config.RemoteConfig)
local remoteFolder = root:WaitForChild(RemoteConfig.FolderName)

local controllersFolder = script:WaitForChild("controllers")
local CameraController = require(controllersFolder.CameraController)
local CapturePresentationController = require(controllersFolder.CapturePresentationController)
local CharacterMotionController = require(controllersFolder.CharacterMotionController)
local CosmeticPresentationController = require(controllersFolder.CosmeticPresentationController)
local CourseVisibilityController = require(controllersFolder.CourseVisibilityController)
local FeedbackController = require(controllersFolder.FeedbackController)
local GraphicsQualityController = require(controllersFolder.GraphicsQualityController)
local HudController = require(controllersFolder.HudController)
local InputQaController = require(controllersFolder.InputQaController)

for remoteName in RemoteConfig.Definitions do
	remoteFolder:WaitForChild(remoteName)
end

CameraController.start()
CapturePresentationController.start()
CharacterMotionController.start()
CosmeticPresentationController.start()
CourseVisibilityController.start()
FeedbackController.start()
GraphicsQualityController.start()
HudController.start()
InputQaController.start()
