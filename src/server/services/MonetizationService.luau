--!strict

local MarketplaceService = game:GetService("MarketplaceService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")

local root = ReplicatedStorage:WaitForChild("RisingSteps")
local MonetizationConfig = require(root.config.MonetizationConfig)
local ReceiptRules = require(root.rules.ReceiptRules)
local PlayerProfileService = require(script.Parent.PlayerProfileService)

local MonetizationService = {}

local function refreshCoinMultiplier(player: Player)
	local pass = MonetizationConfig.GamePasses.CoinMultiplier
	local multiplier = 1

	if pass.GamePassId > 0 then
		local ok, owns = pcall(function()
			return MarketplaceService:UserOwnsGamePassAsync(player.UserId, pass.GamePassId)
		end)
		if ok and owns then
			multiplier = pass.Multiplier
		end
	end

	player:SetAttribute("RisingStepsCoinMultiplier", multiplier)
end

function MonetizationService.start()
	MarketplaceService.ProcessReceipt = function(receiptInfo)
		local productKey = ReceiptRules.productKeyForId(receiptInfo.ProductId)
		if not productKey then
			return Enum.ProductPurchaseDecision.NotProcessedYet
		end

		local player = Players:GetPlayerByUserId(receiptInfo.PlayerId)
		if not player then
			return Enum.ProductPurchaseDecision.NotProcessedYet
		end

		local ok = PlayerProfileService.grantDeveloperProduct(player, productKey, tostring(receiptInfo.PurchaseId))
		if ok then
			return Enum.ProductPurchaseDecision.PurchaseGranted
		end
		return Enum.ProductPurchaseDecision.NotProcessedYet
	end

	for _, player in Players:GetPlayers() do
		task.spawn(refreshCoinMultiplier, player)
	end
	Players.PlayerAdded:Connect(function(player)
		task.spawn(refreshCoinMultiplier, player)
	end)
end

return MonetizationService
