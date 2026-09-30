local StepRules = require("../src/shared/rules/StepRules")

assert(StepRules.canTransition("Active", "Warning"), "Active must transition to Warning")
assert(StepRules.canTransition("Warning", "Expiring"), "Warning must transition to Expiring")
assert(StepRules.canTransition("Expiring", "Expired"), "Expiring must transition to Expired")
assert(not StepRules.canTransition("Active", "Expired"), "Active must not skip directly to Expired")
assert(not StepRules.canTransition("Expired", "Active"), "Expired steps must not reactivate implicitly")

assert(StepRules.isExpectedLanding(3, 3, 12, 11.5, -8), "valid downward landing was rejected")
assert(StepRules.isExpectedLanding(3, 3, 12, 11.5, 2.5), "near-neutral valid landing was rejected")
assert(not StepRules.isExpectedLanding(3, 4, 12, 11.5, -8), "out-of-sequence landing was accepted")
assert(not StepRules.isExpectedLanding(3, 3, 10.5, 11.5, -8), "landing below the step top was accepted")
assert(not StepRules.isExpectedLanding(3, 3, 12, 11.5, 8), "rising contact was accepted as a landing")

return true
