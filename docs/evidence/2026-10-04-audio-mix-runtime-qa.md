# Rising Steps audio mix runtime QA — 2026-10-04

## Scope

Hardening and runtime verification for transient landing, warning, reward/celebration and failure cues.

## Mixer changes

- Maximum transient concurrency reduced from 4 to 3.
- Cross-cue priorities are explicit: Failure > Warning > Celebration > Landing.
- A recent higher-priority cue suppresses lower-priority cues inside a short guard window.
- Incoming concurrent cues are progressively volume-ducked rather than summed at full volume.
- Higher-priority cues duck already-active lower-priority cues.
- Per-cue minimum intervals and the 0.28 single-transient volume cap remain enforced.
- Sound cleanup decrements concurrency on both `Ended` and `Destroying`, guarded against double cleanup.

## Static verification

- StyLua pass.
- Selene: 0 errors / 0 warnings / 0 parse errors.
- 16/16 pure-Luau test files pass, including new cross-cue priority/ducking regression checks.
- 16,000/16,000 reachability placements pass.
- Release-readiness passes.
- Rojo build passes.

## Studio runtime verification

A temporary Studio-only QA build enabled the existing full runtime harness without changing production QA defaults. The run exercised 42 sequential Perfect landings, streak/reward paths, warning -> expiry, Miss reset, failure, Revive, Retry, daily reward, persistence and final visual states.

- Harness result: `FUNCTIONAL_COMPLETE` then `COMPLETE`.
- CreatorErrors: 0.
- BaseParts: 846 / 900 budget.
- Beams/Lights: 21 / 96 budget.
- Warning, failure, reward and repeated landing paths all executed under the hardened mixer without runtime script errors.

## Remaining acceptance

P11-T09 remains `[~]` only for a subjective physical mobile-speaker listening pass. The deterministic anti-stacking, priority, ducking, concurrency and runtime-stability work is complete and evidenced here.
