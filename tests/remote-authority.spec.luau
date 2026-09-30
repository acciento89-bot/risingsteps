local RemoteConfig = require("../src/shared/config/RemoteConfig")

local seen = {}
for name, definition in RemoteConfig.Definitions do
	assert(seen[name] == nil, "duplicate remote name: " .. name)
	seen[name] = true
	assert(
		definition.ClassName == "RemoteEvent" or definition.ClassName == "RemoteFunction",
		"unsupported remote class: " .. tostring(definition.ClassName)
	)
	assert(
		definition.Direction == "ClientToServer" or definition.Direction == "ServerToClient",
		"invalid remote direction: " .. tostring(definition.Direction)
	)
	assert(definition.ClientMayMutateScore == false, name .. " must not let the client mutate score")
	assert(definition.ClientMayGrantCurrency == false, name .. " must not let the client grant currency")
	assert(definition.ClientMayGrantPurchase == false, name .. " must not let the client grant purchases")
end

assert(
	RemoteConfig.Definitions.StateSnapshot.Direction == "ServerToClient",
	"StateSnapshot must remain server-to-client"
)
assert(
	RemoteConfig.Definitions.PlayerCommand.Direction == "ClientToServer",
	"PlayerCommand must remain client-to-server"
)

return true
