--!strict

local RemoteConfig = {
	FolderName = "Remotes",
	Definitions = table.freeze({
		PlayerCommand = table.freeze({
			ClassName = "RemoteEvent",
			Direction = "ClientToServer",
			ClientMayMutateScore = false,
			ClientMayGrantCurrency = false,
			ClientMayGrantPurchase = false,
		}),
		StateSnapshot = table.freeze({
			ClassName = "RemoteEvent",
			Direction = "ServerToClient",
			ClientMayMutateScore = false,
			ClientMayGrantCurrency = false,
			ClientMayGrantPurchase = false,
		}),
		ShopCommand = table.freeze({
			ClassName = "RemoteEvent",
			Direction = "ClientToServer",
			ClientMayMutateScore = false,
			ClientMayGrantCurrency = false,
			ClientMayGrantPurchase = false,
		}),
		RequestBootstrap = table.freeze({
			ClassName = "RemoteFunction",
			Direction = "ClientToServer",
			ClientMayMutateScore = false,
			ClientMayGrantCurrency = false,
			ClientMayGrantPurchase = false,
		}),
	}),
}

return table.freeze(RemoteConfig)
