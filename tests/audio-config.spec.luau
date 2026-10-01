local AudioConfig = require("../src/shared/config/AudioConfig")

assert(AudioConfig.Source:sub(1, 11) == "rbxasset://", "audio source must use a Roblox-owned/runtime asset path")
assert(AudioConfig.Landing.BaseVolume > 0 and AudioConfig.Landing.BaseVolume <= 1, "landing volume outside safe range")
assert(AudioConfig.Warning.Volume <= 0.5, "warning mix is too loud for repeated mobile playback")
assert(AudioConfig.Failure.Volume <= 0.5, "failure mix is too loud")
assert(AudioConfig.MaxConcurrentTransientSounds <= 4, "transient audio concurrency budget is too high")
assert(AudioConfig.MaxSingleTransientVolume <= 0.30, "single transient volume cap is too high")
assert(AudioConfig.CueMinIntervalSeconds.Warning >= 0.40, "warning cue needs mobile-safe throttling")
assert(AudioConfig.CueMinIntervalSeconds.Failure >= 0.50, "failure cue needs duplicate suppression")
assert(AudioConfig.CueMinIntervalSeconds.Celebration >= 0.30, "celebration cue needs stack suppression")

return true
