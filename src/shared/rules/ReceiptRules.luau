--!strict

local MonetizationConfig = require("../config/MonetizationConfig")

local ReceiptRules = {}

function ReceiptRules.productKeyForId(productId: number): string?
	if productId <= 0 then
		return nil
	end

	for productKey, product in MonetizationConfig.DeveloperProducts do
		if product.ProductId == productId and product.ProductId > 0 then
			return productKey
		end
	end
	return nil
end

function ReceiptRules.contains(processedReceipts: { string }, purchaseId: string): boolean
	for _, existing in processedReceipts do
		if existing == purchaseId then
			return true
		end
	end
	return false
end

function ReceiptRules.append(processedReceipts: { string }, purchaseId: string): { string }
	if ReceiptRules.contains(processedReceipts, purchaseId) then
		return table.clone(processedReceipts)
	end

	local result = table.clone(processedReceipts)
	table.insert(result, purchaseId)
	while #result > MonetizationConfig.MaxProcessedReceipts do
		table.remove(result, 1)
	end
	return result
end

return table.freeze(ReceiptRules)
