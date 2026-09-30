local MonetizationConfig = require("../src/shared/config/MonetizationConfig")
local ReceiptRules = require("../src/shared/rules/ReceiptRules")

assert(ReceiptRules.productKeyForId(0) == nil, "placeholder product id must never resolve")
assert(not ReceiptRules.contains({ "a", "b" }, "c"), "unknown receipt was reported as processed")
assert(ReceiptRules.contains({ "a", "b" }, "a"), "known receipt was not found")

local first = ReceiptRules.append({ "a", "b" }, "c")
assert(#first == 3 and first[3] == "c", "receipt append failed")

local duplicate = ReceiptRules.append(first, "c")
assert(#duplicate == #first, "duplicate receipt changed receipt count")

local many = {}
for index = 1, MonetizationConfig.MaxProcessedReceipts + 5 do
	many = ReceiptRules.append(many, tostring(index))
end
assert(#many == MonetizationConfig.MaxProcessedReceipts, "receipt history was not bounded")
assert(many[1] == "6", "bounded receipt history did not evict oldest entries")

return true
