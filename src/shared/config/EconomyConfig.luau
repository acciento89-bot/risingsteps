--!strict

local EconomyConfig = {
	ProfileVersion = 4,
	DataStoreName = "RisingStepsProfileV1",
	AutosaveSeconds = 60,
	LoadAttempts = 3,
	LoadRetrySeconds = 1.5,
	CompetitiveHeightPurchasable = false,
	PaidMovementAdvantageAllowed = false,
	PromptOnSpawnAllowed = false,
	MaxPaidRevivesPerRun = 1,
	AllowedCategories = table.freeze({
		"revive",
		"step-stability",
		"coin-multiplier",
		"trail",
		"landing-effect",
		"step-theme",
		"environment-theme",
	}),
	Currency = {
		Name = "Coins",
		Persistent = true,
		CleanLanding = 2,
		PerfectLanding = 4,
		StreakEvery = 5,
		StreakBonus = 2,
		HeightMilestoneEvery = 10,
		HeightMilestoneBonus = 6,
	},
}

return table.freeze(EconomyConfig)
