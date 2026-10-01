# Rising Steps — Store-art review and clean-capture gate

Date: 2026-10-01

## Reviewed prepared assets

- `icon-512.png`
- `thumbnail-1-core.png`
- `thumbnail-2-height.png`
- `thumbnail-3-shop.png`

## Review result

The icon composition is usable as a production candidate: the avatar and authored rising-step mechanic are immediately visible and there is no placeholder text.

The three prepared thumbnails are **not final store assets yet**. They still contain Roblox CoreGui/topbar/player-list chrome in the captured frame. This does not invalidate the gameplay/runtime visual QA, but it fails the stricter store-art presentation gate.

Therefore:

- P15-T01 remains in progress.
- P15-T05A remains open.
- P15-T05B remains open.
- No existing thumbnail is being falsely accepted as a final uploaded store screenshot.

## Remediation implemented

Commit `513863ae94d044661930bf9956a4c36788b9bccf` adds a Studio + VisualOnly-gated capture controller.

When the existing VisualOnly QA mode is deliberately enabled:

- Roblox CoreGui types are disabled for capture.
- Topbar hiding is retried while CoreGui initializes.
- Normal players are unaffected.
- Normal Studio sessions with runtime QA disabled are unaffected.
- No gameplay geometry, HUD, camera, collision or monetization behavior changes.

## Exact-commit verification

The exact commit was verified from a fresh source archive:

- StyLua: pass
- Selene: 0 errors / 0 warnings / 0 parse errors
- Pure Luau tests: 16 files passed
- Reachability simulation: 16,000 placements passed
- Release-readiness: pass
- Rojo build: pass

Final store screenshots still require a clean VisualOnly capture from the accepted build and Creator Dashboard upload.
