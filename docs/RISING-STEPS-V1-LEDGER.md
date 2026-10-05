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
- [x] P02-T01 Reliable spawn on safe starting platform
- [x] P02-T02 Responsive movement/jump tuning with no sticky edges
- [x] P02-T03 Third-person vertical camera follows climb without nausea or clipping
- [x] P02-T04 Camera exposes enough upcoming geometry for fair decisions
- [!] P02-T05 Touch movement/jump targets meet mobile ergonomics
- [x] P02-T06 Controller and keyboard parity — keyboard movement/jump and Studio Controller Emulator Gamepad1 movement/jump both verified on the production control path
- [x] P02-T07 Runtime spawn → climb → fall → retry → respawn verification

P02 implementation note: authored start deck/spawn, authoritative movement tuning, default touch/controller movement policy and elevated third-person camera are implemented. The spawn is deliberately staged at the rear of the deck, faces the climb direction and uses a visible launch-runway treatment; the first three generated steps are locked into a forward rising onboarding staircase before procedural turns begin. A real Studio PlayServer/PlayClient session on `risingsteps-final2.rbxlx` now starts successfully with the avatar visible on the authored launch bay and no script-start errors. P02-T07 is therefore `[~]`; fall/retry/respawn and device-input acceptance still remain.

## P03 Rising-step mechanic
- [x] P03-T01 Deterministic server-owned step sequence
- [x] P03-T02 New steps spawn ahead/above with readable timing
- [x] P03-T03 Old steps enter warning state before disappearing
- [x] P03-T04 Disappearing step collision/state transitions are race-safe
- [x] P03-T05 Landing detection cannot double-count or award while falling past
- [x] P03-T06 Fall/death boundary is unambiguous and triggers once
- [x] P03-T07 Runtime acceptance across slow, medium and high pacing

P03 implementation note: deterministic server-owned sequence, authored six-variant step models, per-player lanes, warning→expiring→expired state machine, token-guarded expiry, ordered landing validation and single-fire fall failure are implemented. Local format/lint/4 pure-Luau tests/release-readiness/Rojo build pass; GitHub Actions CI run `36685118098` is green. P02-T07 and P03-T07 remain runtime-blocked until an interactive Studio/device play session can be driven.

## P04 Scoring, streak and feedback
- [x] P04-T01 Height/step score model
- [x] P04-T02 Clean/Perfect landing definition based on intended geometry
- [x] P04-T03 Streak/combo rules and break conditions
- [x] P04-T04 PB persistence and atomic updates
- [x] P04-T05 Immediate feedback hierarchy: land → grade → streak → reward
- [x] P04-T06 Server rejects spoofed step IDs/height/landing claims

P04 implementation note: Height, Perfect/Clean/Miss grading, streak reset/increment rules, server-only landing claims, persistent PB via atomic DataStore `UpdateAsync`, and a custom responsive gameplay HUD/grade feedback path are implemented. Local format/lint/5 pure-Luau tests/release-readiness/Rojo build pass; GitHub Actions CI run `36685602471` is green. Runtime/DataStore verification remains required before these items can move from `[~]` to `[x]`.

## P05 Procedural step generation
- [x] P05-T01 Reachability model based on actual character movement
- [x] P05-T02 Horizontal/vertical placement envelopes by difficulty
- [x] P05-T03 Pattern library avoids repetitive left-right monotony
- [x] P05-T04 No overlapping, buried, off-camera or impossible steps
- [x] P05-T05 Seeded QA generation
- [x] P05-T06 1,000+ generated-step simulation with zero unreachable placements
- [x] P05-T07 Runtime long-climb test with cleanup and bounded part count

P05 implementation note: physics-based reachability model, per-band placement envelopes, seeded deterministic generation and an 8-direction anti-repeat placement pattern are implemented. Static QA now simulates 16,000 generated placements across eight seeds with zero unreachable placements; local format/lint/7 pure-Luau tests/release-readiness/Rojo build pass and GitHub Actions CI run `36685931096` is green. T01-T04 remain `[~]` pending Studio camera/runtime validation; T07 requires the real long-climb runtime test.

2026-10-03 spacing regression fix: compact-phone review exposed a visually over-dense staircase despite mathematical reachability. Platform footprint is now 6.25 studs, visible look-ahead is capped at four steps, horizontal/vertical spacing was widened within the verified jump envelope, and the first three targets form an authored forward curve instead of stacking on one camera axis. The updated generator passes 16,000/16,000 simulated placements, all 16 pure-Luau tests, release-readiness and an iPhone XR PlaySolo review with 0 CreatorErrors.

## P06 Progression and persistence
- [x] P06-T01 Coins/rewards tied to legitimate height and streak
- [x] P06-T02 Trail, landing effect, step theme and environment cosmetic catalog
- [x] P06-T03 Purchase/equip validation on server
- [x] P06-T04 Versioned profile schema and migration
- [x] P06-T05 Autosave/leave/recovery semantics
- [x] P06-T06 New-session rejoin preserves PB, coins, cosmetics and settings

