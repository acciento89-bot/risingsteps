--!strict

local ScoreRules = {}

function ScoreRules.gradeLanding(
	rootX: number,
	rootZ: number,
	stepX: number,
	stepZ: number,
	footprint: number,
	perfectToleranceRatio: number,
	cleanToleranceRatio: number
): (string, number)
	assert(footprint > 0, "footprint must be positive")
	local halfWidth = footprint / 2
	local centerError = math.max(math.abs(rootX - stepX), math.abs(rootZ - stepZ))
	local ratio = centerError / halfWidth

	if ratio <= perfectToleranceRatio then
		return "Perfect", ratio
	end
	if ratio <= cleanToleranceRatio then
		return "Clean", ratio
	end
	return "Miss", ratio
end

function ScoreRules.nextStreak(currentStreak: number, grade: string): number
	if grade == "Perfect" or grade == "Clean" then
		return currentStreak + 1
	end
	return 0
end

return table.freeze(ScoreRules)
