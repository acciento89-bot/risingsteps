# Rising Steps — final store-art acceptance

Date: 2026-10-04

## Accepted files

- `icon-512.png` — previously reviewed 512x512 production icon candidate; avatar + authored rising-step mechanic visible, no text/placeholder art.
- `thumbnail-1-core.png` — core climb, avatar visible, next reachable steps readable.
- `thumbnail-2-height.png` — altitude progression, avatar visible, climb continues upward through authored structures.
- `thumbnail-3-cosmetics.png` — real gameplay with equipped cosmetic/theme presentation; no fabricated gameplay geometry.

All three files are 1920x1080 PNGs.

## Capture provenance

The gameplay/art source is the accepted Rising Steps `main` line after production publish of commit `a6970b6`. The capture source head `caaa6ba` adds only Studio + VisualOnly screenshot hygiene (`PerformanceStatsVisible=false`) and does not alter production gameplay, world geometry, collision, camera rules, HUD behavior, cosmetics, monetization or player-facing presentation.

The store-thumbnail harness is strictly Studio-only and reproduces shipped gameplay states via existing QA functions:

- Core: Steps 1–8 with Warning on Step 6.
- Height: Steps 1–30.
- Cosmetics: Ion Trail + Pulse Landing + Frost Step Theme + Dusk Environment, Steps 1–20.

The final crops contain only gameplay pixels. Studio chrome, Roblox CoreGui controls, debug/performance overlays and desktop chrome are outside the accepted 16:9 crop. No image-generation or fabricated gameplay content was used.

## Visual acceptance

Reviewed at native 1920x1080 output:

- avatar remains visible;
- reachable platform progression is readable;
- no placeholder copy/icons/materials/models are visible;
- no Studio/toolbox/explorer/debug overlay is visible;
- no known visual P0/P1/P2 defect is visible in the accepted store frames;
- artwork truthfully represents the shipped mechanic and authored environment.

## SHA-256

```text
ede8b1fe19dbfb96f3dea158b1f3c3d022feefe043d98df05aa699a7578671bc  docs/evidence/2026-10-04-final-store-art/icon-512.png
92652ed386afed56ad848d4fccecaca7dbc3c8e5151ab977964ade8dea3186be  docs/evidence/2026-10-04-final-store-art/thumbnail-1-core.png
11263a9fffb38ca7e9c7d44acf4d3bfd56467f3d2230c15dd0d262fa797e3c61  docs/evidence/2026-10-04-final-store-art/thumbnail-2-height.png
e618cafa832d3e746536598b5554f29420125fa79669036d6a8ae51973f9791d  docs/evidence/2026-10-04-final-store-art/thumbnail-3-cosmetics.png
```