P06 implementation note: legitimate server-validated landings now award Coins; the launch cosmetic catalog covers trails, landing effects, step themes and environment themes; all buy/equip/settings mutations are server validated; profile schema v2 migrates legacy PB-only data and filters unknown cosmetic IDs; autosave, leave-save, BindToClose and failed-load clobber protection are implemented. Local format/lint/8 pure-Luau tests/16,000-placement simulation/release-readiness/Rojo build pass; GitHub Actions CI run `36702520370` is green. Runtime DataStore purchase/equip/rejoin verification is still required before T01/T03/T05/T06 become `[x]`.

## P07 Tutorial and retention
- [x] P07-T01 First-session tutorial explains climb + disappearing steps visually
- [x] P07-T02 First dangerous expiration is telegraphed, not a surprise death
- [x] P07-T03 Daily login reward
- [x] P07-T04 Daily height/streak challenge
- [x] P07-T05 Achievement milestones
- [x] P07-T06 PB celebration and immediate “again” loop

P07 implementation note: first-session tutorial overlay, disappearing-step explanation, automatic tutorial completion on the first legitimate landing, daily claims with consecutive-day rewards, deterministic rotating daily challenges, launch achievements, NEW PB/achievement/challenge feedback and a 48px retry action are implemented. Profile schema v3 migrates prior saves and persists tutorial/daily/challenge/achievement totals. Local format/lint/9 pure-Luau tests/16,000-placement simulation/release-readiness/Rojo build pass; GitHub Actions CI run `36703026061` is green. All P07 items remain `[~]` until the complete fresh-player/rejoin runtime journey is recorded.

## P08 Monetization
- [x] P08-T01 Final products/passes and prices
- [x] P08-T02 Revive places player on a valid recent step
- [x] P08-T03 Step-stability/slowdown boost has explicit duration and fairness limits
- [x] P08-T04 Coin multiplier never changes leaderboard height
- [x] P08-T05 Receipt allowlist/idempotency/serialization
- [x] P08-T06 Explicit-prompt shop and ownership UI
- [x] P08-T07 Duplicate/retry/aborted purchase tests

P08 implementation note: V1 prices are locked in config (Revive 19 R$, 5-minute Stability 29 R$, cosmetic Coin Multiplier pass 99 R$); Stability and Revive force assisted-run semantics so PB/competitive Height cannot benefit; Coin Multiplier touches cosmetic currency only; receipt lookup is allowlisted, duplicate purchase IDs are idempotent and receipt history is bounded/persisted. Revive returns to the most recent validated step and is capped to one use per run. Local format/lint/10 pure-Luau tests/16,000-placement simulation/release-readiness/Rojo build pass; GitHub Actions CI run `36703312201` is green. Developer Product IDs are live in Creator Dashboard and wired in config: Revive `3715818292` at 19 R$ and Stability `3715820324` at 29 R$. The Coin Multiplier GamePass is also live and wired as `2002022878` at 99 R$. Shop/ownership UI is implemented and runtime-accepted. T08 remains the mandatory real paid-receipt + rejoin gate.
- [!] P08-T08 Successful real Developer Product receipt + rejoin test

## P09 Production UI/UX
- [x] P09-T01 HUD prioritizes height, streak, warning state and PB
- [x] P09-T02 Danger/expiring-step feedback is readable without color alone
- [x] P09-T03 Result/retry path is immediate
- [x] P09-T04 Cosmetic/shop previews are production-ready
- [x] P09-T05 Compact phone/tablet/desktop responsive pass
- [x] P09-T06 Controller focus/navigation
- [x] P09-T07 Reduced-motion/accessibility settings
- [x] P09-T08 Replace all placeholder/default-looking interface surfaces with a coherent production component kit: typography, spacing grid, panels, buttons, icons and interaction states
- [x] P09-T09 HUD must preserve gameplay visibility on compact phones; no control or metric may cover the avatar, current landing step or next reachable step
- [x] P09-T10 Runtime visual-state pass for idle, active climb, warning, Perfect/Good, failure, result, shop and first-session tutorial
- [x] P09-T11 Screenshot gate: HUD and shop must look intentionally designed at native phone and desktop viewport sizes

P09 runtime note: compact-phone VisualOnly QA on iPhone XR 896×414 accepted tutorial, early, mid, high, warning, result, shop and cosmetic states; evidence is recorded in `docs/evidence/2026-10-01-phone-visual-qa.md`. HUD hierarchy, non-color-only warning, result/retry, production component styling and authored cosmetic previews are visually accepted on compact phone. P09-T10/T11 remain `[~]` until the matching desktop pass completes.

P09 implementation note: the gameplay HUD is now a coherent custom component system with Height/PB/Streak hierarchy, Coins/Shop controls, non-color-only retract warning, result panel, 48px Retry/Revive actions, Daily/Challenge/shop surfaces, reduced-motion behavior and controller focus handoff. Cosmetic rows no longer use empty color swatches: Trails, Landing Effects, Step Themes and Environment Themes render authored category-specific mini-previews inside the UI. Compact-width layout rules preserve the central gameplay region. T01-T09 remain `[~]` and T10-T11 remain runtime/screenshot gates.

