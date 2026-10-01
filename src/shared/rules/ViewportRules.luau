--!strict

local ViewportRules = {}

function ViewportRules.classify(width: number): string
	if width <= 760 then
		return "compact"
	elseif width <= 1100 then
		return "tablet"
	end
	return "desktop"
end

function ViewportRules.uiScale(width: number): number
	return math.clamp(width / 900, 0.82, 1.08)
end

return table.freeze(ViewportRules)
