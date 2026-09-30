--!strict

local SecurityRules = {}

function SecurityRules.isFiniteNumber(value: any): boolean
	return type(value) == "number" and value == value and value ~= math.huge and value ~= -math.huge
end

function SecurityRules.componentsWithinLimit(x: any, y: any, z: any, maxAbs: number): boolean
	if
		not SecurityRules.isFiniteNumber(x)
		or not SecurityRules.isFiniteNumber(y)
		or not SecurityRules.isFiniteNumber(z)
	then
		return false
	end

	return math.abs(x) <= maxAbs and math.abs(y) <= maxAbs and math.abs(z) <= maxAbs
end

function SecurityRules.validText(value: any, maxLength: number): boolean
	return type(value) == "string" and #value >= 1 and #value <= maxLength
end

function SecurityRules.intervalAllowed(previousAt: number, now: number, minimumInterval: number): boolean
	if not SecurityRules.isFiniteNumber(previousAt) or not SecurityRules.isFiniteNumber(now) then
		return false
	end
	return now >= previousAt and now - previousAt >= minimumInterval
end

return table.freeze(SecurityRules)
