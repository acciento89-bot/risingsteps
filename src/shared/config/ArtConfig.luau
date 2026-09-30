--!strict

local ArtConfig = {
	Lighting = table.freeze({
		ClockTime = 17.35,
		Brightness = 2.1,
		ExposureCompensation = 0.05,
		AtmosphereDensity = 0.30,
		AtmosphereHaze = 1.8,
		CloudCover = 0.28,
		CloudDensity = 0.34,
	}),
	Bands = table.freeze({
		table.freeze({
			Id = "service-decks",
			MinHeight = 0,
			MaxHeight = 11,
			CenterY = 28,
			StructureColor = "#202938",
			AccentColor = "#FFB454",
		}),
		table.freeze({
			Id = "cloudline-works",
			MinHeight = 12,
			MaxHeight = 29,
			CenterY = 88,
			StructureColor = "#263447",
			AccentColor = "#5EEBFF",
		}),
		table.freeze({
			Id = "stratosphere-spine",
			MinHeight = 30,
			MaxHeight = math.huge,
			CenterY = 154,
			StructureColor = "#182331",
			AccentColor = "#B9F6FF",
		}),
	}),
}

return table.freeze(ArtConfig)
