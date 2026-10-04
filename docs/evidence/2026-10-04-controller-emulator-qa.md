# Rising Steps controller-emulator QA — 2026-10-04

## Scope

This evidence verifies the production movement/jump control path with Roblox Studio Controller Emulator. It does not claim genuine tablet touch input or a physical retail controller device.

## Harness

`InputQaController` now contains a Studio-only, default-disabled `Gamepad1` observer. `RuntimeQaConfig.GamepadQaEnabled` remains `false` in source. The observer records actual `UserInputService` gamepad events and then measures the Humanoid/root response instead of assuming that an emulator click worked.

For deterministic ButtonA triggering, the existing Studio Controller Emulator keyboard mapping was read from `com.roblox.RobloxStudio.plist`. Roblox enum values were independently queried inside Studio: `ButtonA=1002`, `Thumbstick1=1016`. The QA run temporarily mapped `ButtonA` from keyboard `9` to unused keyboard `G`; the original mapping was restored immediately after the run and verified back at encoded key `57`. No game source/control binding was changed for this mapping.

## Final run

Local QA build: `/tmp/risingsteps-gamepad-qa300.rbxlx`

Observed log sequence:

- `PASS gamepad_connected`
- `PASS gamepad_button_a_event`
- `PASS gamepad_jump rise=7.27`
- `PASS gamepad_thumbstick_event magnitude=1.00`
- `PASS gamepad_move distance=6.63 moveDirection=1.00`
- `[RisingStepsGamepadQA] COMPLETE connected=true stick=true move=true buttonA=true jump=true pass=true`

An earlier ordering test sent movement before jump and correctly produced ButtonA input but no vertical rise because the avatar had already left the safe start position. The final accepted run resets the session and sends ButtonA while grounded before testing Thumbstick1.

## Acceptance

- Standard Roblox `Gamepad1` movement: pass.
- Standard Roblox `ButtonA` jump: pass.
- Production character response: pass.
- Existing gamepad UI selection/focus implementation: present (`Selectable`, `GuiService.SelectedObject` routes for Retry/Shop/Close).
- P02-T06 controller/keyboard parity: accepted.
- P14-T04 Controller: accepted.
- P14-T02 Tablet touch: remains open because Device Simulator visual/touch-control rendering is not equivalent to genuine touch interaction evidence.
