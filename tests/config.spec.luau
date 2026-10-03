local GameConfig = require("../src/shared/config/GameConfig")
local DifficultyConfig = require("../src/shared/config/DifficultyConfig")
local EconomyConfig = require("../src/shared/config/EconomyConfig")

assert(GameConfig.Name == "Rising Steps", "product name mismatch")
assert(GameConfig.Score.PrimaryMetric == "Height", "primary metric must be Height")
assert(
	GameConfig.Score.PerfectToleranceRatio < GameConfig.Score.CleanToleranceRatio,
	"Perfect tolerance must be stricter than Clean"
)
assert(GameConfig.Movement.WalkSpeed == 16, "WalkSpeed drifted from product lock")
assert(GameConfig.Movement.JumpPower == 50, "JumpPower drifted from product lock")
assert(GameConfig.Step.MaxHorizontalCenterDistance == 9.0, "horizontal hard cap changed")
assert(GameConfig.Step.MaxVerticalRise == 5.0, "vertical hard cap changed")
assert(GameConfig.Step.MinimumWarningSeconds >= 0.70, "warning floor below product lock")
assert(GameConfig.Failure.MaxPaidRevivesPerRun == 1, "paid revive cap changed")

local previousMinHeight = -1
local previousWarning = math.huge
for _, band in DifficultyConfig.Bands do
	assert(band.MinHeight > previousMinHeight, "difficulty band heights must be strictly ascending")
	assert(band.HorizontalMin <= band.HorizontalMax, "horizontal band bounds are inverted")
	assert(band.VerticalMin <= band.VerticalMax, "vertical band bounds are inverted")
	assert(band.WarningSeconds <= previousWarning, "warning time must not increase with difficulty")
	assert(band.WarningSeconds >= GameConfig.Step.MinimumWarningSeconds, "difficulty band warning is below the minimum")
	assert(
		band.HorizontalMax <= GameConfig.Step.MaxHorizontalCenterDistance,
		"difficulty band exceeds horizontal hard cap"
	)
	assert(band.VerticalMax <= GameConfig.Step.MaxVerticalRise, "difficulty band exceeds vertical hard cap")
	previousMinHeight = band.MinHeight
	previousWarning = band.WarningSeconds
end

assert(EconomyConfig.CompetitiveHeightPurchasable == false, "competitive Height cannot be purchasable")
assert(EconomyConfig.PaidMovementAdvantageAllowed == false, "paid movement advantage is forbidden")
assert(EconomyConfig.PromptOnSpawnAllowed == false, "spawn purchase prompts are forbidden")

return true
