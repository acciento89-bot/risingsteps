--!strict

local WorldConfig = {
	WorldFolderName = "RisingStepsWorld",
	StartDeck = {
		Name = "StartDeck",
		CenterY = 4,
		DeckSize = 12,
		DeckThickness = 1.2,
		LandingInsetSize = 7,
		LandingInsetThickness = 0.28,
		SpawnHeightAboveDeck = 3.25,
		SpawnBackOffset = 3.2,
		OnboardingGuideCount = 3,
	},
	Palette = {
		Foundation = "#2A2340",
		Steel = "#685A78",
		Concrete = "#769268",
		WarmAccent = "#FFD36A",
		GameplayAccent = "#74E8FF",
	},
}

return table.freeze(WorldConfig)
