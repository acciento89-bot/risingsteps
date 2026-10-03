--!strict

local DifficultyConfig = {
	Bands = {
		{
			Id = "service-decks",
			DisplayName = "Service Decks",
			MinHeight = 0,
			MaxHeight = 11,
			HorizontalMin = 3.4,
			HorizontalMax = 5.0,
			VerticalMin = 2.7,
			VerticalMax = 3.5,
			WarningSeconds = 1.70,
		},
		{
			Id = "cloudline-works",
			DisplayName = "Cloudline Works",
			MinHeight = 12,
			MaxHeight = 29,
			HorizontalMin = 4.0,
			HorizontalMax = 5.8,
			VerticalMin = 3.0,
			VerticalMax = 3.9,
			WarningSeconds = 1.35,
		},
		{
			Id = "stratosphere-spine",
			DisplayName = "Stratosphere Spine",
			MinHeight = 30,
			MaxHeight = math.huge,
			HorizontalMin = 4.5,
			HorizontalMax = 6.4,
			HorizontalPeak = 7.0,
			VerticalMin = 3.2,
			VerticalMax = 4.1,
			VerticalPeak = 4.5,
			WarningSeconds = 1.05,
		},
	},
}

return table.freeze(DifficultyConfig)
