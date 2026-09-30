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
- **No 08/15/default Roblox look:** raw Parts, default materials, default-looking UI, empty-baseplate composition, visible debug text, placeholder icons and unstyled generated geometry are release blockers.
- The world must have authored visual depth: foreground gameplay kit, mid-ground structures and distant/parallax depth, with at least three coherent altitude bands that visibly evolve during a climb.
- Gameplay platforms may use simple collision primitives internally, but the visible step kit must use authored silhouettes, trims, supports, materials and state-specific surface treatment.
- The avatar and next 2–3 reachable steps must remain visually readable on a compact phone without zooming or guessing.
- Every production screenshot must look like a shippable game without requiring an explanation of what is unfinished.
- A full fresh-player and rejoin journey must pass in the published private place.
- QA captures go to `/tmp/risingsteps-qa`; only curated evidence enters `docs/evidence/`.

## P00 Product lock
- [x] P00-T01 Lock score vocabulary: height, streak, clean/perfect landing and PB
- [x] P00-T02 Lock movement/jump model and whether default Roblox jump is retained or tuned
- [x] P00-T03 Lock step lifetime, warning window, vertical spacing and horizontal reach envelope
- [x] P00-T04 Lock difficulty ramp, speed-up bands and fail/revive rules
- [x] P00-T05 Lock ethical monetization and cosmetic categories
- [x] P00-T06 Measurable quality/release criteria
- [x] P00-T07 Lock visual identity board: palette, material family, typography, UI radius/spacing, icon language, world silhouettes and explicit “do not use” examples
- [x] P00-T08 Lock camera composition targets for compact phone/tablet/desktop: avatar scale, visible upcoming steps and horizon/depth balance
- [x] P00-T09 Lock altitude-art progression with at least three visually distinct but coherent world bands so a long climb never looks like repeated blocks

## P01 Technical foundation
- [x] P01-T01 Rojo client/server/shared layout
- [x] P01-T02 Shared movement/generation/scoring/economy config
- [x] P01-T03 Remote schemas and authority boundaries
- [x] P01-T04 Lint/format/tests/build tooling
- [x] P01-T05 CI and release-readiness gates
- [x] P01-T06 Dev/prod place documentation and canonical build policy

P01 verification: local StyLua, Selene, pure-Luau tests, release-readiness and Rojo build passed; GitHub Actions CI run `36683819750` completed successfully.

## P02 Character, controls and camera
- [~] P02-T01 Reliable spawn on safe starting platform
- [~] P02-T02 Responsive movement/jump tuning with no sticky edges
- [~] P02-T03 Third-person vertical camera follows climb without nausea or clipping
- [~] P02-T04 Camera exposes enough upcoming geometry for fair decisions
- [~] P02-T05 Touch movement/jump targets meet mobile ergonomics
- [~] P02-T06 Controller and keyboard parity
- [!] P02-T07 Runtime spawn → climb → fall → retry → respawn verification

P02 implementation note: authored start deck/spawn, authoritative movement tuning, default touch/controller movement policy and elevated third-person camera are implemented and pass local format/lint/tests/build. GitHub Actions CI run `36684305304` is green. T01-T06 remain `[~]` until Studio/device runtime verifies spawn safety, camera framing, sticky-edge behavior and input parity.

## P03 Rising-step mechanic
- [~] P03-T01 Deterministic server-owned step sequence
- [~] P03-T02 New steps spawn ahead/above with readable timing
- [~] P03-T03 Old steps enter warning state before disappearing
- [~] P03-T04 Disappearing step collision/state transitions are race-safe
- [~] P03-T05 Landing detection cannot double-count or award while falling past
- [~] P03-T06 Fall/death boundary is unambiguous and triggers once
- [!] P03-T07 Runtime acceptance across slow, medium and high pacing

P03 implementation note: deterministic server-owned sequence, authored six-variant step models, per-player lanes, warning→expiring→expired state machine, token-guarded expiry, ordered landing validation and single-fire fall failure are implemented. Local format/lint/4 pure-Luau tests/release-readiness/Rojo build pass; GitHub Actions CI run `36685118098` is green. P02-T07 and P03-T07 remain runtime-blocked until an interactive Studio/device play session can be driven.

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
- [ ] P09-T08 Replace all placeholder/default-looking interface surfaces with a coherent production component kit: typography, spacing grid, panels, buttons, icons and interaction states
- [ ] P09-T09 HUD must preserve gameplay visibility on compact phones; no control or metric may cover the avatar, current landing step or next reachable step
- [ ] P09-T10 Runtime visual-state pass for idle, active climb, warning, Perfect/Good, failure, result, shop and first-session tutorial
- [ ] P09-T11 Screenshot gate: HUD and shop must look intentionally designed at native phone and desktop viewport sizes

