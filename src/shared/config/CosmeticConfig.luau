--!strict

local CosmeticConfig = {
	Categories = table.freeze({
		Trails = table.freeze({
			Default = "none",
			Items = table.freeze({
				none = table.freeze({ DisplayName = "No Trail", Price = 0 }),
				ion = table.freeze({ DisplayName = "Ion Wake", Price = 280 }),
				ember = table.freeze({ DisplayName = "Ember Thread", Price = 420 }),
			}),
		}),
		LandingEffects = table.freeze({
			Default = "basic",
			Items = table.freeze({
				basic = table.freeze({ DisplayName = "Impact Ring", Price = 0 }),
				spark = table.freeze({ DisplayName = "Arc Spark", Price = 320 }),
				pulse = table.freeze({ DisplayName = "Cloud Pulse", Price = 520 }),
			}),
		}),
		StepThemes = table.freeze({
			Default = "steel",
			Items = table.freeze({
				steel = table.freeze({ DisplayName = "Maintenance Steel", Price = 0 }),
				forge = table.freeze({ DisplayName = "Amber Forge", Price = 620 }),
				frost = table.freeze({ DisplayName = "Cryo Grid", Price = 680 }),
			}),
		}),
		EnvironmentThemes = table.freeze({
			Default = "default",
			Items = table.freeze({
				default = table.freeze({ DisplayName = "Aerial Spine", Price = 0 }),
				dusk = table.freeze({ DisplayName = "Industrial Dusk", Price = 850 }),
				storm = table.freeze({ DisplayName = "Cloudbreak Storm", Price = 980 }),
			}),
		}),
	}),
}

return table.freeze(CosmeticConfig)