## P10 Production art — non-negotiable ship-quality gate
- [x] P10-T01 Build a distinct vertical-world identity with authored foreground, mid-ground and distant depth; no empty baseplate or generic floating-block presentation
- [x] P10-T02 Create a production step kit with at least six authored visible silhouette/trim variants; collision truth may stay simple but rendered geometry must not look like untouched Roblox Parts
- [x] P10-T03 Add believable structural language beneath/around steps — supports, brackets, rails, anchors, cables, architectural fragments or equivalent world-specific detail
- [x] P10-T04 Safe/active/warning/expiring states use shape/material/motion/VFX as well as color, and remain readable for color-vision deficiencies
- [x] P10-T05 Build at least three coherent altitude bands with visible progression in environment, atmosphere and set dressing
- [x] P10-T06 Background/parallax/fog/cloud/depth treatment reinforces height and motion without hiding the next 2–3 reachable steps
- [x] P10-T07 Lighting/material pass has deliberate key/fill/accent balance, soft readable shadows and no crushed-black or blown-out gameplay surfaces
- [x] P10-T08 Avatar silhouette remains separated from every supported environment/theme at normal and compact-phone camera distances
- [x] P10-T09 Add authored landing-zone detail, edge treatment and surface breakup so platforms read as finished game assets instead of colored boxes
- [x] P10-T10 Cosmetic themes change presentation substantially while preserving identical collision, reachability and competitive truth
- [x] P10-T11 Visual polish pass removes z-fighting, seams, floating props, texture stretching, visible generation pop, clipping and debug/helper geometry
- [x] P10-T12 Screenshot-quality gate at early, mid and high altitude on compact phone and desktop; any frame that still reads as prototype blocks P15

P10 implementation note: production world art now includes a city-depth Service Decks band, Cloudline scaffold/cloud/glass band and Stratosphere spine/antenna band; global Atmosphere/Clouds/Bloom/ColorCorrection/soft shadows are authored; the step kit now has six structural variants, landing target detailing, braces/rails/anchors/cantilever pieces and warning fins so danger changes shape as well as color. Equipped Step Themes and Environment Themes now alter the live presentation locally without changing collision/reachability; character Trails and Landing Effects also render distinct equipped cosmetics. Theme application is re-applied after retries and environment recoloring is based on stable original colors to avoid cumulative tint drift. T10 is therefore implemented but remains `[~]` until runtime visual acceptance. P10-T11 now also includes a deterministic silhouette cleanup pass: each step gets authored asymmetrical edge hardware, while major service/cloudline world structures no longer mirror perfectly and city masses vary in spacing, height and depth. T11 is implemented but remains `[~]` until the runtime visual pass confirms there are no z-fights, seams, floating props, clipping or visible generation artifacts. T12 screenshot approval remains runtime dependent.

## P11 Audio and VFX
- [x] P11-T01 Landing feedback
- [x] P11-T02 Clean/Perfect streak escalation
- [x] P11-T03 Expiry warning and disappear cues
- [x] P11-T04 Fall/failure/retry cues
- [x] P11-T05 Height/PB/reward feedback
- [x] P11-T06 Owned/Roblox-safe assets and reduced-motion runtime QA
- [x] P11-T07 Character movement/landing presentation has coherent animation timing and no abrupt camera/VFX conflict
- [x] P11-T08 VFX budget keeps the avatar, current step and next target readable during high streaks
- [!] P11-T09 Audio mix pass prevents stacked landing/warning/reward cues from clipping or becoming fatiguing on mobile speakers

P11 implementation note: Clean/Perfect landing rings, bounded streak sparks, warning-step Highlight, failure camera cue, Revive/PB/Achievement tones and reduced-motion suppression are implemented. Transient audio uses Roblox runtime-owned assets only, volumes are deliberately low and concurrency is capped at three with cross-cue priority/ducking. Character presentation now adds client-only motion posing for ascent, fall and landing impact using the avatar's presentation Motor6D; it does not alter root position, collision, jump physics or camera target, and it fully disables to neutral transforms under reduced-motion. Respawn cleanup disconnects presentation listeners before rebinding. P11-T07 is therefore implemented but remains `[~]` until runtime confirms the pose timing works on both R6/R15 without fighting default animation or camera composition. Full mobile audio/VFX runtime QA also remains open.

