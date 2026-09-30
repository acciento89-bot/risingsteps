local AudioConfig = require("../src/shared/config/AudioConfig")

assert(AudioConfig.Source:sub(1, 11) == "rbxasset://", "audio source must use a Roblox-owned/runtime asset path")
assert(AudioConfig.Landing.BaseVolume > 0 and AudioConfig.Landing.BaseVolume <= 1, "landing volume outside safe range")
assert(AudioConfig.Warning.Volume <= 0.5, "warning mix is too loud for repeated mobile playback")
assert(AudioConfig.Failure.Volume <= 0.5, "failure mix is too loud")
assert(AudioConfig.MaxConcurrentTransientSounds <= 8, "transient audio concurrency budget is too high")

return true
