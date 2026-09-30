--!strict

local EconomyConfig = {
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
	},
}

return table.freeze(EconomyConfig)