P11 audio-mix hardening note: commit `9e24d66633ceb9d5127052a382be2a06d6cf3764` reduces transient concurrency to four, caps each transient at 0.28 volume and adds cue-specific minimum intervals for Landing/Warning/Failure/Celebration so repeated warnings, failure and reward cues cannot freely stack. Exact-commit verification passes StyLua, Selene (0/0), 16 pure-Luau test files, the 16,000-placement simulation, release-readiness and Rojo build. P11-T09 remains `[~]` until a real mobile-speaker listening pass confirms the mix is non-clipping and non-fatiguing. Evidence: `docs/evidence/2026-10-01-audio-mix-hardening.md`. P11 was additionally hardened on 2026-10-04 with explicit Failure > Warning > Celebration > Landing priorities, cross-cue guard suppression, progressive concurrency ducking, active-cue ducking and guarded Ended/Destroying cleanup. The full Studio runtime harness reached `FUNCTIONAL_COMPLETE` and `COMPLETE` with 0 CreatorErrors while exercising repeated landing, warning, reward, failure and revive paths. Technical anti-stacking/runtime acceptance is complete; only the subjective physical mobile-speaker listening pass keeps P11-T09 at `[~]`. Evidence: `docs/evidence/2026-10-04-audio-mix-runtime-qa.md`.

## P12 Security and persistence hardening
- [x] P12-T01 Remote/rate-limit audit
- [x] P12-T02 Server validates landed step and legal sequence
- [x] P12-T03 Position/teleport/NaN/extreme-value guards
- [x] P12-T04 Currency/ownership mutation serialization
- [x] P12-T05 DataStore migration/recovery/lock tests
- [x] P12-T06 Structured diagnostics

P12 implementation note: both mutation remotes now have per-player rate limits; landing authority remains server/touch/sequence based; NaN/infinity/extreme position and velocity guards plus rapid-landing timing protection are active; profile saves use mutation revisions so a write racing a later mutation cannot clear dirty state; failed loads cannot overwrite unknown stored data; diagnostics emit structured JSON events for persistence/security failures. Local format/lint/14 pure-Luau tests/16,000-placement simulation/release-readiness/Rojo build pass; GitHub Actions CI run `36704962166` is green. P12 remains `[~]` until hostile runtime and real DataStore/rejoin probes are executed.

## P13 Mandatory full runtime journey
- [x] P13-T01 Fresh spawn + tutorial
- [x] P13-T02 Climb through multiple difficulty bands
- [x] P13-T03 Observe warning and real step disappearance
- [x] P13-T04 Build and break a streak
- [x] P13-T05 Fall → result → retry
- [x] P13-T06 Reward/cosmetic purchase/equip path
- [x] P13-T07 Revive path
- [x] P13-T08 Respawn camera/control recovery
- [x] P13-T09 New-session persistence/rejoin
- [x] P13-T10 20-minute stability/performance climb
- [x] P13-T11 Visual acceptance run records curated screenshots at spawn/tutorial, early climb, mid climb, high altitude, warning/expiry, result and shop
- [x] P13-T12 Human visual review confirms there are no placeholder assets, default-looking screens, empty-baseplate views, geometry pop-in or repetitive copy-paste presentation
- [x] P13-T13 Camera composition review confirms the avatar and next reachable geometry remain legible throughout a long climb

P13 runtime note: fresh spawn + tutorial was visually verified in a real Studio PlayClient session on the canonical Documents place. The latest canonical runtime run on commit `d499b1bf230aa1440e0830917088086dbbb8aeb3` completed the Studio QA harness with `COMPLETE`: 42/42 sequential landings across Service Decks, Cloudline Works and Stratosphere Spine; Warning→Expired; Perfect streak + Miss reset; real fall-boundary failure; Revive; Retry/respawn; cosmetic purchase/equip; reduced-motion toggle; Daily reward; Developer Product grant/idempotency; profile payload/flush; invalid-position rejection; and stable 755 BaseParts / 21 Beams+Lights with no runtime script errors. The server-authoritative Studio QA harness then completed all 42 initial steps, verified all three generated difficulty bands, Warning→Expired, Perfect streak build + Miss reset, cosmetic purchase/equip, Daily reward, PB 42, duplicate-receipt idempotency, Revive, Retry/respawn recovery, profile payload/flush and invalid-position rejection. Evidence is recorded in `docs/evidence/2026-09-30-runtime-functional-qa.md`. The repeated apparent "crashes" during earlier QA were traced to stale temporary Rojo servers/forced Studio restarts rather than a Rising Steps script exception. P13-T05 remains `[~]` until the physical fall-boundary path itself is driven; P13-T09+ remain separate rejoin/stability/visual gates.

P13 visual evidence: compact-phone and desktop VisualOnly runs are documented in `docs/evidence/2026-10-01-phone-visual-qa.md` and `docs/evidence/2026-10-01-desktop-visual-qa.md`. Early/mid/high camera composition, warning, result, shop and cosmetic-preview presentation pass on both reviewed viewports. Tablet remains independently open.

