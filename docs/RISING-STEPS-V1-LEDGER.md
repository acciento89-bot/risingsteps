# Rising Steps V1 Quality Ledger

**Portfolio order:** 2 / 4 — start only after Perfect Drop reaches its release-quality gate.

Status: `[ ]` open · `[~]` implemented but not fully runtime-verified · `[x]` verified complete · `[!]` externally/runtime blocked

## Release quality contract

Rising Steps must feel like a polished vertical survival platformer, not generated blocks on a baseplate.

Mandatory V1 rules:
- Avatar always remains visible and is the movement anchor.
- New player understands “climb before lower steps disappear” in under 10 seconds.
- Camera climbs smoothly and never loses the avatar or next reachable steps.
- Step generation is always reachable within the intended movement envelope.
- Expiring steps communicate danger before disappearing.
- Difficulty rises continuously without sudden impossible spikes.
- Death/retry is fast and deterministic.
- Touch, keyboard/mouse and controller all support the same skill ceiling.
- Persistent progression, rewards and purchases are server-authoritative.
- Production art, sound and VFX must survive screenshot-quality review.
- A full fresh-player and rejoin journey must pass in the published private place.
- QA captures go to `/tmp/risingsteps-qa`; only curated evidence enters `docs/evidence/`.

## P00 Product lock
- [ ] P00-T01 Lock score vocabulary: height, streak, clean/perfect landing and PB
- [ ] P00-T02 Lock movement/jump model and whether default Roblox jump is retained or tuned
- [ ] P00-T03 Lock step lifetime, warning window, vertical spacing and horizontal reach envelope
- [ ] P00-T04 Lock difficulty ramp, speed-up bands and fail/revive rules
- [ ] P00-T05 Lock ethical monetization and cosmetic categories
- [ ] P00-T06 Measurable quality/release criteria

## P01 Technical foundation
- [ ] P01-T01 Rojo client/server/shared layout
- [ ] P01-T02 Shared movement/generation/scoring/economy config
- [ ] P01-T03 Remote schemas and authority boundaries
- [ ] P01-T04 Lint/format/tests/build tooling
- [ ] P01-T05 CI and release-readiness gates
- [ ] P01-T06 Dev/prod place documentation and canonical build policy

## P02 Character, controls and camera
- [ ] P02-T01 Reliable spawn on safe starting platform
- [ ] P02-T02 Responsive movement/jump tuning with no sticky edges
- [ ] P02-T03 Third-person vertical camera follows climb without nausea or clipping
- [ ] P02-T04 Camera exposes enough upcoming geometry for fair decisions
- [ ] P02-T05 Touch movement/jump targets meet mobile ergonomics
- [ ] P02-T06 Controller and keyboard parity
- [ ] P02-T07 Runtime spawn → climb → fall → retry → respawn verification

## P03 Rising-step mechanic
- [ ] P03-T01 Deterministic server-owned step sequence
- [ ] P03-T02 New steps spawn ahead/above with readable timing
- [ ] P03-T03 Old steps enter warning state before disappearing
- [ ] P03-T04 Disappearing step collision/state transitions are race-safe
- [ ] P03-T05 Landing detection cannot double-count or award while falling past
- [ ] P03-T06 Fall/death boundary is unambiguous and triggers once
- [ ] P03-T07 Runtime acceptance across slow, medium and high pacing

## P04 Scoring, streak and feedback
- [ ] P04-T01 Height/step score model
- [ ] P04-T02 Clean/Perfect landing definition based on intended geometry
- [ ] P04-T03 Streak/combo rules and break conditions
- [ ] P04-T04 PB persistence and atomic updates
- [ ] P04-T05 Immediate feedback hierarchy: land → grade → streak → reward
- [ ] P04-T06 Server rejects spoofed step IDs/height/landing claims

## P05 Procedural step generation
- [ ] P05-T01 Reachability model based on actual character movement
- [ ] P05-T02 Horizontal/vertical placement envelopes by difficulty
- [ ] P05-T03 Pattern library avoids repetitive left-right monotony
- [ ] P05-T04 No overlapping, buried, off-camera or impossible steps
- [ ] P05-T05 Seeded QA generation
- [ ] P05-T06 1,000+ generated-step simulation with zero unreachable placements
- [ ] P05-T07 Runtime long-climb test with cleanup and bounded part count

## P06 Progression and persistence
- [ ] P06-T01 Coins/rewards tied to legitimate height and streak
- [ ] P06-T02 Trail, landing effect, step theme and environment cosmetic catalog
- [ ] P06-T03 Purchase/equip validation on server
- [ ] P06-T04 Versioned profile schema and migration
- [ ] P06-T05 Autosave/leave/recovery semantics
- [ ] P06-T06 New-session rejoin preserves PB, coins, cosmetics and settings

