local CourseConfig = require("../src/shared/config/CourseConfig")
local GameConfig = require("../src/shared/config/GameConfig")
local ReachabilityRules = require("../src/shared/rules/ReachabilityRules")
local StepSequence = require("../src/shared/rules/StepSequence")

local seeds = { 1, 7, 91, 2026, 4099, 88001, 456789, 1900003 }
local stepsPerSeed = 2000
local totalChecked = 0

local function horizontalDistance(a, b)
	local dx = a.X - b.X
	local dz = a.Z - b.Z
	return math.sqrt(dx * dx + dz * dz)
end

for _, seed in seeds do
	local startY = 4.6
	local sequence = StepSequence.generate(seed, stepsPerSeed, startY, CourseConfig.VisualVariantCount)
	assert(#sequence == stepsPerSeed, "simulation generator returned the wrong number of steps")

	local previous = { X = 0, Y = startY, Z = 0 }
	for index, step in sequence do
		local horizontal = horizontalDistance(step, previous)
		local vertical = step.Y - previous.Y
		local band = StepSequence.bandForHeight(index - 1)

		assert(vertical > GameConfig.Step.CollisionThickness, "step vertical separation would physically overlap")
		assert(horizontal >= band.HorizontalMin - 0.001, "generated horizontal distance below band minimum")
		assert(
			horizontal <= GameConfig.Step.MaxHorizontalCenterDistance + 0.001,
			"generated horizontal distance above hard cap"
		)
		assert(vertical <= GameConfig.Step.MaxVerticalRise + 0.001, "generated vertical rise above hard cap")
		assert(
			ReachabilityRules.isReachable(
				horizontal,
				vertical,
				GameConfig.Movement.WalkSpeed,
				GameConfig.Movement.JumpPower,
				GameConfig.Movement.Gravity,
				GameConfig.Step.BaseFootprint,
				GameConfig.Step.BaseFootprint,
				CourseConfig.ReachabilityControlSafetyFactor
			),
			("unreachable placement at seed %d step %d"):format(seed, index)
		)

		previous = step
		totalChecked += 1
	end
end

assert(totalChecked == #seeds * stepsPerSeed, "simulation did not inspect every generated step")
print(("Rising Steps simulation: %d placements passed reachability checks"):format(totalChecked))

return true
