local AudioConfig = require("../src/shared/config/AudioConfig")
local AudioMixRules = require("../src/shared/rules/AudioMixRules")

assert(AudioConfig.Source:sub(1, 11) == "rbxasset://", "audio source must use a Roblox-owned/runtime asset path")
assert(AudioConfig.Landing.BaseVolume > 0 and AudioConfig.Landing.BaseVolume <= 1, "landing volume outside safe range")
assert(AudioConfig.Warning.Volume <= 0.5, "warning mix is too loud for repeated mobile playback")
assert(AudioConfig.Failure.Volume <= 0.5, "failure mix is too loud")
assert(AudioConfig.MaxConcurrentTransientSounds <= 3, "transient audio concurrency budget is too high")
assert(AudioConfig.ConcurrentVolumeDuck < 1, "concurrent audio must duck instead of summing at full volume")
assert(AudioConfig.HigherPriorityDuckMultiplier <= 0.60, "higher-priority cue ducking is too weak")
assert(AudioConfig.CuePriority.Failure > AudioConfig.CuePriority.Warning, "failure must outrank warning")
assert(AudioConfig.CuePriority.Warning > AudioConfig.CuePriority.Celebration, "warning must outrank celebration")
assert(AudioConfig.CuePriority.Celebration > AudioConfig.CuePriority.Landing, "celebration must outrank landing")
assert(AudioConfig.MaxSingleTransientVolume <= 0.30, "single transient volume cap is too high")
assert(AudioConfig.CueMinIntervalSeconds.Warning >= 0.40, "warning cue needs mobile-safe throttling")
assert(AudioConfig.CueMinIntervalSeconds.Failure >= 0.50, "failure cue needs duplicate suppression")
assert(AudioConfig.CueMinIntervalSeconds.Celebration >= 0.30, "celebration cue needs stack suppression")

local now = 10
assert(
	AudioMixRules.shouldPlay(
		"Landing",
		now,
		{},
		{},
		AudioConfig.CueMinIntervalSeconds,
		AudioConfig.CuePriority,
		AudioConfig.HigherPriorityGuardSeconds
	),
	"idle landing cue should play"
)
assert(
	not AudioMixRules.shouldPlay(
		"Landing",
		now,
		{ Landing = 9.96 },
		{},
		AudioConfig.CueMinIntervalSeconds,
		AudioConfig.CuePriority,
		AudioConfig.HigherPriorityGuardSeconds
	),
	"landing cue cooldown should suppress duplicates"
)
assert(
	not AudioMixRules.shouldPlay(
		"Landing",
		now,
		{},
		{ { Cue = "Warning", StartedAt = 9.90 } },
		AudioConfig.CueMinIntervalSeconds,
		AudioConfig.CuePriority,
		AudioConfig.HigherPriorityGuardSeconds
	),
	"recent warning should suppress lower-priority landing cue"
)
assert(
	AudioMixRules.shouldPlay(
		"Failure",
		now,
		{},
		{ { Cue = "Landing", StartedAt = 9.90 } },
		AudioConfig.CueMinIntervalSeconds,
		AudioConfig.CuePriority,
		AudioConfig.HigherPriorityGuardSeconds
	),
	"failure cue must be allowed over a lower-priority landing cue"
)
assert(
	AudioMixRules.shouldDuck("Landing", "Warning", AudioConfig.CuePriority),
	"warning should duck an active landing cue"
)
assert(
	not AudioMixRules.shouldDuck("Warning", "Landing", AudioConfig.CuePriority),
	"landing must not duck a warning cue"
)
assert(
	AudioMixRules.volumeForConcurrency(0.28, 2, AudioConfig.MaxSingleTransientVolume, AudioConfig.ConcurrentVolumeDuck)
		< AudioMixRules.volumeForConcurrency(
			0.28,
			1,
			AudioConfig.MaxSingleTransientVolume,
			AudioConfig.ConcurrentVolumeDuck
		),
	"additional concurrent cues must reduce incoming volume"
)

return true
