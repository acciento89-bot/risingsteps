--!strict

local StepRules = {}

StepRules.State = table.freeze({
	Active = "Active",
	Warning = "Warning",
	Expiring = "Expiring",
	Expired = "Expired",
})

local ALLOWED_TRANSITIONS = {
	Active = { Warning = true },
	Warning = { Expiring = true },
	Expiring = { Expired = true },
	Expired = {},
}

function StepRules.canTransition(fromState: string, toState: string): boolean
	local allowed = ALLOWED_TRANSITIONS[fromState]
	return allowed ~= nil and allowed[toState] == true
end

function StepRules.isExpectedLanding(
	expectedStepId: number,
	touchedStepId: number,
	rootY: number,
	stepTopY: number,
	verticalVelocityY: number
): boolean
	if touchedStepId ~= expectedStepId then
		return false
	end
	if rootY < stepTopY - 0.35 then
		return false
	end
	if verticalVelocityY > 3 then
		return false
	end
	return true
end

return table.freeze(StepRules)
