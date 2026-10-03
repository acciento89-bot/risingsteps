# Mobile regression revalidation — 2026-10-03

## Scope

Corrective pass based on real phone play: landing intent was unclear and the opening/difficulty curve was too punishing.

## Implemented corrections

- Platform footprint increased and horizontal/vertical gaps reduced across all three bands.
- Warning windows increased and one additional completed step remains behind the player before expiry.
- The first staircase is authored to clear the spawn capsule while keeping the following jumps short.
- Perfect/Clean tolerances were widened.
- Tutorial copy explicitly states **LAND ANYWHERE** and explains that centre precision is a bonus, not a requirement.
- Repeated world-space landing billboards were removed after phone visual QA showed they stacked across several visible steps; the glowing centre marker remains as a bonus cue without blocking the route.
- Non-centre accepted landings use player-facing `SAFE LANDING` feedback.
- Platform materials/palette were brightened and all four platform edges now carry the band accent, improving silhouette and landing readability against the sky.
- A runtime-only colour omission found by this QA pass was fixed before acceptance.

## Verification

- StyLua/Selene/release-readiness: pass.
- Pure Luau: 16 test files passed.
- Procedural simulation: 16,000 placements passed reachability checks.
- Canonical cloud-place Studio QA on iPhone XR emulation completed with `[RisingStepsQA] COMPLETE`.
- Runtime climbed all 42 generated steps across Service Decks, Cloudline Works and Stratosphere Spine.
- Warning→Expired, streak build/reset, fall boundary, Revive, Retry/respawn, cosmetics, reduced motion, receipt idempotency, persistence/flush and invalid-position rejection all passed.
- Final lane presentation remained within budget after the four-edge trim pass: 839/900 BaseParts and 21/96 beams/lights.
- Final iPhone XR visual holds for tutorial, early climb and high climb show a clear route with no overlapping world-space instructional text.

No external release gate is reclassified by this corrective pass.