## P07 Tutorial and retention
- [ ] P07-T01 First-session tutorial explains climb + disappearing steps visually
- [ ] P07-T02 First dangerous expiration is telegraphed, not a surprise death
- [ ] P07-T03 Daily login reward
- [ ] P07-T04 Daily height/streak challenge
- [ ] P07-T05 Achievement milestones
- [ ] P07-T06 PB celebration and immediate “again” loop

## P08 Monetization
- [ ] P08-T01 Final products/passes and prices
- [ ] P08-T02 Revive places player on a valid recent step
- [ ] P08-T03 Step-stability/slowdown boost has explicit duration and fairness limits
- [ ] P08-T04 Coin multiplier never changes leaderboard height
- [ ] P08-T05 Receipt allowlist/idempotency/serialization
- [ ] P08-T06 Explicit-prompt shop and ownership UI
- [ ] P08-T07 Duplicate/retry/aborted purchase tests
- [!] P08-T08 Successful real Developer Product receipt + rejoin test

## P09 Production UI/UX
- [ ] P09-T01 HUD prioritizes height, streak, warning state and PB
- [ ] P09-T02 Danger/expiring-step feedback is readable without color alone
- [ ] P09-T03 Result/retry path is immediate
- [ ] P09-T04 Cosmetic/shop previews are production-ready
- [ ] P09-T05 Compact phone/tablet/desktop responsive pass
- [ ] P09-T06 Controller focus/navigation
- [ ] P09-T07 Reduced-motion/accessibility settings

## P10 Production art
- [ ] P10-T01 Distinct vertical-world art direction with depth cues
- [ ] P10-T02 Step kit has intentional geometry/materials, not raw primitives
- [ ] P10-T03 Safe/active/warning/expiring states have coherent visual language
- [ ] P10-T04 Background/parallax/fog reinforces altitude without obscuring gameplay
- [ ] P10-T05 Lighting/material pass across low and high altitude
- [ ] P10-T06 Screenshot-quality gate at representative heights

## P11 Audio and VFX
- [ ] P11-T01 Landing feedback
- [ ] P11-T02 Clean/Perfect streak escalation
- [ ] P11-T03 Expiry warning and disappear cues
- [ ] P11-T04 Fall/failure/retry cues
- [ ] P11-T05 Height/PB/reward feedback
- [ ] P11-T06 Owned/Roblox-safe assets and reduced-motion runtime QA

## P12 Security and persistence hardening
- [ ] P12-T01 Remote/rate-limit audit
- [ ] P12-T02 Server validates landed step and legal sequence
- [ ] P12-T03 Position/teleport/NaN/extreme-value guards
- [ ] P12-T04 Currency/ownership mutation serialization
- [ ] P12-T05 DataStore migration/recovery/lock tests
- [ ] P12-T06 Structured diagnostics

## P13 Mandatory full runtime journey
- [ ] P13-T01 Fresh spawn + tutorial
- [ ] P13-T02 Climb through multiple difficulty bands
- [ ] P13-T03 Observe warning and real step disappearance
- [ ] P13-T04 Build and break a streak
- [ ] P13-T05 Fall → result → retry
- [ ] P13-T06 Reward/cosmetic purchase/equip path
- [ ] P13-T07 Revive path
- [ ] P13-T08 Respawn camera/control recovery
- [ ] P13-T09 New-session persistence/rejoin
- [ ] P13-T10 20-minute stability/performance climb

## P14 Device and performance QA
- [ ] P14-T01 Compact phone touch
- [ ] P14-T02 Tablet touch
- [ ] P14-T03 Desktop keyboard/mouse
- [ ] P14-T04 Controller
- [ ] P14-T05 Camera/readability at low/high altitude on each viewport
- [ ] P14-T06 Generator/cleanup performance and bounded memory/part count

## P15 Release
- [ ] P15-T01 Production icon/thumbnails/metadata
- [ ] P15-T02 Privacy/content questionnaire
- [ ] P15-T03 Publish canonical build privately
- [ ] P15-T04 Repeat complete P13 journey in published private place
- [ ] P15-T05 Record build hash/place version/rollback
- [!] P15-T06 Controlled public release after paid receipt/rejoin evidence and zero P0/P1 defects

## P16 Post-launch
- [!] P16-T01 First telemetry review
- [!] P16-T02 Evidence-based difficulty/balance patch
- [ ] P16-T03 New step patterns/themes/cosmetics cadence

## Definition of Done

Rising Steps V1 is complete only when a player can repeatedly climb, understand danger, fail, retry, progress and rejoin without broken camera/control/generation state, while the published game meets production visual/audio quality on all supported device classes.
