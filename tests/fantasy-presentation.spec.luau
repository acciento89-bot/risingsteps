local fs = require("@lune/fs")

local world = fs.readFile("src/server/services/WorldArtService.luau")
local deck = fs.readFile("src/server/services/StartDeckService.luau")
local steps = fs.readFile("src/server/services/StepCourseService.luau")

assert(string.find(world, "FloatingRock", 1, true), "fantasy world must include floating rock islands")
assert(string.find(world, "PortalRune", 1, true), "fantasy world must include portal landmarks")
assert(string.find(world, "SkyWaterfall", 1, true), "fantasy world must include waterfall landmarks")
assert(string.find(deck, "SkyIslandDeck", 1, true), "spawn must read as a floating island")
assert(string.find(steps, "RockShard", 1, true), "steps need hanging rock silhouettes")
assert(string.find(steps, "Enum.Material.Grass", 1, true), "steps need grass landing surfaces")

local cosmetics = fs.readFile("src/shared/config/CosmeticConfig.luau")
local presentation = fs.readFile("src/client/controllers/CosmeticPresentationController.luau")
assert(string.find(cosmetics, "Sky Trail", 1, true), "shop copy must match the fantasy sky identity")
assert(string.find(cosmetics, "Dawn Gardens", 1, true), "default environment cosmetic must match the fantasy world")
assert(
	string.find(presentation, "Color3.fromRGB(112, 94, 126)", 1, true),
	"default step cosmetics must preserve fantasy stone colors"
)
assert(
	string.find(presentation, "RuneEdge", 1, true),
	"all platform rune edges must receive the equipped fantasy theme"
)
assert(
	string.find(presentation, 'descendant.Name == "BonusCore"', 1, true),
	"bonus core must receive the equipped fantasy accent"
)

return true
