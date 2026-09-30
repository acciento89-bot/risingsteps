--!strict

local RetentionConfig = require("../config/RetentionConfig")

local RetentionRules = {}

function RetentionRules.dayIndex(timestamp: number): number
	return math.floor(timestamp / 86400)
end

function RetentionRules.dailyClaim(lastClaimDay: number, currentStreak: number, day: number)
	if lastClaimDay == day then
		return false, "already_claimed", currentStreak, 0
	end

	local nextStreak = if lastClaimDay == day - 1 then currentStreak + 1 else 1
	local bonusDays = math.clamp(nextStreak - 1, 0, RetentionConfig.Daily.MaxBonusDays - 1)
	local reward = RetentionConfig.Daily.BaseCoins + bonusDays * RetentionConfig.Daily.ConsecutiveDayBonus
	return true, "ok", nextStreak, reward
end

function RetentionRules.challengeForDay(day: number)
	local index = (math.max(0, day) % #RetentionConfig.Challenges) + 1
	return RetentionConfig.Challenges[index]
end

function RetentionRules.challengeProgress(challenge: any, currentProgress: number, event: any): number
	local progress = math.max(0, math.floor(currentProgress))

	if challenge.Kind == "Height" then
		local height = if type(event.Height) == "number" then math.max(0, math.floor(event.Height)) else 0
		return math.max(progress, height)
	elseif challenge.Kind == "Streak" then
		local streak = if type(event.Streak) == "number" then math.max(0, math.floor(event.Streak)) else 0
		return math.max(progress, streak)
	elseif challenge.Kind == "Perfect" and event.Grade == "Perfect" then
		return progress + 1
	end

	return progress
end

function RetentionRules.achievementIdsForEvent(event: any): { string }
	local result = {}

	for achievementId, definition in RetentionConfig.Achievements do
		local value = 0
		if definition.Kind == "Height" and type(event.Height) == "number" then
			value = event.Height
		elseif definition.Kind == "Streak" and type(event.Streak) == "number" then
			value = event.Streak
		elseif definition.Kind == "PerfectTotal" and type(event.PerfectTotal) == "number" then
			value = event.PerfectTotal
		end

		if value >= definition.Target then
			table.insert(result, achievementId)
		end
	end

	table.sort(result)
	return result
end

return table.freeze(RetentionRules)
