local fs = require("@lune/fs")

local hud = fs.readFile("src/client/controllers/HudController.luau")
local art = fs.readFile("docs/ART-DIRECTION.md")

assert(string.find(hud, "shop.Visible = false", 1, true), "shop must be hidden until the Shop action is pressed")
assert(string.find(hud, 'dailyPanel.Name = "DailyRewardsPanel"', 1, true), "daily rewards need a dedicated modal panel")
assert(string.find(hud, "dailyPanel.Visible = false", 1, true), "daily rewards must be hidden by default")
assert(string.find(hud, "result.Visible = false", 1, true), "revive/result menu must be hidden until failure")
assert(string.find(hud, "setDailyVisible(true)", 1, true), "Daily action must explicitly open the daily modal")
assert(string.find(hud, "setDailyVisible(false)", 1, true), "daily modal must have an explicit close path")
assert(
	string.find(art, "feature montage", 1, true),
	"art direction must state that the concept screenshot is not a simultaneous HUD layout"
)

return true
