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
