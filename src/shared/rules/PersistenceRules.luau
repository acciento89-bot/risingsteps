--!strict

local PersistenceRules = {}

function PersistenceRules.shouldClearDirty(saveSucceeded: boolean, revisionAtStart: number, currentRevision: number): boolean
	return saveSucceeded and revisionAtStart == currentRevision
end

return table.freeze(PersistenceRules)