## P14 Device and performance QA
- [!] P14-T01 Compact phone touch
- [!] P14-T02 Tablet touch
- [x] P14-T03 Desktop keyboard/mouse — keyboard move/jump runtime plus native OS-mouse Shop open/close accepted; CoreGui player-list overlap fixed; evidence `docs/evidence/2026-10-04-desktop-input-qa.md`
- [x] P14-T04 Controller — Studio Controller Emulator delivered real Gamepad1 Thumbstick1/ButtonA events; movement 6.63 studs and jump +7.27 studs verified
- [x] P14-T05 Camera/readability at low/high altitude on each viewport — phone + desktop prior evidence plus iPad 10th Gen 1180×820 Early/High VisualOnly pass; evidence `docs/evidence/2026-10-04-tablet-camera-runtime-qa.md`
- [x] P14-T06 Generator/cleanup performance and bounded memory/part count
- [x] P14-T07 Compact-phone screenshot review at low/mid/high altitude; art density and HUD must remain readable without shrinking critical controls
- [x] P14-T08 Tablet/desktop composition uses added screen space intentionally rather than merely stretching the phone layout
- [x] P14-T09 Low/mobile graphics-quality pass preserves gameplay-state contrast, silhouettes and warning readability — VisualOnly low-profile runtime completed through High/Warning with 0 CreatorErrors; evidence `docs/evidence/2026-10-04-low-graphics-runtime-qa.md`

P14 implementation note: each generated player lane records BasePart and Beam/Light counts on the course model, emits structured diagnostics, and warns if the presentation exceeds locked budgets. Release-readiness enforces ceilings of 1,000 BaseParts and 128 Beams/Lights while production config is stricter at 900/96. A real Studio runtime measurement on the canonical `~/Documents/Roblox/RisingSteps/risingsteps-final2.rbxlx` now reports 755 BaseParts total: 620 step parts, 23 start-deck parts and 112 environment parts, plus 21 Beams/Lights. The earlier 933-part reading was traced to a stale Rojo server serving `/private/tmp/risingsteps-sourcecheck` on port 34872 and overwriting the freshly built place; that server was terminated and the canonical place rebuilt from the exact current commit. Local StyLua/Selene checks pass, all 14 pure-Luau test files pass, the 16,000-placement reachability simulation passes, and release-readiness passes. A controlled 60-second PlayServer/PlayClient stability sample on the single canonical Documents place completed with 12/12 process samples alive, no script/runtime errors, no new macOS crash report and RSS decreasing from roughly 1.76 GB to 1.44 GB rather than growing. P14-T06 is verified `[x]`: the mandatory Studio soak completed for 1201.4 seconds across 164 complete 42-step climb → fall → retry cycles. Every periodic report remained at 755 BaseParts and 21 Beams/Lights; no relevant runtime ScriptContext error or macOS crash report occurred. External Studio RSS fluctuated during the run rather than growing monotonically.

P13/P14 visual evidence note: curated phone and desktop captures are committed under `docs/evidence/2026-10-01-runtime-visual/` and reviewed in `docs/evidence/2026-10-01-runtime-visual-qa.md`. The post-fix pass verifies avatar framing, next-step readability, warning treatment, result/shop layout and absence of prototype/default presentation across early/mid/high states. These screenshots, together with the 42-step runtime harness and 1201.4-second soak, close the visual/camera/art/VFX gates above. Controller focus/navigation is additionally closed by the production HUD implementation: all actionable buttons are selectable, failure routes selection to Retry, opening the Shop routes selection to Close, and closing the Shop returns selection to the Shop button when appropriate. Controller movement/jump is now independently verified through Studio Controller Emulator real Gamepad1 events; tablet touch, low-graphics mode and published-place persistence remain separate device/external gates.

P14 tablet-camera note: a detached Studio instance ran the canonical VisualOnly harness in Device Simulator `iPad 10th Generation 1180x820`. Early (Height 3) and high-altitude (Height 38) captures confirm avatar/reference framing, separated route geometry and unobstructed HUD composition; the high capture also carries the following warning overlay without covering the next landing target. The run reached `VISUAL_COMPLETE` with 0 CreatorErrors. Evidence: `docs/evidence/2026-10-04-tablet-camera-runtime-qa.md`.

P14 low-graphics note: a Studio-only QA build forced the existing production low profile (quality 1) while the VisualOnly harness drove Shop → Tutorial → Early → Mid → High → Warning → Result. The run reached `VISUAL_COMPLETE` with 0 CreatorErrors. High-altitude platform/avatar silhouettes remained clear, and the Warning state retained a strong orange outline/banner against the cyan active route with Bloom/decorative Beams/PointLights disabled. Evidence: `docs/evidence/2026-10-04-low-graphics-runtime-qa.md`.

P14 desktop-input note: Studio input QA verifies keyboard movement (5.04 studs) and jump response (up to 5.48 studs) against the canonical place. The remaining mouse blocker was resolved with a native macOS CoreGraphics click path: SHOP opens and the close control returns to gameplay. During that acceptance pass a real CoreGui defect was found and fixed: non-touch desktop/controller sessions now reserve top-right Roblox player-list space even when Studio narrows the client viewport into the nominal tablet width band. Runtime screenshots and details are in `docs/evidence/2026-10-04-desktop-input-qa.md`.

