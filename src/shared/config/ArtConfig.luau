--!strict

local ArtConfig = {
	VisualIdentity = "fantasy-sky-realms",
	Lighting = table.freeze({
		ClockTime = 17.78,
		Brightness = 2.65,
		ExposureCompensation = 0.28,
		AtmosphereDensity = 0.34,
		AtmosphereHaze = 2.15,
		CloudCover = 0.30,
		CloudDensity = 0.34,
	}),
	Fantasy = table.freeze({
		FloatingIslandClusters = 7,
		PortalCount = 2,
		WaterfallCount = 3,
		TreeClusters = 8,
	}),
	Bands = table.freeze({
		table.freeze({
			Id = "service-decks",
			MinHeight = 0,
			MaxHeight = 11,
			CenterY = 28,
			StructureColor = "#685A78",
			AccentColor = "#FFD36A",
		}),
		table.freeze({
			Id = "cloudline-works",
			MinHeight = 12,
			MaxHeight = 29,
			CenterY = 88,
			StructureColor = "#718AA0",
			AccentColor = "#74E8FF",
		}),
		table.freeze({
			Id = "stratosphere-spine",
			MinHeight = 30,
			MaxHeight = math.huge,
			CenterY = 154,
			StructureColor = "#806FA8",
			AccentColor = "#D7C5FF",
		}),
	}),
}

return table.freeze(ArtConfig)
