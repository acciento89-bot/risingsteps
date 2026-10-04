local ArtConfig = require("../src/shared/config/ArtConfig")

assert(#ArtConfig.Bands >= 3, "production art requires at least three altitude bands")
assert(ArtConfig.VisualIdentity == "fantasy-sky-realms", "Rising Steps must use the approved fantasy sky concept")
assert(type(ArtConfig.Fantasy) == "table", "fantasy art contract missing")
assert(ArtConfig.Fantasy.FloatingIslandClusters >= 3, "fantasy world needs multiple floating island clusters")
assert(ArtConfig.Fantasy.PortalCount >= 2, "fantasy world needs portal landmarks")
assert(ArtConfig.Fantasy.WaterfallCount >= 2, "fantasy world needs waterfall landmarks")

assert(ArtConfig.Bands[1].Id == "service-decks", "first art band must match product lock")
assert(ArtConfig.Bands[2].Id == "cloudline-works", "second art band must match product lock")
assert(ArtConfig.Bands[3].Id == "stratosphere-spine", "third art band must match product lock")

local previousMin = -1
for _, band in ArtConfig.Bands do
	assert(band.MinHeight > previousMin, "art bands must ascend by height")
	assert(type(band.StructureColor) == "string", "art band missing structure color")
	assert(type(band.AccentColor) == "string", "art band missing accent color")
	previousMin = band.MinHeight
end

return true
