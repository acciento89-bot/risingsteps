local CourseConfig = require("../src/shared/config/CourseConfig")
local GameConfig = require("../src/shared/config/GameConfig")
local ReachabilityRules = require("../src/shared/rules/ReachabilityRules")

local maxRise = ReachabilityRules.maxJumpRise(GameConfig.Movement.JumpPower, GameConfig.Movement.Gravity)
assert(maxRise > GameConfig.Step.MaxVerticalRise, "configured max rise exceeds the physical jump apex")

local flightTime =
	ReachabilityRules.descendingFlightTime(GameConfig.Movement.JumpPower, GameConfig.Movement.Gravity, 5.0)
assert(flightTime ~= nil and flightTime > 0, "high-band jump must have a descending landing solution")

local reachable = ReachabilityRules.isReachable(
	GameConfig.Step.MaxHorizontalCenterDistance,
	GameConfig.Step.MaxVerticalRise,
	GameConfig.Movement.WalkSpeed,
	GameConfig.Movement.JumpPower,
	GameConfig.Movement.Gravity,
	GameConfig.Step.BaseFootprint,
	GameConfig.Step.BaseFootprint,
	CourseConfig.ReachabilityControlSafetyFactor
)
assert(reachable, "hard-cap step must remain reachable with the conservative control factor")

local impossibleRise = maxRise + 0.25
assert(
	not ReachabilityRules.isReachable(
		4,
		impossibleRise,
		GameConfig.Movement.WalkSpeed,
		GameConfig.Movement.JumpPower,
		GameConfig.Movement.Gravity,
		GameConfig.Step.BaseFootprint,
		GameConfig.Step.BaseFootprint,
		CourseConfig.ReachabilityControlSafetyFactor
	),
	"step above the physical jump apex was accepted"
)

return true
