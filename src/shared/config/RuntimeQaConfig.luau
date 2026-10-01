--!strict

local RuntimeQaConfig = {
	EnabledInStudio = true,
	VisualOnly = false,
	StartupDelaySeconds = 4.0,
	VisualHoldSeconds = 8,
	SoakEnabled = false,
	SoakDurationSeconds = 1200,
	SoakCyclePauseSeconds = 6,
	SoakReportIntervalSeconds = 60,
	InputQaEnabled = true,
	InputQaStartupDelaySeconds = 0.5,
}

return table.freeze(RuntimeQaConfig)
