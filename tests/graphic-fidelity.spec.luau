local fs = require("@lune/fs")

local world = fs.readFile("src/server/services/WorldArtService.luau")
local steps = fs.readFile("src/server/services/StepCourseService.luau")
local art = fs.readFile("docs/ART-DIRECTION.md")

assert(string.find(world, 'distantChain.Name = "DistantIslandChain"', 1, true), "distant island chain missing")
assert(string.find(world, '"SkySunHalo"', 1, true), "layered sun halo missing")
assert(string.find(world, '"SkyBridgeRuin"', 1, true), "sky bridge ruin missing")
assert(string.find(world, '"CloudRibbon"', 1, true), "cloud ribbon depth cue missing")
assert(string.find(steps, '"EdgeMossLip"', 1, true), "step edge moss detail missing")
assert(string.find(steps, '"HangingCrystal"', 1, true), "high-step hanging crystal detail missing")
assert(string.find(art, "parallax sky-island identity", 1, true), "parallax sky-island identity missing")

return true
