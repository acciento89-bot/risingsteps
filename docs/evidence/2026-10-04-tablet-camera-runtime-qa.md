# Rising Steps tablet camera/readability QA — 2026-10-04

## Scope

This pass closes the remaining tablet portion of the low/high-altitude camera/readability gate. Compact-phone and desktop low/mid/high visual evidence already existed; this run adds exact iPad 10th Generation evidence from the current accepted source.

## QA build and viewport

- Source baseline: `8e61d8a` (`main` at build time).
- Local QA-only build: `/tmp/risingsteps-tablet-camera-qa.rbxlx`.
- Temporary build-only override: existing `RuntimeQaConfig` VisualOnly harness enabled with three-second holds.
- Source configuration was restored immediately after build generation; repository remained clean.
- Roblox Studio Device Simulator: **iPad 10th Generation, 1180×820**.

## Runtime sequence

The VisualOnly harness completed:

- Shop
- Cosmetics
- Tutorial
- Early
- Mid
- High
- Warning
- Result
- `VISUAL_COMPLETE`

CreatorErrors: **0**.

## Early / low-altitude acceptance

Evidence: `docs/evidence/2026-10-04-tablet-camera/early.png`

- Height 3 state is visibly framed inside the iPad simulator.
- Avatar remains readable at the bottom-center of the playfield.
- The next three reachable platforms remain visually separated rather than collapsing into a dark vertical mass.
- Cyan active-route accents remain readable against the Service Decks environment.
- Height/PB/streak and currency/Shop HUD groups stay outside the central jump line.
- Touch thumbstick/jump controls remain clear of the next-platform route.

## High-altitude acceptance

Evidence: `docs/evidence/2026-10-04-tablet-camera/high.png`

- Height 38 state remains readable in the same 1180×820 tablet composition.
- Avatar reference remains visible at the lower edge while the next route dominates the useful center of the camera.
- Platform silhouette and landing-center treatment remain distinct against the sky.
- The capture coincides with the immediately following warning overlay at the same high-altitude state; the warning banner does not cover the next landing platform and therefore exercises a stricter HUD/camera composition than the clean high state alone.
- Currency/Shop and Height/PB/streak remain readable without obstructing the climb path.

## Cross-viewport conclusion

- Compact phone low/mid/high: previously accepted in `docs/evidence/2026-10-01-phone-visual-qa.md`.
- Desktop low/mid/high: previously accepted in `docs/evidence/2026-10-01-desktop-visual-qa.md`.
- Tablet low/high: accepted by this run.

P14-T05 camera/readability at low/high altitude on each reviewed viewport is **accepted**.

This does **not** close P14-T02 genuine tablet touch interaction; that remains separately open until authoritative touch input is available.