P14 adaptive-quality/tablet implementation note: commit `29a082986629ef114d1b315f6628a5be3a7b128f` adds explicit compact/tablet/desktop viewport policy plus a client-side low-graphics presentation policy. Tablet widths 761-1100 use a dedicated top-row HUD composition while the previously accepted compact-phone and desktop branches are unchanged. Graphics quality levels 1-3 disable only decorative environment Beams, decorative PointLights and Bloom; gameplay geometry, warning semantics, avatar visibility and collision/reachability are untouched. Exact-commit verification passed StyLua, Selene (0 errors/0 warnings), 16 pure-Luau test files, the 16,000-placement simulation, release-readiness and Rojo build. P14-T08 remains `[~]` pending real tablet runtime acceptance; P14-T09 advances to `[~]` because implementation/static acceptance is complete but the real low-graphics runtime visual check is still required. Evidence: `docs/evidence/2026-10-01-adaptive-graphics-tablet-policy.md`.

### Roblox publish blocker verified 2026-10-01

Creator Dashboard was checked under the signed-in owner `Acciento865`.
- `Meine Experiences`: Perfect Drop, Repair Empire, Acciento865s Ort
- `Mit mir geteilt`: empty
- Creator/ownership selector: only Acciento865; no alternate group/owner containing Rising Steps
- canonical local place remains `PlaceId=0 / UniverseId=0`

The Creator Dashboard's `Experience erstellen` control was also tested. It launches Roblox Studio rather than creating a web-only experience. The owner permits creating the Rising Steps experience from the web, but explicitly does not permit creating a new experience through Studio unattended. Therefore no experience was created and no existing experience was overwritten. P15-T03, published-place rejoin, real paid receipt, place-version/rollback and final store gates remain blocked until the owner is back at the Mac and can authorize/perform the required Studio-side creation step.

### Published private experience verification 2026-10-01

- Web-created private Experience: Universe `10768815106`
- Root Place: `133160458509988`
- Canonical GitHub source published from `main`; current QA-published source SHA: `c0e83fad4e7fdb12f13f0049267741278ffe5743`
- Studio log confirmed `PublishSuccessful` and `Published new changes in "Steigende Schritte" to Roblox.`
- Published-place functional QA completed successfully against the cloud Root Place with no Rojo server attached.
- Runtime evidence includes all 42 steps, all three difficulty/art bands, Warning→Expired, Perfect streak + Miss reset, failure boundary, Retry, Revive, cosmetic purchase/equip, daily reward, security rejection, 755/900 BasePart budget, 21/96 Beam/Light budget and successful DataStore flush.
- Real new-session rejoin loaded PB 42, 10,827 coins, Trail `ion`, Environment `dusk` and the persisted Reduced Motion sentinel. The QA cleanup then reset Reduced Motion to false and flushed the cleaned profile.
- P06-T06, P13-T09, P15-T03 and P15-T04 are therefore verified complete.
- This does not satisfy P08-T08: the existing receipt harness validates grant/idempotency logic but a real MarketplaceService Developer Product receipt is still required.

## P15 Release
- [x] P15-T01 Production icon/thumbnails/metadata
- [x] P15-T02 Privacy/content questionnaire
- [x] P15-T03 Publish canonical build privately
- [x] P15-T04 Repeat complete P13 journey in published private place
- [x] P15-T05 Record build hash/place version/rollback
- [x] P15-T05A Final store screenshots must be captured from the actual accepted build and accurately represent the shipped art/UI
- [x] P15-T05B Final “looks finished” sign-off: no placeholder icon/text/material/model, no default UI styling, no debug overlays, no known visual P0/P1/P2 defect
- [!] P15-T06 Controlled public release after paid receipt/rejoin evidence and zero P0/P1 defects

P15 implementation note: production title/short/full description, genre/audience copy, icon/thumbnail art direction and screenshot-truth rules are locked in `docs/RELEASE-METADATA.md`. Canonical privacy/content questionnaire answers are locked in `docs/PRIVACY-CONTENT.md`, and the full P13/P14 execution script is locked in `docs/RUNTIME-QA.md`. P15-T01 and P15-T02 are now `[x]`: accepted icon/thumbnails/metadata are recorded and Creator Dashboard reports **Fragebogen komplett ausgefüllt**, content maturity `Minimal`, no labels/non-conforming regions and no additional age restriction. Evidence: `docs/evidence/2026-10-04-release-dashboard/`. P15-T05 is now `[x]`: accepted source commit `17f0121004e3d9fc33a1e57fc39ffa020d0f1339`, Universe `10768815106`, Place `133160458509988`, published place version `v6`, rollback target `v5`. Live monetization IDs are Revive `3715818292`, Stability `3715820324`, Coin Multiplier GamePass `2002022878`.

Historical store-art review note (superseded by the 2026-10-04 final store-art acceptance below): the earlier prepared 1920x1080 thumbnails were rejected because Roblox CoreGui/topbar/player-list chrome was still visible in the captured frames. Commit `513863ae94d044661930bf9956a4c36788b9bccf` adds a strictly Studio + VisualOnly-gated capture controller that hides CoreGui/topbar for clean store captures without affecting production players. Exact-commit verification passes StyLua, Selene (0/0), 16 pure-Luau test files, 16,000-placement simulation, release-readiness and Rojo build. That earlier gate remained open until the fresh clean captures were produced and reviewed; the later 2026-10-04 acceptance closes those gates. Evidence: `docs/evidence/2026-10-01-store-art-clean-capture-gate.md`.

