local GraphicsQualityRules = require("../src/shared/rules/GraphicsQualityRules")

assert(GraphicsQualityRules.isLow(0) == false, "automatic quality must remain engine-managed")
assert(GraphicsQualityRules.isLow(1) == true, "quality level 1 must use low presentation")
assert(GraphicsQualityRules.isLow(2) == true, "quality level 2 must use low presentation")
assert(GraphicsQualityRules.isLow(3) == true, "quality level 3 must use low presentation")
assert(GraphicsQualityRules.isLow(4) == false, "quality level 4 must retain standard presentation")
assert(GraphicsQualityRules.isLow(10) == false, "high quality must retain standard presentation")

local low = GraphicsQualityRules.profile(2)
assert(low.Low == true, "low profile flag missing")
assert(low.DecorativeBeamsEnabled == false, "low profile must disable decorative beams")
assert(low.DecorativePointLightsEnabled == false, "low profile must disable decorative point lights")
assert(low.BloomEnabled == false, "low profile must disable bloom")

local standard = GraphicsQualityRules.profile(7)
assert(standard.Low == false, "standard profile flag incorrect")
assert(standard.DecorativeBeamsEnabled == true, "standard profile must keep beams")
assert(standard.DecorativePointLightsEnabled == true, "standard profile must keep point lights")
assert(standard.BloomEnabled == true, "standard profile must keep bloom")

return true
