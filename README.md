# Rising Steps

A compact third-person vertical survival platformer. New steps appear above the visible avatar while old steps disappear and pacing continuously increases.

## Product rule

This is intentionally a **small-scope, high-quality Roblox game**. Small scope does not permit placeholder presentation, debug-looking UI, inaccessible geometry, broken mobile layouts or unverified monetization.

## Core loop

Read the next step, move/jump upward before lower steps expire, chain clean landings, climb for height and score.

## Non-negotiables

- The Roblox avatar remains visible during core gameplay.
- Retry from failure must be fast and obvious.
- First-time understanding target: under 10 seconds.
- Short-session loop with score, best score and readable progression.
- Server-authoritative rewards, purchases and persistent progression.
- Mobile, tablet, desktop and controller support.
- No surprise purchase prompt on spawn.
- Monetization accelerates/revives/cosmetics; it must not directly buy leaderboard placement.
- Production-quality UI, lighting, sound/VFX and environment treatment before public release.
- No QA screenshots or temporary artifacts on the user's Desktop. Use `/tmp/risingsteps-qa`; only intentionally retained evidence belongs under `docs/evidence/`.

## Monetization direction

Revive, temporary step-stability/slowdown boost, coin multiplier, trails, jump/landing effects and environment themes.

## Canonical execution order

1. `README.md`
2. `docs/MASTER-PLAN.md`
3. `docs/ART-DIRECTION.md`
4. `docs/RISING-STEPS-V1-LEDGER.md`
5. Detail plan for the next open phase

The ledger is the source of truth for implementation state.