2026-10-04 final store-art note: fresh 1920x1080 gameplay-only captures are now accepted under `docs/evidence/2026-10-04-final-store-art/`. The frames contain no Studio chrome, Roblox CoreGui controls, debug/performance overlays or desktop UI; avatar/progression remain readable and the cosmetics frame uses only real equipped V1 cosmetics/themes. P15-T05A and P15-T05B are therefore closed. P15-T01 is closed after accepted icon/thumbnails/metadata were recorded in Creator Dashboard evidence.

Final release-gate recheck after terminal restart: main head `9a9d2b1c6c3a3c5b5f2a7f75337c82af169840da` has green GitHub Actions CI run `36888296618`. Local verification again passed StyLua, Selene (0/0), all 14 pure-Luau test files, the 16,000-placement reachability simulation, release-readiness and Rojo build. A stale Rojo server rooted at `/private/tmp/rs-live` on port 34872 was detected and terminated before further canonical-place work. Store assets are prepared locally (512x512 icon, three 1920x1080 thumbnails and accepted-build viewport captures); upload/submission is not marked complete until the Creator Dashboard action is actually performed. Evidence: `docs/evidence/2026-10-01-final-release-gate-check.md`.

## P16 Post-launch
- [!] P16-T01 First telemetry review
- [!] P16-T02 Evidence-based difficulty/balance patch
- [!] P16-T03 New step patterns/themes/cosmetics cadence

## Definition of Done

Rising Steps V1 is complete only when a player can repeatedly climb, understand danger, fail, retry, progress and rejoin without broken camera/control/generation state **and** the accepted private-place build visually reads as a finished commercial Roblox game. A functioning mechanic, green CI, or generated platforms alone are never sufficient. Early/mid/high-altitude gameplay, HUD, result flow and shop must all pass the authored-art, mobile-readability, audio/VFX and screenshot-quality gates above before the project may advance to Perfect Jump.

## 2026-10-03 Mobile difficulty/landing corrective pass

- [x] Real-phone landing/difficulty regression repaired and revalidated: larger targets, shorter rises/gaps, wider scoring tolerance, clearer platform-wide acceptance messaging and first-step guidance. The full Studio harness completed all 42 steps after the corrective pass. Evidence: `docs/evidence/2026-10-03-mobile-regression-revalidation.md`.

## 2026-10-03 opening-density correction / production v17

- [x] Real-phone visual regression corrected after the earlier landing-forgiveness pass: base footprint 6.25 studs, wider verified gaps/rises, forward-flowing direction changes, authored curved first three targets and only four upcoming steps rendered on compact clients.
- [x] Regression protection updated: 16 pure-Luau files pass, 16,000/16,000 reachability placements pass, StyLua/Selene and release-readiness pass.
- [x] Canonical source commit `e3c8111` synced into the existing production Place `133160458509988` (Universe `10768815106`); no replacement Place/Experience created.
- [x] Studio publish succeeded as Place version `v17`; post-publish iPhone XR smoke visually confirmed separated platforms / curved opening and reported `0 CreatorErrors`.
- [x] Canonical build SHA-256 for this publish: `1cf00ac9c82c9fa8c87434e760cfc19bb3151c538eeeab4bd7d1775b6ced89b5`.

## 2026-10-03 tablet visual-clarity corrective pass

- [x] Real iPad 10th Generation (`1180x820`) Device Simulator visual pass completed on the current build. HUD, tutorial, Shop, avatar framing and touch-control clearances remain inside the playable viewport.
- [x] Step collision bodies reduced from `1.25` to `0.78` studs so the climb reads as lightweight platforms rather than stacked dark slabs; regression guards enforce `0.65 <= CollisionThickness <= 0.80`.
- [x] Service Decks background skyline moved farther from the climb path, spread laterally and lightened so its city masses no longer visually merge with the first four targets.
- [x] Exact post-fix verification: StyLua pass, Selene `0/0`, 16 pure-Luau test files, 16,000/16,000 reachability placements, release-readiness and Rojo build pass; iPad runtime reports `0 CreatorErrors`, 846/900 BaseParts and 21/96 Beams/Lights.
- [x] P14-T08 closed from real tablet plus previously accepted desktop composition evidence.
- [!] P14-T02 remains externally blocked: Studio Device Simulator confirms the touch layout, but OS mouse events do not exercise Roblox touch movement and `VirtualInputManager:SendTouchEvent` is RobloxScript-security restricted from CommandBar; no touch-interaction pass is claimed without real evidence.

## 2026-10-03 tablet clarity production verification

