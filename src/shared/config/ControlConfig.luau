--!strict

local ControlConfig = {
	UseDefaultRobloxMovement = true,
	UseDefaultRobloxJump = true,
	AllowCameraOrbit = true,
	DisableShiftLock = false,
	Touch = {
		MinimumCustomTargetPixels = 48,
		ReserveLowerRightForJump = true,
	},
	Controller = {
		UseStandardThumbstickMovement = true,
		UseStandardJumpAction = true,
	},
}

return table.freeze(ControlConfig)
