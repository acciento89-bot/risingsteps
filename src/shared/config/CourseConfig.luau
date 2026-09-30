--!strict

local CourseConfig = {
	LaneSpacing = 80,
	InitialStepCount = 42,
	VisualVariantCount = 6,
	ExpiryBehindCurrent = 2,
	FallCheckIntervalSeconds = 0.10,
	RespawnResetDelaySeconds = 0.15,
}

return table.freeze(CourseConfig)
