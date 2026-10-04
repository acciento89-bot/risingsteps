--!strict

local AudioConfig = {
	Source = "rbxasset://sounds/electronicpingshort.wav",
	Landing = table.freeze({
		CleanPitch = 1.05,
		PerfectPitch = 1.28,
		BaseVolume = 0.24,
	}),
	Warning = table.freeze({
		Pitch = 0.78,
		Volume = 0.20,
	}),
	Failure = table.freeze({
		Pitch = 0.58,
		Volume = 0.28,
	}),
	Celebration = table.freeze({
		Pitch = 1.52,
		Volume = 0.26,
	}),
	MaxConcurrentTransientSounds = 3,
	MaxSingleTransientVolume = 0.28,
	ConcurrentVolumeDuck = 0.72,
	HigherPriorityDuckMultiplier = 0.52,
	HigherPriorityGuardSeconds = 0.18,
	CuePriority = table.freeze({
		Landing = 1,
		Celebration = 2,
		Warning = 3,
		Failure = 4,
	}),
	CueMinIntervalSeconds = table.freeze({
		Landing = 0.08,
		Warning = 0.45,
		Failure = 0.55,
		Celebration = 0.35,
	}),
}

return table.freeze(AudioConfig)
