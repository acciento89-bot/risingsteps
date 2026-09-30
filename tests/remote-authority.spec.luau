local RemoteConfig = require("../src/shared/config/RemoteConfig")

local seen = {}
for name, definition in RemoteConfig.Definitions do
	assert(seen[name] == nil, "duplicate remote name")
	seen[name] = true
	assert(definition.ClassName == "RemoteEvent" or definition.ClassName == "RemoteFunction")
	assert(definition.Direction == "ClientToServer" or definition.Direction == "ServerToClient")
	assert(definition.ClientMayMutateScore == false)
	assert(definition.ClientMayGrantCurrency == false)
	assert(definition.ClientMayGrantPurchase == false)
end

assert(RemoteConfig.Definitions.StateSnapshot.Direction == "ServerToClient")
assert(RemoteConfig.Definitions.PlayerCommand.Direction == "ClientToServer")

return true
