local GameConfig = require("../src/shared/config/GameConfig")
local CourseConfig = require("../src/shared/config/CourseConfig")
local StepSequence = require("../src/shared/rules/StepSequence")

local function distance2D(a, b)
	local dx = a.X - b.X
	local dz = a.Z - b.Z
	return math.sqrt(dx * dx + dz * dz)
end

local first = StepSequence.generate(123456, 120, 4.6, CourseConfig.VisualVariantCount)
local second = StepSequence.generate(123456, 120, 4.6, CourseConfig.VisualVariantCount)

assert(#first == 120, "generator returned the wrong number of steps")
assert(#second == #first, "deterministic replay changed step count")

local previous = { X = 0, Y = 4.6, Z = 0 }
for index, step in first do
	local replay = second[index]
	assert(step.Id == index, "step ids must be contiguous")
	assert(step.X == replay.X and step.Y == replay.Y and step.Z == replay.Z, "same seed must reproduce positions")
	assert(step.BandId == replay.BandId, "same seed must reproduce difficulty bands")
	assert(step.Variant == replay.Variant, "same seed must reproduce visual variants")
	assert(step.Variant >= 1 and step.Variant <= CourseConfig.VisualVariantCount, "variant outside configured range")

	local band = StepSequence.bandForHeight(index - 1)
	local horizontal = distance2D(step, previous)
	local vertical = step.Y - previous.Y
	assert(horizontal >= band.HorizontalMin - 0.001, "horizontal distance below band minimum")
	assert(horizontal <= GameConfig.Step.MaxHorizontalCenterDistance + 0.001, "horizontal distance exceeds hard cap")
	assert(vertical >= band.VerticalMin - 0.001, "vertical rise below band minimum")
	assert(vertical <= GameConfig.Step.MaxVerticalRise + 0.001, "vertical rise exceeds hard cap")

	previous = step
end

return true
