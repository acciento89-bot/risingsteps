local GameConfig = require("../src/shared/config/GameConfig")
local DifficultyConfig = require("../src/shared/config/DifficultyConfig")
local EconomyConfig = require("../src/shared/config/EconomyConfig")
local CosmeticConfig = require("../src/shared/config/CosmeticConfig")
local RemoteConfig = require("../src/shared/config/RemoteConfig")
local CourseConfig = require("../src/shared/config/CourseConfig")

assert(GameConfig.Name == "Rising Steps", "product name changed")
assert(
	GameConfig.Score.PerfectToleranceRatio < GameConfig.Score.CleanToleranceRatio,
	"landing grade tolerances invalid"
)
assert(GameConfig.Step.MinimumWarningSeconds >= 0.70, "warning window below product lock")
assert(GameConfig.Step.MaxHorizontalCenterDistance <= 8.25, "horizontal reach exceeds product lock")
assert(GameConfig.Step.MaxVerticalRise <= 5.0, "vertical reach exceeds product lock")
assert(GameConfig.Failure.MaxPaidRevivesPerRun == 1, "paid revive cap changed")
assert(EconomyConfig.CompetitiveHeightPurchasable == false, "competitive height must not be purchasable")
assert(EconomyConfig.PaidMovementAdvantageAllowed == false, "paid movement advantage is forbidden")
assert(EconomyConfig.ProfileVersion >= 2, "progression profile schema must be versioned")
assert(CosmeticConfig.Categories.Trails.Default ~= nil, "trail catalog is missing a default")
assert(CosmeticConfig.Categories.StepThemes.Default ~= nil, "step-theme catalog is missing a default")
assert(RemoteConfig.Definitions.StateSnapshot.Direction == "ServerToClient", "state snapshot authority drifted")
assert(#DifficultyConfig.Bands >= 3, "altitude progression requires at least three bands")
assert(CourseConfig.VisualVariantCount >= 6, "step kit must expose at least six visual variants")
assert(CourseConfig.ExpiryBehindCurrent >= 2, "expiry must not remove the player's current/next support too early")

print("Rising Steps release-readiness static rules passed")
