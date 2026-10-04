# Rising Steps low-graphics runtime QA — 2026-10-04

## Scope

This pass verifies the shipped low-presentation policy under a real Studio PlaySolo/VisualOnly run. It specifically checks that lowering graphics removes decorative effects without weakening gameplay-state contrast, avatar readability, HUD readability or the warning state.

## QA build

A local QA-only build `/tmp/risingsteps-lowgraphics-qa.rbxlx` was produced from the current `main` source with two temporary Studio-only overrides:

- `RuntimeQaConfig.EnabledInStudio = true` and `VisualOnly = true` so the existing visual acceptance harness drives exact phases.
- `GraphicsQualityController` receives an in-build Studio-only quality value of `1`, selecting the production low-graphics profile from `GraphicsQualityRules`.

The source files were restored immediately after the `.rbxlx` was built. `git status` was clean before runtime acceptance; no production default was changed.

## Runtime result

The VisualOnly harness completed the complete sequence:

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

The runtime course budget remained within the production lane budget and the standard gameplay path remained active.

## Visual acceptance

### High altitude

Evidence: `docs/evidence/2026-10-04-low-graphics/high.png`

- Avatar remains clearly separated from the step surface.
- Current and upcoming platforms retain readable silhouettes against the sky.
- Cyan gameplay accents remain visible without Bloom/decorative light dependence.
- Height, PB, streak, currency and Shop controls remain readable.

### Warning state

Evidence: `docs/evidence/2026-10-04-low-graphics/warning.png`

- Warning banner remains immediately readable.
- Retracting platform changes to a strong orange outline and cannot be confused with the cyan active route.
- The state remains distinguishable by geometry/outline treatment in addition to color.
- Disabling decorative Beams/PointLights/Bloom does not hide the next route or the avatar.

## Acceptance

- Low profile (`SavedQualityLevel` 1–3 policy): runtime accepted.
- Gameplay geometry/collision unchanged: accepted.
- High-altitude silhouette/readability: accepted.
- Warning-state contrast/readability: accepted.
- HUD readability: accepted.
- P14-T09 Low/mobile graphics-quality presentation: **accepted**.

This evidence does not close genuine tablet touch interaction; P14-T02 remains independently open.
