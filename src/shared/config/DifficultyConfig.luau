--!strict

local DifficultyConfig = {
	Bands = {
		{
			Id = "service-decks",
			DisplayName = "Service Decks",
			MinHeight = 0,
			MaxHeight = 11,
			HorizontalMin = 4.5,
			HorizontalMax = 6.0,
			VerticalMin = 3.4,
			VerticalMax = 4.0,
			WarningSeconds = 1.35,
		},
		{
			Id = "cloudline-works",
			DisplayName = "Cloudline Works",
			MinHeight = 12,
			MaxHeight = 29,
			HorizontalMin = 5.0,
			HorizontalMax = 7.0,
			VerticalMin = 3.6,
			VerticalMax = 4.4,
			WarningSeconds = 1.00,
		},
		{
			Id = "stratosphere-spine",
			DisplayName = "Stratosphere Spine",
			MinHeight = 30,
			MaxHeight = math.huge,
			HorizontalMin = 5.5,
			HorizontalMax = 7.5,
			HorizontalPeak = 8.25,
			VerticalMin = 3.8,
			VerticalMax = 4.6,
			VerticalPeak = 5.0,
			WarningSeconds = 0.70,
		},
	},
}

return table.freeze(DifficultyConfig)
