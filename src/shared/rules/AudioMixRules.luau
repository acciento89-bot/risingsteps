--!strict

local AudioMixRules = {}

type ActiveCue = {
	Cue: string,
	StartedAt: number,
}

type NumberMap = { [string]: number }

function AudioMixRules.priority(cue: string, cuePriority: NumberMap): number
	return cuePriority[cue] or 0
end

function AudioMixRules.shouldPlay(
	cue: string,
	now: number,
	lastCueAt: NumberMap,
	activeCues: { ActiveCue },
	cueMinIntervalSeconds: NumberMap,
	cuePriority: NumberMap,
	higherPriorityGuardSeconds: number
): boolean
	local minInterval = cueMinIntervalSeconds[cue]
	local lastAt = lastCueAt[cue]
	if type(minInterval) == "number" and lastAt and now - lastAt < minInterval then
		return false
	end

	local priority = AudioMixRules.priority(cue, cuePriority)
	for _, active in activeCues do
		if
			AudioMixRules.priority(active.Cue, cuePriority) > priority
			and now - active.StartedAt < higherPriorityGuardSeconds
		then
			return false
		end
	end

	return true
end

function AudioMixRules.volumeForConcurrency(
	requestedVolume: number,
	activeCount: number,
	maxSingleTransientVolume: number,
	concurrentVolumeDuck: number
): number
	local capped = math.min(requestedVolume, maxSingleTransientVolume)
	return capped * (concurrentVolumeDuck ^ math.max(0, activeCount))
end

function AudioMixRules.shouldDuck(existingCue: string, incomingCue: string, cuePriority: NumberMap): boolean
	return AudioMixRules.priority(incomingCue, cuePriority) > AudioMixRules.priority(existingCue, cuePriority)
end

return table.freeze(AudioMixRules)
