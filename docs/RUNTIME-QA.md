# Rising Steps — Final Runtime Acceptance Matrix

This is the execution script for P13/P14. A green CI run is not a substitute for this runtime pass.

## Canonical local runtime workflow
- Use exactly one local place: `~/Documents/Roblox/RisingSteps/risingsteps-final2.rbxlx`.
- Do not launch QA from `/tmp` or generate parallel `QA`, `final2-copy`, `mainQA` or similar place files.
- Before runtime QA, verify there is no stale `rojo serve` process rooted in an old temporary checkout.
- Runtime acceptance is performed from the canonical place only.
- Do not force-terminate Roblox Studio during QA. Stop play normally and close Studio normally only when required.
- A process restart caused by the QA harness is not evidence of a game crash; real crash acceptance requires a script/runtime error, crash report or unexplained process exit while the canonical session is otherwise untouched.

## Fresh-session journey
1. Join with a fresh profile.
2. Confirm spawn is on the rear of the authored launch bay and faces the first three guided steps.
3. Confirm avatar is visible immediately; no editor/map camera frame appears.
4. Complete tutorial and verify the first dangerous expiry is telegraphed before collision disappears.
5. Build Clean/Perfect streak and deliberately break it.
6. Reach multiple difficulty/art bands.
7. Fall, reach result UI, Retry, and confirm camera/control recovery.
8. Exercise cosmetic earn/buy/equip path.
9. Exercise Revive from a validated step.
10. Leave and rejoin; confirm PB, coins, ownership, equipment and settings persist.

## Visual states to capture
- spawn/tutorial
- early climb
- warning/expiry
- Perfect landing
- failure/result
- shop/cosmetics
- mid climb
- high altitude

## Device matrix
- compact phone touch
- tablet touch
- desktop keyboard/mouse
- controller
- low graphics quality
- normal/high graphics quality

## Acceptance checks
- avatar + next 2–3 reachable steps readable
- HUD never covers avatar/current/next step
- no sticky edges or camera clipping
- no placeholder/default UI
- no empty-baseplate view
- no obvious copy-paste repetition
- no z-fighting/floating props/seams
- warning readable without color alone
- reduced-motion disables pose/ring motion cleanly
- no audio clipping/fatiguing cue stack
- lane BasePartCount <= 900
- lane BeamAndLightCount <= 96

## Stability run
Run continuously for 20 minutes while climbing/retrying. Record:
- starting and ending memory
- BasePartCount and BeamAndLightCount
- any Output errors/warnings
- camera/control recovery after multiple deaths
- any geometry or VFX accumulation

## Release evidence
Only after the complete matrix passes:
- record commit SHA
- record Roblox place version
- record rollback target
- retain curated screenshots under docs/evidence/
- then move runtime-gated ledger items from [~]/[!] to [x]
