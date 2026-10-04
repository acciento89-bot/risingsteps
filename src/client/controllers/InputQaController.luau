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
	print(
		("[RisingStepsInputQA] %s %s%s"):format(
			if passed then "PASS" else "FAIL",
			name,
			if detail then " " .. detail else ""
		)
	)
end

local function buttonCenter(button: GuiObject): Vector2
	-- AbsolutePosition is already expressed in screen coordinates for a ScreenGui
	-- with IgnoreGuiInset=false. Adding GetGuiInset() again double-offsets clicks.
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

local function getCharacterParts(): (BasePart?, Humanoid?)
	local character = LOCAL_PLAYER.Character or LOCAL_PLAYER.CharacterAdded:Wait()
	local rootPart = character:WaitForChild("HumanoidRootPart", 8)
	local humanoid = character:WaitForChild("Humanoid", 8)
	if not rootPart or not rootPart:IsA("BasePart") or not humanoid or not humanoid:IsA("Humanoid") then
		return nil, nil
	end
	return rootPart, humanoid
end

local function runKeyboardMouse()
	task.wait(RuntimeQaConfig.InputQaStartupDelaySeconds)

	local virtualInput = UserInputService:CreateVirtualInput()
	if not virtualInput then
		emit("virtual_input_available", false)
		LOCAL_PLAYER:SetAttribute("RisingStepsInputQaComplete", true)
		return
	end
	emit("virtual_input_available", true)

	local rootPart, humanoid = getCharacterParts()
	if not rootPart or not humanoid then
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

local function runGamepadObservation()
	task.wait(RuntimeQaConfig.InputQaStartupDelaySeconds)

	local rootPart, humanoid = getCharacterParts()
	if not rootPart or not humanoid then
		emit("gamepad_character_ready", false)
		LOCAL_PLAYER:SetAttribute("RisingStepsGamepadQaComplete", true)
		return
	end

	local connected = UserInputService.GamepadEnabled
		or UserInputService:GetGamepadConnected(Enum.UserInputType.Gamepad1)
	emit("gamepad_connected", connected)
	LOCAL_PLAYER:SetAttribute("RisingStepsGamepadQaConnected", connected)

	local stickSeen = false
	local movePass = false
	local buttonASeen = false
	local jumpPass = false
	local moveCheckStarted = false
	local jumpCheckStarted = false
	local moveOrigin = rootPart.Position

	local changedConnection = UserInputService.InputChanged:Connect(function(input)
		if input.UserInputType ~= Enum.UserInputType.Gamepad1 or input.KeyCode ~= Enum.KeyCode.Thumbstick1 then
			return
		end
		local magnitude = Vector2.new(input.Position.X, input.Position.Y).Magnitude
		if magnitude < 0.25 then
			return
		end
		if not stickSeen then
			stickSeen = true
			emit("gamepad_thumbstick_event", true, ("magnitude=%.2f"):format(magnitude))
			LOCAL_PLAYER:SetAttribute("RisingStepsGamepadQaStick", true)
		end
		if moveCheckStarted then
			return
		end
		moveCheckStarted = true
		moveOrigin = rootPart.Position
		task.delay(0.45, function()
			if not rootPart.Parent then
				return
			end
			local delta =
				Vector3.new(rootPart.Position.X - moveOrigin.X, 0, rootPart.Position.Z - moveOrigin.Z).Magnitude
			movePass = delta >= 0.35 or humanoid.MoveDirection.Magnitude >= 0.20
			emit(
				"gamepad_move",
				movePass,
				("distance=%.2f moveDirection=%.2f"):format(delta, humanoid.MoveDirection.Magnitude)
			)
			LOCAL_PLAYER:SetAttribute("RisingStepsGamepadQaMove", movePass)
		end)
	end)

	local beganConnection = UserInputService.InputBegan:Connect(function(input)
		if input.UserInputType ~= Enum.UserInputType.Gamepad1 or input.KeyCode ~= Enum.KeyCode.ButtonA then
			return
		end
		if not buttonASeen then
			buttonASeen = true
			emit("gamepad_button_a_event", true)
			LOCAL_PLAYER:SetAttribute("RisingStepsGamepadQaButtonA", true)
		end
		if jumpCheckStarted then
			return
		end
		jumpCheckStarted = true
		local beforeY = rootPart.Position.Y
		task.spawn(function()
			local peakY = beforeY
			local deadline = os.clock() + 0.85
			repeat
				if not rootPart.Parent then
					break
				end
				peakY = math.max(peakY, rootPart.Position.Y)
				task.wait(0.03)
			until os.clock() >= deadline
			jumpPass = peakY > beforeY + 0.75
			emit("gamepad_jump", jumpPass, ("rise=%.2f"):format(peakY - beforeY))
			LOCAL_PLAYER:SetAttribute("RisingStepsGamepadQaJump", jumpPass)
		end)
	end)

	local deadline = os.clock() + RuntimeQaConfig.GamepadQaTimeoutSeconds
	repeat
		task.wait(0.1)
	until os.clock() >= deadline or (stickSeen and movePass and buttonASeen and jumpPass)

	changedConnection:Disconnect()
	beganConnection:Disconnect()

	local passed = connected and stickSeen and movePass and buttonASeen and jumpPass
	if not stickSeen then
		emit("gamepad_thumbstick_event", false, "no Thumbstick1 event observed")
	end
	if not buttonASeen then
		emit("gamepad_button_a_event", false, "no ButtonA event observed")
	end
	LOCAL_PLAYER:SetAttribute("RisingStepsGamepadQaPass", passed)
	LOCAL_PLAYER:SetAttribute("RisingStepsGamepadQaComplete", true)
	print(
		("[RisingStepsGamepadQA] COMPLETE connected=%s stick=%s move=%s buttonA=%s jump=%s pass=%s"):format(
			tostring(connected),
			tostring(stickSeen),
			tostring(movePass),
			tostring(buttonASeen),
			tostring(jumpPass),
			tostring(passed)
		)
	)
end

function InputQaController.start()
	if not RunService:IsStudio() then
		return
	end
	if RuntimeQaConfig.InputQaEnabled then
		task.spawn(runKeyboardMouse)
	end
	if RuntimeQaConfig.GamepadQaEnabled then
		task.spawn(runGamepadObservation)
	end
end

return InputQaController
