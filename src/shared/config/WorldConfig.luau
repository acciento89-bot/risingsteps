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
	},
	Palette = {
		Foundation = "#121722",
		Steel = "#202938",
		Concrete = "#3A4657",
		WarmAccent = "#FFB454",
		GameplayAccent = "#5EEBFF",
	},
}

return table.freeze(WorldConfig)
