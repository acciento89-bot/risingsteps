local SecurityRules = require("../src/shared/rules/SecurityRules")

assert(SecurityRules.isFiniteNumber(5), "normal finite number rejected")
assert(not SecurityRules.isFiniteNumber(0 / 0), "NaN accepted")
assert(not SecurityRules.isFiniteNumber(math.huge), "infinity accepted")
assert(SecurityRules.componentsWithinLimit(1, 2, 3, 100), "normal position rejected")
assert(not SecurityRules.componentsWithinLimit(101, 0, 0, 100), "extreme position accepted")
assert(SecurityRules.validText("Retry", 32), "normal remote action rejected")
assert(not SecurityRules.validText("", 32), "empty remote action accepted")
assert(not SecurityRules.validText(string.rep("x", 33), 32), "oversized remote action accepted")
assert(SecurityRules.intervalAllowed(10, 10.2, 0.18), "legal interval rejected")
assert(not SecurityRules.intervalAllowed(10, 10.1, 0.18), "rapid interval accepted")
assert(not SecurityRules.intervalAllowed(10, 9, 0.18), "time reversal accepted")

return true
