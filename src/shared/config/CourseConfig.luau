--!strict

local CourseConfig = {
	LaneSpacing = 80,
	InitialStepCount = 42,
	VisualVariantCount = 6,
	ExpiryBehindCurrent = 3,
	FallCheckIntervalSeconds = 0.10,
	RespawnResetDelaySeconds = 0.15,
	ReachabilityControlSafetyFactor = 0.55,
	MaxLaneBaseParts = 900,
	MaxLaneBeamsAndLights = 96,
	ClientVisibleStepsAhead = 4,
}

return table.freeze(CourseConfig)
