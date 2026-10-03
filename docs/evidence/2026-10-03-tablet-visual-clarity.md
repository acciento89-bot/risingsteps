# Rising Steps tablet visual-clarity QA — 2026-10-03

## Scope

Follow-up after the mobile opening-density correction. The iPad 10th Generation Device Simulator (`1180x820`) exposed a second readability issue: the jump spacing was correct, but the dark platform bodies and Service Decks skyline visually merged behind the route.

## Corrective design pass

- `GameConfig.Step.CollisionThickness`: `1.25 -> 0.78` studs.
- Landing footprint, step-center spacing, jump power, visibility count and progression rules are unchanged.
- Regression guard: collision thickness must remain between `0.65` and `0.80` studs.
- Service Decks city masses moved farther back, spread laterally and use lighter values so they read as background architecture rather than climb geometry.

## Verification

- Device Simulator: iPad 10th Generation, `1180x820`.
- HUD/top-left status, centered tutorial, Shop/currency cluster and bottom touch controls remain separated and readable.
- First reachable platform is visually isolated from the skyline and the four-target route no longer presents as one stacked dark mass.
- StyLua: pass.
- Selene: 0 errors / 0 warnings / 0 parse errors.
- Pure-Luau tests: 16 files passed.
- Reachability simulation: 16,000 / 16,000 placements passed.
- Release-readiness: pass.
- Rojo canonical build: pass.
- Runtime: 0 CreatorErrors; 846 BaseParts / 900 budget; 21 Beams+Lights / 96 budget.

## Tablet touch status

Visual/touch-target layout is accepted, but touch interaction is intentionally not marked complete. macOS pointer injection did not enter Roblox's touch path, and `VirtualInputManager:SendTouchEvent` is RobloxScript-security restricted from the Studio CommandBar. P14-T02 remains open until a real touch-capable run or another authoritative Roblox touch-input path is captured.
