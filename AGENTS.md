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


## Store icon accuracy gate — 2026-10-15

The user explicitly requires icon accuracy checks for every app after Apple cited guideline2.3 for One More Floor1.0(6). Before any new TestFlight/App Review upload, compare the assigned icon with actual gameplay, character, activity and world; do not approve a beautiful illustration that advertises a different interaction. Verify the exported native store icon and small/large variants use the intended same design. Record exact Unity source and source/native PNG/gameplay hashes, reviewer, date and decision in `docs/QA/store-icon-review.json`. `scripts/verify-visual-release.py` now fails closed for pending/failed/stale icon reviews as well as graphical reviews. Do not interpret PNG size or a passed gameplay test as icon-content approval. Complete this review from evidence; no new user permission is required. Existing public submissions are unchanged by this future-upload guard.
