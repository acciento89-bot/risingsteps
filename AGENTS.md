# AGENTS.md

## Product
Rising Steps is a native Unity mobile game for iOS and Android.

## Execution
- Read README -> MASTER-PLAN -> ART-DIRECTION -> V1 LEDGER before implementation.
- Ledger is canonical.
- Portrait, touch-first, safe-area aware, 60 FPS target.
- Camera orbit is player-controlled; never force yaw behind the character every frame.
- Shop, Daily and Style are real contextual UI surfaces, not decorative labels.
- Do not add legacy platform/runtime files, Lua/Luau, Rojo or place files.
- All active UI must remain outside the primary jump route.
- Commit coherent verified slices to main.

## Quality
- Clear next 2-4 steps on a compact phone.
- Immediate recovery after fall.
- Touch targets >= 48 logical points.
- DE/EN before release.
- Reduced-motion and contrast pass before release.

## TestFlight visual release gate — 2026-10-11

The user requires actual Unity-rendered gameplay, menus, character/tower and world/design variants to resemble the approved concept before ANY new TestFlight upload. Compilation and functional tests alone do not satisfy this gate. Run `python3 scripts/verify-visual-release.py` before scripted or Xcode GUI distribution. A failed/pending/stale review blocks upload. Inspect actual rendered evidence against the approved board before recording a passed report in `docs/QA/visual-release-gate.json`; bind it to the exact Unity source fingerprint. Never mark illustrations or generated mockups as actual gameplay. All other release checks remain required.
