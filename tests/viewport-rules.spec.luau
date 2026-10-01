local ViewportRules = require("../src/shared/rules/ViewportRules")

assert(ViewportRules.classify(414) == "compact", "compact phone classification failed")
assert(ViewportRules.classify(760) == "compact", "compact upper boundary failed")
assert(ViewportRules.classify(761) == "tablet", "tablet lower boundary failed")
assert(ViewportRules.classify(900) == "tablet", "tablet classification failed")
assert(ViewportRules.classify(1100) == "tablet", "tablet upper boundary failed")
assert(ViewportRules.classify(1101) == "desktop", "desktop lower boundary failed")
assert(ViewportRules.classify(1440) == "desktop", "desktop classification failed")

assert(math.abs(ViewportRules.uiScale(320) - 0.82) < 0.001, "small viewport must clamp scale")
assert(math.abs(ViewportRules.uiScale(900) - 1.0) < 0.001, "900 width must be baseline scale")
assert(math.abs(ViewportRules.uiScale(1440) - 1.08) < 0.001, "large viewport must clamp scale")

return true
