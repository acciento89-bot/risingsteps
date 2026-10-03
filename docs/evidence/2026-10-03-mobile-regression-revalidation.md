# Mobile regression revalidation — 2026-10-03

## Scope

Corrective pass based on real phone play: landing intent was unclear and the opening/difficulty curve was too punishing.

## Implemented corrections

- Platform footprint increased and horizontal/vertical gaps reduced across all three bands.
- Warning windows increased and one additional completed step remains behind the player before expiry.
- The first staircase is authored to clear the spawn capsule while keeping the following jumps short.
- Perfect/Clean tolerances were widened.
- The first four targets explicitly state: **LAND ANYWHERE — CENTER = BONUS**.
- Tutorial copy now makes clear that any valid landing on the next platform advances the run; centre precision is a bonus.
- Non-centre accepted landings use player-facing `SAFE LANDING` feedback.
- A runtime-only colour omission found by this QA pass was fixed before acceptance.

## Verification

- StyLua/Selene/release-readiness: pass.
- Pure Luau: 16 test files passed.
- Procedural simulation: 16,000 placements passed reachability checks.
- Canonical cloud-place Studio QA on iPhone XR emulation completed with `[RisingStepsQA] COMPLETE`.
- Runtime climbed all 42 generated steps across Service Decks, Cloudline Works and Stratosphere Spine.
- Warning→Expired, streak build/reset, fall boundary, Revive, Retry/respawn, cosmetics, reduced motion, receipt idempotency, persistence/flush and invalid-position rejection all passed.
- Lane presentation remained within budget: 755/900 BaseParts and 21/96 beams/lights.

No external release gate is reclassified by this corrective pass.
