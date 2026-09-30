local RetentionConfig = require("../src/shared/config/RetentionConfig")
local RetentionRules = require("../src/shared/rules/RetentionRules")

assert(RetentionRules.dayIndex(0) == 0, "day index at epoch must be zero")
assert(RetentionRules.dayIndex(86400 * 12 + 50) == 12, "day index floor is incorrect")

local ok, reason, streak, reward = RetentionRules.dailyClaim(10, 3, 11)
assert(ok and reason == "ok", "consecutive daily claim was rejected")
assert(streak == 4, "consecutive daily streak did not increment")
assert(
	reward == RetentionConfig.Daily.BaseCoins + 3 * RetentionConfig.Daily.ConsecutiveDayBonus,
	"daily streak reward mismatch"
)

local duplicate, duplicateReason = RetentionRules.dailyClaim(11, 4, 11)
assert(not duplicate and duplicateReason == "already_claimed", "duplicate daily claim was accepted")

local resetOk, _, resetStreak = RetentionRules.dailyClaim(2, 7, 15)
assert(resetOk and resetStreak == 1, "broken daily sequence did not reset")

local heightChallenge = { Kind = "Height", Target = 15, RewardCoins = 45 }
assert(
	RetentionRules.challengeProgress(heightChallenge, 8, { Height = 13 }) == 13,
	"height challenge did not track max height"
)
assert(RetentionRules.challengeProgress(heightChallenge, 13, { Height = 9 }) == 13, "height challenge regressed")

local perfectChallenge = { Kind = "Perfect", Target = 5, RewardCoins = 55 }
assert(
	RetentionRules.challengeProgress(perfectChallenge, 2, { Grade = "Perfect" }) == 3,
	"Perfect challenge did not increment"
)
assert(
	RetentionRules.challengeProgress(perfectChallenge, 2, { Grade = "Clean" }) == 2,
	"Clean landing incorrectly advanced Perfect challenge"
)

local achievements = RetentionRules.achievementIdsForEvent({
	Height = 31,
	Streak = 10,
	PerfectTotal = 25,
})
assert(#achievements == 4, "expected all four launch achievements to qualify")

return true
