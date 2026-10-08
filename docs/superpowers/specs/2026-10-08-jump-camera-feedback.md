# Rising Steps physical jump-camera feedback

The user reports that jumping automatically turns the view/player and makes platforms difficult to reach. The approved product and AGENTS.md require player-owned camera orbit. Repair internal technical delivery was prioritized first and is now backed up/uploaded; its external acceptance remains open.

Restore the approved behavior: jumping, strafing and smooth position-follow must not independently change rendered camera yaw or pitch. Intentional player orbit remains available, including after jump/pause/restart. Keep existing jump/gravity/speed, authored route, progression, artwork and touch ownership.

Current hypothesis: OrbitCamera stores player-owned yaw/pitch, but LookRotation toward the moving target after damping camera position changes the rendered orientation; PlayerMotor then uses that rendered forward/right as input basis. Existing QA checks only stored yaw and corrects its joystick axis every frame, hiding fixed-thumb drift. Reproduce in the actual engine before a production fix. Also verify actual raycast ownership of jump/joystick and preserve pause/input behavior.

Acceptance: actual controller jump with fixed joystick produces no unsolicited rendered-angle change; intentional orbit changes the view; existing controller reachability/recovery/UI/progression checks remain green. Fresh native internal build, signing/backups and explicit physical feedback limitations required. Genuine commerce/privacy and official Duo hardware remain separate external gates.
