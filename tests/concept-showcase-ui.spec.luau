local fs = require("@lune/fs")

local hud = fs.readFile("src/client/controllers/HudController.luau")

assert(string.find(hud, 'conceptShowcase.Name = "ConceptShowcase"', 1, true), "concept reference layer missing")
assert(
	string.find(hud, "conceptShowcase.Visible = false", 1, true),
	"concept montage must never be permanently visible during gameplay"
)
assert(string.find(hud, "shop.Visible = false", 1, true), "shop panel must remain contextual")
assert(string.find(hud, "dailyPanel.Visible = false", 1, true), "daily rewards panel must remain contextual")
assert(string.find(hud, "result.Visible = false", 1, true), "revive/result panel must remain contextual")

return true
