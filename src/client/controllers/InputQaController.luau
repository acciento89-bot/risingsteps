--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local UserInputService = game:GetService("UserInputService")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local RuntimeQaConfig = require(root.config.RuntimeQaConfig)

local InputQaController = {}
local LOCAL_PLAYER = Players.LocalPlayer

local function emit(name: string, passed: boolean, detail: string?)
	print(("[RisingStepsInputQA] %s %s%s"):format(
		if passed then "PASS" else "FAIL",
		name,
		if detail then " " .. detail else ""
	))
end

local function buttonCenter(button: GuiObject): Vector2
	return button.AbsolutePosition + button.AbsoluteSize / 2
end

local function clickButton(virtualInput: any, button: GuiButton)
	local center = buttonCenter(button)
	virtualInput:SendMousePosition(center)
	task.wait(0.05)
	virtualInput:SendMouseButton(center, Enum.UserInputType.MouseButton1, true, 1)
	task.wait(0.05)
	virtualInput:SendMouseButton(center, Enum.UserInputType.MouseButton1, false, 1)
end

local function run()
	task.wait(RuntimeQaConfig.InputQaStartupDelaySeconds)

	local virtualInput = UserInputService:CreateVirtualInput()
	if not virtualInput then
		emit("virtual_input_available", false)
		LOCAL_PLAYER:SetAttribute("RisingStepsInputQaComplete", true)
		return
	end
	emit("virtual_input_available", true)

	local character = LOCAL_PLAYER.Character or LOCAL_PLAYER.CharacterAdded:Wait()
	local rootPart = character:WaitForChild("HumanoidRootPart", 8)
	local humanoid = character:WaitForChild("Humanoid", 8)
	if not rootPart or not rootPart:IsA("BasePart") or not humanoid or not humanoid:IsA("Humanoid") then
		emit("character_ready", false)
		LOCAL_PLAYER:SetAttribute("RisingStepsInputQaComplete", true)
		return
	end
	emit("character_ready", true)

	local start = rootPart.Position
	virtualInput:SendKey(true, Enum.KeyCode.W, false)
	task.wait(0.70)
	virtualInput:SendKey(false, Enum.KeyCode.W, false)
	task.wait(0.15)
	local moved = Vector3.new(rootPart.Position.X - start.X, 0, rootPart.Position.Z - start.Z).Magnitude
	emit("keyboard_move", moved >= 1.0, ("distance=%.2f"):format(moved))
	LOCAL_PLAYER:SetAttribute("RisingStepsInputQaKeyboardMove", moved >= 1.0)

	local beforeJumpY = rootPart.Position.Y
	virtualInput:SendKey(true, Enum.KeyCode.Space, false)
	task.wait(0.06)
	virtualInput:SendKey(false, Enum.KeyCode.Space, false)
	local peakY = rootPart.Position.Y
	local deadline = os.clock() + 0.75
	repeat
		peakY = math.max(peakY, rootPart.Position.Y)
		task.wait(0.03)
	until os.clock() >= deadline
	local jumped = peakY > beforeJumpY + 0.75
	emit("keyboard_jump", jumped, ("rise=%.2f"):format(peakY - beforeJumpY))
	LOCAL_PLAYER:SetAttribute("RisingStepsInputQaKeyboardJump", jumped)

	local playerGui = LOCAL_PLAYER:WaitForChild("PlayerGui")
	local hud = playerGui:WaitForChild("RisingStepsHUD", 8)
	if hud and hud:IsA("ScreenGui") then
		local topRight = hud:FindFirstChild("TopRight")
		local shopButton = topRight and topRight:FindFirstChild("ShopButton")
		local shopPanel = hud:FindFirstChild("ShopPanel")
		if shopButton and shopButton:IsA("GuiButton") and shopPanel and shopPanel:IsA("GuiObject") then
			clickButton(virtualInput, shopButton)
			task.wait(0.25)
			local opened = shopPanel.Visible
			emit("mouse_shop_open", opened)
			LOCAL_PLAYER:SetAttribute("RisingStepsInputQaMouseUi", opened)

			local closeButton = shopPanel:FindFirstChild("CloseButton", true)
			if closeButton and closeButton:IsA("GuiButton") then
				clickButton(virtualInput, closeButton)
				task.wait(0.25)
				emit("mouse_shop_close", not shopPanel.Visible)
			end
		else
			emit("mouse_shop_open", false, "shop controls unavailable")
		end
	else
		emit("hud_ready", false)
	end

	local keyboardPass = LOCAL_PLAYER:GetAttribute("RisingStepsInputQaKeyboardMove") == true
		and LOCAL_PLAYER:GetAttribute("RisingStepsInputQaKeyboardJump") == true
	local mousePass = LOCAL_PLAYER:GetAttribute("RisingStepsInputQaMouseUi") == true
	LOCAL_PLAYER:SetAttribute("RisingStepsInputQaPass", keyboardPass and mousePass)
	LOCAL_PLAYER:SetAttribute("RisingStepsInputQaComplete", true)
	print(("[RisingStepsInputQA] COMPLETE keyboard=%s mouse=%s"):format(tostring(keyboardPass), tostring(mousePass)))
end

function InputQaController.start()
	if not RunService:IsStudio() or not RuntimeQaConfig.InputQaEnabled then
		return
	end
	task.spawn(run)
end

return InputQaController
