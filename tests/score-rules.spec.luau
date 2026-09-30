local GameConfig = require("../src/shared/config/GameConfig")
local ScoreRules = require("../src/shared/rules/ScoreRules")

local function grade(x, z)
	return ScoreRules.gradeLanding(
		x,
		z,
		0,
		0,
		GameConfig.Step.BaseFootprint,
		GameConfig.Score.PerfectToleranceRatio,
		GameConfig.Score.CleanToleranceRatio
	)
end

local perfect = grade(0.2, 0.3)
local clean = grade(1.2, 0.4)
local miss = grade(3.0, 0)

assert(perfect == "Perfect", "center landing should be Perfect")
assert(clean == "Clean", "mid-pad landing should be Clean")
assert(miss == "Miss", "edge landing outside Clean tolerance should be Miss")
assert(ScoreRules.nextStreak(4, "Perfect") == 5, "Perfect must increment streak")
assert(ScoreRules.nextStreak(4, "Clean") == 5, "Clean must increment streak")
assert(ScoreRules.nextStreak(4, "Miss") == 0, "Miss must break streak")

return true
