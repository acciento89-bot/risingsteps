--!strict

local ReachabilityRules = {}

function ReachabilityRules.maxJumpRise(jumpPower: number, gravity: number): number
	assert(jumpPower > 0, "jumpPower must be positive")
	assert(gravity > 0, "gravity must be positive")
	return (jumpPower * jumpPower) / (2 * gravity)
end

function ReachabilityRules.descendingFlightTime(jumpPower: number, gravity: number, verticalRise: number): number?
	assert(jumpPower > 0, "jumpPower must be positive")
	assert(gravity > 0, "gravity must be positive")

	local discriminant = jumpPower * jumpPower - 2 * gravity * verticalRise
	if discriminant < 0 then
		return nil
	end

	return (jumpPower + math.sqrt(discriminant)) / gravity
end

function ReachabilityRules.maxHorizontalCenterDistance(
	walkSpeed: number,
	jumpPower: number,
	gravity: number,
	verticalRise: number,
	sourceFootprint: number,
	targetFootprint: number,
	controlSafetyFactor: number
): number
	local flightTime = ReachabilityRules.descendingFlightTime(jumpPower, gravity, verticalRise)
	if not flightTime then
		return 0
	end

	local sourceHalfWidth = sourceFootprint / 2
	local targetHalfWidth = targetFootprint / 2
	local controlledTravel = walkSpeed * flightTime * controlSafetyFactor
	return sourceHalfWidth + targetHalfWidth + controlledTravel
end

function ReachabilityRules.isReachable(
	horizontalCenterDistance: number,
	verticalRise: number,
	walkSpeed: number,
	jumpPower: number,
	gravity: number,
	sourceFootprint: number,
	targetFootprint: number,
	controlSafetyFactor: number
): boolean
	if verticalRise < 0 then
		return false
	end

	local maxDistance = ReachabilityRules.maxHorizontalCenterDistance(
		walkSpeed,
		jumpPower,
		gravity,
		verticalRise,
		sourceFootprint,
		targetFootprint,
		controlSafetyFactor
	)
	return horizontalCenterDistance <= maxDistance
end

return table.freeze(ReachabilityRules)
