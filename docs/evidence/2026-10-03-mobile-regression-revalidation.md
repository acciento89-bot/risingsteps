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

## Live refresh

- Revalidated canonical main immediately before release: 16 pure-Luau test files, 16,000 reachability placements, Selene 0/0, Rojo build and release-readiness all passed.
- iPhone XR smoke confirmed the LAND ANYWHERE rule/copy and clear opening route in the existing production place.
- Existing production place 133160458509988 was republished after this corrective pass.
- Studio returned PublishSuccessful and Published new changes in Steigende Schritte to Roblox.

## Follow-up opening-density correction and v17 production verification

A later real-phone screenshot review found that the course could remain mathematically reachable while still reading as a dense black tower from the chase camera. This was treated as a separate presentation regression rather than undoing the earlier landing-forgiveness work.

Corrections in canonical source commit `e3c8111`:
- base platform footprint reduced to 6.25 studs so successive targets no longer visually merge;
- horizontal and vertical spacing increased inside the verified jump envelope;
- opening steps now form an authored forward curve instead of stacking on one camera axis;
- generated direction changes are constrained to a readable forward-flowing path instead of repeatedly crossing recent steps;
- compact-client look-ahead reduced to four upcoming steps to avoid a dense overhead block;
- reachability hard cap and regression tests updated to match the new verified geometry.

Verification:
- StyLua/Selene: pass, 0 errors / 0 warnings / 0 parse errors;
- 16 pure-Luau test files passed;
- 16,000/16,000 procedural placements passed reachability checks;
- release-readiness passed;
- canonical Rojo build SHA-256: `1cf00ac9c82c9fa8c87434e760cfc19bb3151c538eeeab4bd7d1775b6ced89b5`;
- existing production Place `133160458509988` / Universe `10768815106` was opened directly and synced from the canonical `RisingSteps` Rojo project; no new Place/Experience was created;
- Studio published successfully as production Place version `v17` and logged `Published new changes in "Steigende Schritte" to Roblox.`;
- post-publish iPhone XR PlaySolo smoke showed the widened gaps, lateral opening curve and limited look-ahead in the published Place with `0 CreatorErrors`.
