local fs = require("@lune/fs")

local source = fs.readFile("src/client/controllers/CameraController.luau")

assert(
	string.find(source, "camera.CameraType = Enum.CameraType.Custom", 1, true),
	"Rising Steps camera must use the normal player-controlled Roblox camera"
)
assert(
	not string.find(source, "Enum.CameraType.Scriptable", 1, true),
	"camera must never force Scriptable follow/yaw during gameplay"
)
assert(
	not string.find(source, "camera.CFrame =", 1, true),
	"camera controller must not overwrite player camera rotation"
)
assert(
	not string.find(source, "RenderStepped", 1, true),
	"camera controller must not run a per-frame forced follow loop"
)

return true
