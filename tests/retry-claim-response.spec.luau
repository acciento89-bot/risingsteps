local fs = require("@lune/fs")

local course = fs.readFile("src/server/services/StepCourseService.luau")
local hud = fs.readFile("src/client/controllers/HudController.luau")

assert(
	string.find(course, "local function restartFailedCourse", 1, true),
	"retry needs a dedicated immediate restart path"
)
assert(
	string.find(course, "resetCourse(course)", 1, true),
	"retry restart must reset authoritative run state immediately"
)
assert(
	string.find(course, 'if payload.Action == "Retry" then\n\t\t\trestartFailedCourse(course)', 1, true),
	"Retry remote must use the immediate restart path"
)
assert(
	not string.find(course, "task.delay(CourseConfig.RespawnResetDelaySeconds", 1, true),
	"failure must not wait on the old delayed auto-reset"
)

assert(string.find(hud, 'retryButton.Text = "RESTARTING..."', 1, true), "Retry must acknowledge the click immediately")
assert(string.find(hud, 'then "CLAIMING..."', 1, true), "daily claim must show immediate pending feedback")
assert(
	string.find(hud, "local function requestDailyClaim()", 1, true),
	"daily claim needs one shared guarded request path"
)
assert(
	string.find(hud, "requestBootstrap:InvokeServer()", 1, true),
	"daily claim needs a bootstrap recovery/resync path"
)

return true
