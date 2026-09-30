--!strict

local HttpService = game:GetService("HttpService")

local DiagnosticsService = {}

local function encode(eventName: string, fields: any): string
	local payload = {
		Event = eventName,
		Fields = if type(fields) == "table" then fields else {},
		Timestamp = os.time(),
	}

	local ok, encoded = pcall(function()
		return HttpService:JSONEncode(payload)
	end)
	if ok then
		return encoded
	end
	return eventName
end

function DiagnosticsService.info(eventName: string, fields: any?)
	print("[RisingSteps] " .. encode(eventName, fields))
end

function DiagnosticsService.warn(eventName: string, fields: any?)
	warn("[RisingSteps] " .. encode(eventName, fields))
end

return DiagnosticsService
