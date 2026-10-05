local fs = require("@lune/fs")

local hud = fs.readFile("src/client/controllers/HudController.luau")

assert(string.find(hud, 'conceptShowcase.Name = "ConceptShowcase"', 1, true), "desktop concept showcase layer missing")
assert(string.find(hud, 'shopShowcase.Name = "ConceptShopShowcase"', 1, true), "concept shop showcase missing")
assert(string.find(hud, 'conceptProgressCard.Name = "ConceptProgressCard"', 1, true), "concept progress card missing")
assert(
	string.find(hud, 'conceptDailyRewards.Name = "ConceptDailyRewards"', 1, true),
	"concept daily rewards card missing"
)
assert(string.find(hud, 'conceptVistaCard.Name = "ConceptVistaCard"', 1, true), "concept visual preview card missing")
assert(
	string.find(hud, 'local conceptViewport = Instance.new("ViewportFrame")', 1, true),
	"concept visual preview must use a native ViewportFrame"
)
assert(
	string.find(hud, 'conceptShowcase.Visible = viewportClass ~= "compact"', 1, true),
	"concept showcase must stay desktop-only"
)
assert(
	string.find(hud, 'metrics.Visible = viewportClass == "compact"', 1, true),
	"desktop concept composition should replace the legacy metrics block"
)

return true