## P10 Production art — non-negotiable ship-quality gate
- [ ] P10-T01 Build a distinct vertical-world identity with authored foreground, mid-ground and distant depth; no empty baseplate or generic floating-block presentation
- [ ] P10-T02 Create a production step kit with at least six authored visible silhouette/trim variants; collision truth may stay simple but rendered geometry must not look like untouched Roblox Parts
- [ ] P10-T03 Add believable structural language beneath/around steps — supports, brackets, rails, anchors, cables, architectural fragments or equivalent world-specific detail
- [ ] P10-T04 Safe/active/warning/expiring states use shape/material/motion/VFX as well as color, and remain readable for color-vision deficiencies
- [ ] P10-T05 Build at least three coherent altitude bands with visible progression in environment, atmosphere and set dressing
- [ ] P10-T06 Background/parallax/fog/cloud/depth treatment reinforces height and motion without hiding the next 2–3 reachable steps
- [ ] P10-T07 Lighting/material pass has deliberate key/fill/accent balance, soft readable shadows and no crushed-black or blown-out gameplay surfaces
- [ ] P10-T08 Avatar silhouette remains separated from every supported environment/theme at normal and compact-phone camera distances
- [ ] P10-T09 Add authored landing-zone detail, edge treatment and surface breakup so platforms read as finished game assets instead of colored boxes
- [ ] P10-T10 Cosmetic themes change presentation substantially while preserving identical collision, reachability and competitive truth
- [ ] P10-T11 Visual polish pass removes z-fighting, seams, floating props, texture stretching, visible generation pop, clipping and debug/helper geometry
- [ ] P10-T12 Screenshot-quality gate at early, mid and high altitude on compact phone and desktop; any frame that still reads as prototype blocks P15

## P11 Audio and VFX
- [ ] P11-T01 Landing feedback
- [ ] P11-T02 Clean/Perfect streak escalation
- [ ] P11-T03 Expiry warning and disappear cues
- [ ] P11-T04 Fall/failure/retry cues
- [ ] P11-T05 Height/PB/reward feedback
- [ ] P11-T06 Owned/Roblox-safe assets and reduced-motion runtime QA
- [ ] P11-T07 Character movement/landing presentation has coherent animation timing and no abrupt camera/VFX conflict
- [ ] P11-T08 VFX budget keeps the avatar, current step and next target readable during high streaks
- [ ] P11-T09 Audio mix pass prevents stacked landing/warning/reward cues from clipping or becoming fatiguing on mobile speakers

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
- [ ] P13-T11 Visual acceptance run records curated screenshots at spawn/tutorial, early climb, mid climb, high altitude, warning/expiry, result and shop
- [ ] P13-T12 Human visual review confirms there are no placeholder assets, default-looking screens, empty-baseplate views, geometry pop-in or repetitive copy-paste presentation
- [ ] P13-T13 Camera composition review confirms the avatar and next reachable geometry remain legible throughout a long climb

## P14 Device and performance QA
- [ ] P14-T01 Compact phone touch
- [ ] P14-T02 Tablet touch
- [ ] P14-T03 Desktop keyboard/mouse
- [ ] P14-T04 Controller
- [ ] P14-T05 Camera/readability at low/high altitude on each viewport
- [ ] P14-T06 Generator/cleanup performance and bounded memory/part count
- [ ] P14-T07 Compact-phone screenshot review at low/mid/high altitude; art density and HUD must remain readable without shrinking critical controls
- [ ] P14-T08 Tablet/desktop composition uses added screen space intentionally rather than merely stretching the phone layout
- [ ] P14-T09 Low/mobile graphics-quality pass preserves gameplay-state contrast, silhouettes and warning readability

## P15 Release
- [ ] P15-T01 Production icon/thumbnails/metadata
- [ ] P15-T02 Privacy/content questionnaire
- [ ] P15-T03 Publish canonical build privately
- [ ] P15-T04 Repeat complete P13 journey in published private place
- [ ] P15-T05 Record build hash/place version/rollback
- [ ] P15-T05A Final store screenshots must be captured from the actual accepted build and accurately represent the shipped art/UI
- [ ] P15-T05B Final “looks finished” sign-off: no placeholder icon/text/material/model, no default UI styling, no debug overlays, no known visual P0/P1/P2 defect
- [!] P15-T06 Controlled public release after paid receipt/rejoin evidence and zero P0/P1 defects

## P16 Post-launch
- [!] P16-T01 First telemetry review
- [!] P16-T02 Evidence-based difficulty/balance patch
- [ ] P16-T03 New step patterns/themes/cosmetics cadence

## Definition of Done

Rising Steps V1 is complete only when a player can repeatedly climb, understand danger, fail, retry, progress and rejoin without broken camera/control/generation state **and** the accepted private-place build visually reads as a finished commercial Roblox game. A functioning mechanic, green CI, or generated platforms alone are never sufficient. Early/mid/high-altitude gameplay, HUD, result flow and shop must all pass the authored-art, mobile-readability, audio/VFX and screenshot-quality gates above before the project may advance to Perfect Jump.
