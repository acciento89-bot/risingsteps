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

function ViewportRules.topRightHudOffsets(width: number, touchEnabled: boolean): (number, number)
	if not touchEnabled then
		-- Desktop/controller sessions can expose Roblox CoreGui player-list UI in
		-- the top-right even when Studio or a resized window narrows the viewport.
		return 248, 18
	end

	local class = ViewportRules.classify(width)
	if class == "compact" then
		return 12, 126
	elseif class == "tablet" then
		return 16, 16
	end

	-- Large touch screens have no desktop player-list reservation requirement.
	return 16, 16
end

return table.freeze(ViewportRules)
