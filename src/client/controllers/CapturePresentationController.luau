--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local StarterGui = game:GetService("StarterGui")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local RuntimeQaConfig = require(root.config.RuntimeQaConfig)

local CapturePresentationController = {}

local function hideCoreUi()
	for _, coreGuiType in Enum.CoreGuiType:GetEnumItems() do
		pcall(function()
			StarterGui:SetCoreGuiEnabled(coreGuiType, false)
		end)
	end

	-- TopbarEnabled can register a little later than the individual CoreGui
	-- services, so this is intentionally retried by the caller.
	pcall(function()
		StarterGui:SetCore("TopbarEnabled", false)
	end)
end

function CapturePresentationController.start()
	if not RunService:IsStudio() then
		return
	end
	if not RuntimeQaConfig.EnabledInStudio or not RuntimeQaConfig.VisualOnly then
		return
	end

	-- Store/QA captures should contain only shipped game presentation.
	-- This path is Studio + VisualOnly gated and never runs for normal players.
	task.spawn(function()
		for _ = 1, 10 do
			hideCoreUi()
			task.wait(0.2)
		end
	end)
end

return CapturePresentationController
