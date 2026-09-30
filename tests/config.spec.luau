local GameConfig = require("../src/shared/config/GameConfig")
local DifficultyConfig = require("../src/shared/config/DifficultyConfig")
local EconomyConfig = require("../src/shared/config/EconomyConfig")

assert(GameConfig.Name == "Rising Steps")
assert(GameConfig.Score.PrimaryMetric == "Height")
assert(GameConfig.Score.PerfectToleranceRatio < GameConfig.Score.CleanToleranceRatio)
assert(GameConfig.Movement.WalkSpeed == 16)
assert(GameConfig.Movement.JumpPower == 50)
assert(GameConfig.Step.MaxHorizontalCenterDistance == 8.25)
assert(GameConfig.Step.MaxVerticalRise == 5.0)
assert(GameConfig.Step.MinimumWarningSeconds >= 0.70)
assert(GameConfig.Failure.MaxPaidRevivesPerRun == 1)

local previousMinHeight = -1
local previousWarning = math.huge
for _, band in DifficultyConfig.Bands do
	assert(band.MinHeight > previousMinHeight)
	assert(band.HorizontalMin <= band.HorizontalMax)
	assert(band.VerticalMin <= band.VerticalMax)
	assert(band.WarningSeconds <= previousWarning)
	assert(band.WarningSeconds >= GameConfig.Step.MinimumWarningSeconds)
	assert(band.HorizontalMax <= GameConfig.Step.MaxHorizontalCenterDistance)
	assert(band.VerticalMax <= GameConfig.Step.MaxVerticalRise)
	previousMinHeight = band.MinHeight
	previousWarning = band.WarningSeconds
end

assert(EconomyConfig.CompetitiveHeightPurchasable == false)
assert(EconomyConfig.PaidMovementAdvantageAllowed == false)
assert(EconomyConfig.PromptOnSpawnAllowed == false)

return true
