local fs = require("@lune/fs")

local steps = fs.readFile("src/server/services/StepCourseService.luau")
local deck = fs.readFile("src/server/services/StartDeckService.luau")
local world = fs.readFile("src/server/services/WorldArtService.luau")
local art = fs.readFile("docs/ART-DIRECTION.md")
local hud = fs.readFile("src/client/controllers/HudController.luau")

assert(string.find(steps, "collision.Transparency = 1", 1, true), "gameplay collision must be visually hidden")
assert(string.find(steps, "FloatingGrassTop", 1, true), "steps must use organic floating-island tops")
assert(string.find(steps, "RockShard", 1, true), "steps need layered hanging-rock silhouettes")
assert(string.find(steps, "RuneEdge", 1, true), "steps need fantasy rune edge detail")
assert(string.find(deck, "SpawnGrassCrown", 1, true), "spawn needs a circular grass crown")
assert(string.find(deck, "SkySapling", 1, true), "spawn island needs authored vegetation")
assert(string.find(world, "InitialVistaPortal", 1, true), "initial camera needs a visible portal landmark")
assert(string.find(world, "SkySunDisc", 1, true), "world needs a readable warm sky focal point")
assert(string.find(world, "HeroIslandLeft", 1, true), "initial vista needs a left hero island")
assert(string.find(world, "HeroIslandRight", 1, true), "initial vista needs a right hero island")
assert(string.find(world, "ForegroundCloudBank", 1, true), "initial vista needs layered foreground clouds")
assert(string.find(art, "Fantasy Sky Realms", 1, true), "art direction must document the approved fantasy concept")

assert(string.find(hud, 'logo.Name = "RisingStepsLogo"', 1, true), "HUD needs a branded Rising Steps logo block")
assert(string.find(hud, 'stageProgress.Name = "StageProgress"', 1, true), "HUD needs concept-style stage progress")
assert(string.find(hud, 'progressFill.Name = "ProgressFill"', 1, true), "stage progress needs a visible fill")
assert(string.find(hud, 'quickActions.Name = "QuickActions"', 1, true), "HUD needs compact vertical concept actions")
assert(string.find(hud, "dailyQuickButton", 1, true), "HUD needs a daily-reward quick action")

return true
