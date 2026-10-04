--!strict

local CosmeticConfig = {
	Categories = table.freeze({
		Trails = table.freeze({
			Default = "none",
			Items = table.freeze({
				none = table.freeze({ DisplayName = "No Trail", Price = 0 }),
				ion = table.freeze({ DisplayName = "Sky Trail", Price = 280 }),
				ember = table.freeze({ DisplayName = "Sunset Trail", Price = 420 }),
			}),
		}),
		LandingEffects = table.freeze({
			Default = "basic",
			Items = table.freeze({
				basic = table.freeze({ DisplayName = "Cloud Ripple", Price = 0 }),
				spark = table.freeze({ DisplayName = "Star Spark", Price = 320 }),
				pulse = table.freeze({ DisplayName = "Cloud Pulse", Price = 520 }),
			}),
		}),
		StepThemes = table.freeze({
			Default = "steel",
			Items = table.freeze({
				steel = table.freeze({ DisplayName = "Moss Stone", Price = 0 }),
				forge = table.freeze({ DisplayName = "Golden Ruins", Price = 620 }),
				frost = table.freeze({ DisplayName = "Crystal Sky", Price = 680 }),
			}),
		}),
		EnvironmentThemes = table.freeze({
			Default = "default",
			Items = table.freeze({
				default = table.freeze({ DisplayName = "Dawn Gardens", Price = 0 }),
				dusk = table.freeze({ DisplayName = "Violet Dream", Price = 850 }),
				storm = table.freeze({ DisplayName = "Cloudbreak Storm", Price = 980 }),
			}),
		}),
	}),
}

return table.freeze(CosmeticConfig)
