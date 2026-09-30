local PersistenceRules = require("../src/shared/rules/PersistenceRules")

assert(PersistenceRules.shouldClearDirty(true, 4, 4), "stable successful save should clear dirty state")
assert(
	not PersistenceRules.shouldClearDirty(true, 4, 5),
	"mutation during save must keep profile dirty for a follow-up flush"
)
assert(not PersistenceRules.shouldClearDirty(false, 4, 4), "failed save must keep profile dirty")

return true
