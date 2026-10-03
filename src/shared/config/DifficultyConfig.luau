--!strict

local DifficultyConfig = {
	Bands = {
		{
			Id = "service-decks",
			DisplayName = "Service Decks",
			MinHeight = 0,
			MaxHeight = 11,
			HorizontalMin = 6.8,
			HorizontalMax = 7.6,
			VerticalMin = 3.7,
			VerticalMax = 4.2,
			WarningSeconds = 1.70,
		},
		{
			Id = "cloudline-works",
			DisplayName = "Cloudline Works",
			MinHeight = 12,
			MaxHeight = 29,
			HorizontalMin = 7.2,
			HorizontalMax = 8.2,
			VerticalMin = 4.0,
			VerticalMax = 4.5,
			WarningSeconds = 1.35,
		},
		{
			Id = "stratosphere-spine",
			DisplayName = "Stratosphere Spine",
			MinHeight = 30,
			MaxHeight = math.huge,
			HorizontalMin = 7.6,
			HorizontalMax = 8.8,
			HorizontalPeak = 9.0,
			VerticalMin = 4.3,
			VerticalMax = 4.8,
			VerticalPeak = 4.95,
			WarningSeconds = 1.05,
		},
	},
}

return table.freeze(DifficultyConfig)
