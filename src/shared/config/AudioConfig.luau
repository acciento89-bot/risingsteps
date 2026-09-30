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
	MaxConcurrentTransientSounds = 6,
}

return table.freeze(AudioConfig)
