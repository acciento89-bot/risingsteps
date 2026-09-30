--!strict

local RuntimeQaConfig = {
	EnabledInStudio = true,
	StartupDelaySeconds = 4.0,
	VisualHoldSeconds = 4,
	SoakEnabled = false,
	SoakDurationSeconds = 1200,
	SoakCyclePauseSeconds = 6,
	SoakReportIntervalSeconds = 60,
	InputQaEnabled = true,
	InputQaStartupDelaySeconds = 0.5,
}

return table.freeze(RuntimeQaConfig)
