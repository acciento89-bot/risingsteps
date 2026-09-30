--!strict

local SecurityConfig = {
	MaxPayloadTextLength = 32,
	MaxAbsPosition = 100000,
	MaxAbsVelocity = 300,
	MinimumLandingIntervalSeconds = 0.18,
	RemoteIntervals = table.freeze({
		PlayerCommand = 0.12,
		ShopCommand = 0.15,
	}),
}

return table.freeze(SecurityConfig)
