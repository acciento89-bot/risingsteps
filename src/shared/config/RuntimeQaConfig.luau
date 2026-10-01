--!strict

local RuntimeQaConfig = {
	EnabledInStudio = false,
	VisualOnly = false,
	StartupDelaySeconds = 4.0,
	VisualHoldSeconds = 8,
	SoakEnabled = false,
	SoakDurationSeconds = 1200,
	SoakCyclePauseSeconds = 6,
	SoakReportIntervalSeconds = 60,
	InputQaEnabled = false,
	InputQaStartupDelaySeconds = 0.5,
}

return table.freeze(RuntimeQaConfig)
