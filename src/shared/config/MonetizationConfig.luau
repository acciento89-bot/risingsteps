--!strict

local MonetizationConfig = {
	MaxProcessedReceipts = 50,
	DeveloperProducts = table.freeze({
		Revive = table.freeze({
			ProductId = 0,
			Robux = 19,
			Grant = "Revive",
			MaxInventory = 3,
		}),
		Stability = table.freeze({
			ProductId = 0,
			Robux = 29,
			Grant = "Stability",
			DurationSeconds = 300,
			WarningMultiplier = 1.35,
		}),
	}),
	GamePasses = table.freeze({
		CoinMultiplier = table.freeze({
			GamePassId = 0,
			Robux = 99,
			Multiplier = 2,
		}),
	}),
}

return table.freeze(MonetizationConfig)
