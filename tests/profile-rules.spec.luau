local CosmeticConfig = require("../src/shared/config/CosmeticConfig")
local EconomyConfig = require("../src/shared/config/EconomyConfig")
local ProfileRules = require("../src/shared/rules/ProfileRules")

local fresh = ProfileRules.defaultProfile()
assert(fresh.Version == EconomyConfig.ProfileVersion, "default profile version mismatch")
assert(fresh.PB == 0 and fresh.Coins == 0, "fresh profile economy must start at zero")
assert(fresh.Owned.StepThemes.steel == true, "default step theme must be owned")
assert(fresh.TutorialComplete == false, "fresh profile must require the first-session tutorial")
assert(fresh.Daily.LastClaimDay == -1, "fresh profile must not have a daily claim")
assert(fresh.Totals.Perfect == 0, "fresh profile Perfect total must start at zero")
assert(fresh.Equipped.StepThemes == "steel", "default step theme must be equipped")

local migratedV1 = ProfileRules.migrate({
	Version = 1,
	PB = 37,
})
assert(migratedV1.Version == EconomyConfig.ProfileVersion, "v1 profile did not migrate to current version")
assert(migratedV1.PB == 37, "v1 PB was not preserved")
assert(migratedV1.Coins == 0, "v1 migration invented coins")
assert(migratedV1.Owned.Trails.none == true, "v1 migration lost default trail ownership")
assert(migratedV1.TutorialComplete == false, "legacy migration invented tutorial completion")
assert(migratedV1.Challenge.Day == -1, "legacy migration invented challenge state")

local migratedV2 = ProfileRules.migrate({
	Version = 2,
	PB = 81,
	Coins = 1200,
	Owned = {
		Trails = { ion = true, hacked = true },
		StepThemes = { frost = true },
	},
	Equipped = {
		Trails = "ion",
		StepThemes = "frost",
		EnvironmentThemes = "not-real",
	},
	Settings = {
		ReducedMotion = true,
	},
})
assert(migratedV2.PB == 81 and migratedV2.Coins == 1200, "v2 values were not preserved")
assert(migratedV2.Owned.Trails.ion == true, "owned trail was not preserved")
assert(migratedV2.Owned.Trails.hacked == nil, "unknown cosmetic id survived migration")
assert(migratedV2.Equipped.Trails == "ion", "owned trail did not remain equipped")
assert(migratedV2.Equipped.EnvironmentThemes == "default", "invalid environment theme survived migration")
assert(migratedV2.Settings.ReducedMotion == true, "accessibility setting was not preserved")

assert(ProfileRules.rewardForLanding("Miss", 10, 10) == 0, "Miss must never award coins")
assert(
	ProfileRules.rewardForLanding("Perfect", 5, 10)
		== EconomyConfig.Currency.PerfectLanding
			+ EconomyConfig.Currency.StreakBonus
			+ EconomyConfig.Currency.HeightMilestoneBonus,
	"Perfect streak milestone reward mismatch"
)

local shopProfile = ProfileRules.defaultProfile()
shopProfile.Coins = 1000
local ok, reason, price = ProfileRules.validatePurchase(shopProfile, "Trails", "ion")
assert(ok and reason == "ok", "valid cosmetic purchase was rejected")
assert(price == CosmeticConfig.Categories.Trails.Items.ion.Price, "cosmetic price lookup mismatch")

shopProfile.Coins = 0
local poorOk, poorReason = ProfileRules.validatePurchase(shopProfile, "Trails", "ion")
assert(not poorOk and poorReason == "insufficient_coins", "insufficient-coins purchase was accepted")

local equipOk, equipReason = ProfileRules.validateEquip(shopProfile, "Trails", "ion")
assert(not equipOk and equipReason == "not_owned", "unowned cosmetic was equip-able")

return true
