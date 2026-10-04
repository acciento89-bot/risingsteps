# Rising Steps desktop CoreGui layout QA — 2026-10-04

## Scope

Desktop/non-touch HUD placement after the tablet-camera and controller passes.

## Result

- `ViewportRules.topRightHudOffsets(width, touchEnabled)` now reserves the Roblox top-right player-list region for every non-touch session, including narrow Studio desktop viewports that would otherwise classify by width as tablet.
- Compact-phone and touch-tablet offsets remain unchanged.
- Real Studio PlaySolo screenshot review confirms the Coins pill and SHOP button are fully visible immediately left of the Roblox player list with no overlap.
- Static verification passes StyLua, Selene 0/0, all 16 pure-Luau test files, 16,000/16,000 reachability placements, release-readiness and Rojo build.
- P14-T03 remains intentionally `[~]`: keyboard movement/jump are already runtime-proven, but the final embedded-Studio OS-mouse activation path is not claimed as passed because macOS synthetic pointer events are not reliably delivered to the embedded PlayClient surface.