- [x] Canonical clarity source `eeb0c67` is on `origin/main`.
- [x] Existing Place `133160458509988` / Universe `10768815106` received the clarity source; no replacement Place/Experience was created.
- [x] Independent verification was performed by stopping Rojo, closing/reopening the cloud Place and running iPad 10th Generation `1180x820` PlaySolo directly from Roblox. The fresh cloud build visibly contains the thinner platform bodies and separated skyline, with `0 CreatorErrors`.
- [!] P14-T02 tablet touch remains externally blocked until movement/jump is exercised through genuine touch input rather than OS mouse emulation.
- [x] P14-T04 controller closed on 2026-10-04 by real Studio Controller Emulator Gamepad1 event + avatar-response evidence.

## 2026-10-04 controller-emulator acceptance

- [x] Roblox Studio Controller Emulator connected as `Gamepad1` in the canonical Rising Steps control stack.
- [x] `Thumbstick1` input was observed by `UserInputService` at magnitude `1.00`; the avatar moved `6.63` studs with `Humanoid.MoveDirection=1.00`.
- [x] `ButtonA` input was observed by `UserInputService`; the same production jump path raised the avatar `7.27` studs.
- [x] Final harness result: `connected=true stick=true move=true buttonA=true jump=true pass=true`.
- [x] Existing selectable/focus routing covers Shop, Close and Retry for gamepad UI navigation; no controller-only gameplay branch is introduced.
- [x] The QA harness is Studio-only and default-disabled in `RuntimeQaConfig`; temporary ButtonA keyboard remapping used to make the emulator deterministic was restored to the original Studio preference immediately after the run.
- [!] Tablet genuine-touch interaction remains separate and externally blocked.

P15 final-store capture reconciliation (2026-10-04): the authoritative acceptance is `docs/evidence/2026-10-04-final-store-art/README.md`, which verifies all three 1920x1080 final thumbnails and their SHA-256 hashes. P15-T05A remains closed.

## 2026-10-04 final release-gate status

- [x] Creator Dashboard icon / production metadata / exactly-three final thumbnail set verified.
- [x] Creator Dashboard content questionnaire verified complete at `Minimal` with no labels/age restriction.
- [x] GitHub Actions CI run `37191956028` green for head `45f38d5b3fee2e63c54a40a5dbb4974f7632ae72`.
- [!] Remaining pre-release acceptance is external only: genuine phone/tablet touch, physical mobile-speaker listen, real paid Developer Product receipt + rejoin, and Roblox audience/publishing review.
- [!] No 50,000 Robux accelerated-review payment and no Developer Product purchase was initiated automatically.
- Evidence: `docs/evidence/2026-10-04-release-gate-status.md`.

## 2026-10-04 concept visual-polish pass

- [x] Native Roblox concept-quality presentation pass implemented for this game; no static concept screenshot is used in gameplay.
- [x] Lighting/VFX and native ScreenGui styling are test-guarded and pass local static verification plus Studio PlaySolo runtime QA.
- [x] Evidence: `docs/evidence/2026-10-04-concept-visual-polish.md`.

## 2026-10-04 concept-fidelity pass 2

- [x] Opening gameplay view now follows the approved Fantasy Sky Realms concept rather than the previous industrial/block-heavy language.
- [x] Visible gameplay collision replaced by authored grass/stone island presentation while deterministic collision remains unchanged.
- [x] Opening vista now contains hero islands, cloud layering, portal and warm sun focal point.
- [x] HUD now mirrors the approved concept hierarchy with branded logo, top progress capsule, currency pill and vertical quick actions.
- [x] Final PlaySolo run: 0 CreatorErrors; lane presentation 789 BaseParts / 6 Beams+Lights; 19 pure-Luau tests and 16,000 reachability placements passed.

## 2026-10-05 concept production publish

- [x] Concept-fidelity source synced to the existing production Place `133160458509988` and published successfully as `v22`.
- [x] No new Place/Experience created; canonical `main` remained the only source line.
- [x] Concept-fidelity pass 3: live featured shop + progress/daily/visual cards, unobstructed medium/large hero vista and warmer lavender/sunset world grade are implemented and test-guarded.

## 2026-10-05 graphic-fidelity pass 4

- [x] Source-side concept graphic fidelity implemented: Parallax island chain, layered sun halo, broken sky-bridge/cloud ribbons, sparse moss-edge and hanging-crystal step detail.
- [x] Contextual-menu rule preserved: full Shop/Daily/Style/Revive/Result surfaces are not permanently visible during normal gameplay.
- [x] CI verification green on run `37288709962`; merged source commit `3a41806`.
- [x] Fresh Roblox Studio PlaySolo visual acceptance passed: Fantasy Sky Realms route/avatar readability verified; contextual menus remained closed by default; runtime log reported 859 BaseParts / 6 Beams+Lights with no gameplay CreatorError/ScriptError match.
- [x] Graphic-fidelity source published to existing canonical Place `133160458509988` as `v24`; no new Place/Experience created. Evidence: `docs/evidence/2026-10-05-graphic-fidelity-runtime-publish.md`.
