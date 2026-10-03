--!strict

local DifficultyConfig = require("../config/DifficultyConfig")

export type StepDefinition = {
	Id: number,
	X: number,
	Y: number,
	Z: number,
	BandId: string,
	Variant: number,
}

local StepSequence = {}

local MODULUS = 2147483647
local MULTIPLIER = 48271
local DIRECTIONS = {
	{ X = 1.0, Z = 0.0 },
	{ X = 0.70710678, Z = 0.70710678 },
	{ X = 0.0, Z = 1.0 },
	{ X = -0.70710678, Z = 0.70710678 },
	{ X = -1.0, Z = 0.0 },
	{ X = -0.70710678, Z = -0.70710678 },
	{ X = 0.0, Z = -1.0 },
	{ X = 0.70710678, Z = -0.70710678 },
}

local function normalizeSeed(seed: number): number
	local normalized = math.floor(math.abs(seed)) % (MODULUS - 1)
	return normalized + 1
end

local function nextRandom(state: number): (number, number)
	local nextState = (state * MULTIPLIER) % MODULUS
	return nextState, nextState / MODULUS
end

local function lerp(minimum: number, maximum: number, alpha: number): number
	return minimum + (maximum - minimum) * alpha
end

function StepSequence.bandForHeight(height: number)
	for _, band in DifficultyConfig.Bands do
		if height >= band.MinHeight and height <= band.MaxHeight then
			return band
		end
	end
	error(("no difficulty band configured for height %d"):format(height))
end

function StepSequence.generate(seed: number, count: number, startY: number, variantCount: number): { StepDefinition }
	assert(count >= 1, "step count must be positive")
	assert(variantCount >= 1, "variant count must be positive")

	local state = normalizeSeed(seed)
	local x = 0
	local y = startY
	local z = 0
	local previousDirection = 0
	local result = table.create(count)

	for id = 1, count do
		local band = StepSequence.bandForHeight(id - 1)

		local distanceAlpha
		state, distanceAlpha = nextRandom(state)
		local verticalAlpha
		state, verticalAlpha = nextRandom(state)
		local directionAlpha
		state, directionAlpha = nextRandom(state)

		local horizontalDistance = lerp(band.HorizontalMin, band.HorizontalMax, distanceAlpha)
		local verticalRise = lerp(band.VerticalMin, band.VerticalMax, verticalAlpha)
		local directionIndex = math.floor(directionAlpha * #DIRECTIONS) + 1

		if id <= 3 then
			-- The opening run is authored: the first target clears the spawn capsule,
			-- then the next two stay deliberately short and straight.
			horizontalDistance = if id == 1
				then math.max(6.4, band.HorizontalMax)
				else math.min(band.HorizontalMax, band.HorizontalMin + 0.35 + id * 0.08)
			verticalRise = math.min(band.VerticalMax, band.VerticalMin + 0.10 + (id - 1) * 0.08)
			directionIndex = if id == 1 then 7 else if id == 2 then 8 else 1
		elseif previousDirection > 0 then
			-- Keep the climb readable from below: continue mostly forward and only
			-- bend by 45/90 degrees. The old fully-random direction selection
			-- repeatedly crossed back over recent steps and created dense towers.
			local turn = 0
			if directionAlpha < 0.12 then
				turn = -2
			elseif directionAlpha < 0.32 then
				turn = -1
			elseif directionAlpha > 0.88 then
				turn = 2
			elseif directionAlpha > 0.68 then
				turn = 1
			end
			directionIndex = ((previousDirection - 1 + turn) % #DIRECTIONS) + 1
		end
		previousDirection = directionIndex

		local direction = DIRECTIONS[directionIndex]
		x += direction.X * horizontalDistance
		z += direction.Z * horizontalDistance
		y += verticalRise

		result[id] = {
			Id = id,
			X = x,
			Y = y,
			Z = z,
			BandId = band.Id,
			Variant = ((id + directionIndex - 2) % variantCount) + 1,
		}
	end

	return result
end

return table.freeze(StepSequence)
