--!strict

local RuntimeQaConfig = {
	EnabledInStudio = true,
	StartupDelaySeconds = 1.0,
	VisualHoldSeconds = 10,
	SoakEnabled = true,
	SoakDurationSeconds = 1200,
	SoakCyclePauseSeconds = 6,
	SoakReportIntervalSeconds = 60,
}

return table.freeze(RuntimeQaConfig)
