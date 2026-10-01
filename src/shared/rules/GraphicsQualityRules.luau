--!strict

local GraphicsQualityRules = {}

local LOW_QUALITY_MAX = 3

function GraphicsQualityRules.isLow(savedQualityValue: number): boolean
	-- Roblox uses 0 for Automatic. Automatic must not be forced into the low
	-- presentation path because the engine may dynamically choose a higher tier.
	return savedQualityValue >= 1 and savedQualityValue <= LOW_QUALITY_MAX
end

function GraphicsQualityRules.profile(savedQualityValue: number)
	local low = GraphicsQualityRules.isLow(savedQualityValue)
	return table.freeze({
		Low = low,
		DecorativeBeamsEnabled = not low,
		DecorativePointLightsEnabled = not low,
		BloomEnabled = not low,
	})
end

return table.freeze(GraphicsQualityRules)
