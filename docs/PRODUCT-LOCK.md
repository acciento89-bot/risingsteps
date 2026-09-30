# Rising Steps V1 Product Lock

This document is the canonical product definition for P00. Changes after P00 require an explicit ledger note and a reason.

## Score vocabulary

- **Height**: the number of validated upward steps landed in the current run. Height is the primary competitive metric.
- **PB**: the player's persisted personal-best Height.
- **Streak**: consecutive validated landings graded Clean or Perfect without a Miss, fall, revive reset or invalid sequence.
- **Perfect**: landing center error <= 22% of the current step's usable half-width and vertical velocity is downward or near-neutral at contact.
- **Clean**: valid landing center error <= 55% of usable half-width and not Perfect.
- **Miss**: any contact outside Clean tolerance, falling past the intended step, landing an out-of-sequence step or failing the step.
- Height leaderboards never include purchased multipliers, revives or coin boosts.

## Movement and jump model

- Keep the standard Roblox Humanoid locomotion model for familiarity and controller/touch parity.
- Server-authored defaults: WalkSpeed 16, JumpPower 50, UseJumpPower true, standard workspace gravity 196.2.
- AutoJump is not required for the skill loop; the production experience must expose the normal mobile jump control.
- No double jump, dash, air boost or paid movement advantage in V1.
- Landing validation uses server-observed character/root motion and step sequence, never a client-supplied score.
- Runtime tuning may adjust values only within the tested reachability envelope; any change requires generator tests to be rerun.

## Step timing and reachability envelope

Baseline safe geometry:
- Visible gameplay pad target footprint: 7 x 7 studs, with authored visual trim outside the collision truth.
- Collision thickness: 1.25 studs.
- Baseline vertical rise: 3.4-4.6 studs.
- Baseline horizontal center distance: 4.5-7.5 studs.
- Hard V1 generator caps before runtime tuning: vertical rise <= 5.0 studs and horizontal center distance <= 8.25 studs.
- Generator must account for previous pad dimensions, not only center-to-center distance.

Baseline lifetime:
- Newly landed/current step is protected for 1.25 s.
- A step becomes eligible for expiry only after the player has progressed at least two validated steps above it.
- Warning duration starts at 1.35 s and may compress with difficulty, never below 0.70 s.
- Expiry transition lasts 0.35 s; collision is removed exactly once at the end of that transition.
- At least the current step and the next two legal targets remain collision-safe during normal play.

## Difficulty ramp and fail/revive rules

Difficulty rises continuously through three visual/gameplay bands:
- **Band A — Service Decks, Height 0-11:** horizontal 4.5-6.0, vertical 3.4-4.0, generous warning, no aggressive alternation.
- **Band B — Cloudline Works, Height 12-29:** horizontal 5.0-7.0, vertical 3.6-4.4, shorter warning, mixed lateral patterns.
- **Band C — Stratosphere Spine, Height 30+:** horizontal 5.5-7.5 with rare tested peaks to 8.25, vertical 3.8-4.6 with rare tested peaks to 5.0, fastest legal warning, denser pattern variation.

Fail rules:
- A run fails once when the Humanoid dies or the root falls 28 studs below the highest validated landing height.
- Retry must restore a fresh authoritative run with no stale streak, pending landing or expired-step state.
- Revive, if purchased/available, may be offered only after failure and returns the player to a recent server-validated safe step.
- A revive resets Streak and cannot improve leaderboard Height by itself.
- Maximum one paid revive per run in V1.

## Ethical monetization and cosmetics

Allowed V1 categories:
- One post-failure revive.
- Temporary step-stability/slowdown boost with explicit duration and no leaderboard advantage.
- Coin multiplier affecting only earnable cosmetic currency.
- Trails, landing bursts, pad skins and environment themes.
- Optional permanent cosmetic bundles/passes.

Guardrails:
- No purchase prompt on spawn, during tutorial or during an active jump.
- No paid jump height, walk speed, hitbox, reach or hidden score multiplier.
- Purchases never alter authoritative leaderboard Height.
- All grants are server-authoritative, allowlisted, idempotent and persisted where applicable.
- The shop must show the exact benefit before purchase.

## Measurable release criteria

P00 quality targets become release gates:
- First-time player can identify “climb upward before old steps disappear” in <= 10 s without developer text.
- Retry input to controllable avatar target <= 2.0 s in normal runtime conditions.
- Generator simulation: at least 1,000 consecutive seeded placements with zero unreachable, overlapping, buried or off-policy steps.
- Long-climb runtime: 20 minutes without unbounded part growth, duplicate fail triggers, camera loss or stale state.
- Compact-phone view always preserves avatar, current landing surface and at least two upcoming legal targets.
- No known P0/P1 defects at release; no visible placeholder/default-production UI/art in accepted screenshots.
- Production screenshots must pass at spawn/tutorial, early climb, mid climb, high altitude, warning/expiry, result and shop.
- Real receipt + persistence/rejoin evidence remains a hard public-release dependency.

## Visual identity board

Concept: **Aerial Maintenance Spine** — the player climbs a stylized suspended infrastructure route above a deep city/cloud void. It is architectural and authored, not a set of floating cubes.

Palette:
- Foundation: near-black slate `#121722`, deep steel `#202938`, cool concrete `#3A4657`.
- Warm authored accent: amber `#FFB454` for architectural lights and safe guidance.
- Gameplay-state accent: cyan `#5EEBFF` for current/next-step readability.
- Warning: amber/orange `#FF9F43` plus pulse/edge motion.
- Expiring/fail: coral-red `#FF5D6C` plus fracture/retract motion; color alone is never the signal.
- Perfect feedback: restrained pale-cyan/white flash; Neon is limited to short feedback moments.

Materials:
- Painted metal, concrete, rubberized landing inserts, grated steel, frosted/smoked glass and restrained emissive strips.
- Raw Plastic blocks and unmodified default Parts are forbidden as final visible art.

Typography/UI:
- Gotham/BuilderSans-family Roblox-native typography with bold numeric hierarchy.
- 8 px-equivalent spacing rhythm, 12-16 px-equivalent corner radii, compact glass/steel panels.
- Icons use a single rounded geometric language with consistent stroke weight.
- Default Roblox-looking production panels/buttons are release blockers.

World silhouette language:
- Cantilever pads, ribbed undersides, brackets, short rail fragments, cable anchors, maintenance lamps and vertical spine structures.
- Visible pads use at least six silhouette/trim variants while collision stays predictable.
- Supports and architectural fragments explain why pads exist; nothing should read as random blocks in space.

Do not use:
- Empty baseplate framing, checkerboard platform spam, rainbow/random material assignment, giant Neon slabs, placeholder icons, debug labels, flat repeated cubes, copy-pasted free-model scenery or visually noisy particles.

## Camera composition targets

Core camera is third-person and keeps the avatar as the visual anchor.

Compact phone:
- Avatar occupies roughly 24-30% of viewport height during normal climb.
- Current pad and next 2-3 legal targets must be visible whenever geometry permits.
- HUD reserves the central 62% width and lower gameplay zone for unobstructed movement reading.

Tablet:
- Avatar target 21-27% of viewport height.
- Use added width for environmental depth, not larger HUD chrome.

Desktop:
- Avatar target 18-24% of viewport height.
- Upcoming geometry remains large enough to judge landing centers without zoom.

All:
- Vertical follow uses damped interpolation; no abrupt snap on ordinary landings.
- Camera cannot pass through primary step geometry during the standard loop.
- Failure/retry restores deterministic framing immediately.
- Camera FOV changes, if any, are subtle feedback only and never change reachability perception.

## Altitude-art progression

### Band A — Service Decks (Height 0-11)
Dense lower maintenance architecture, warmer practical lights, visible city depth, thicker structural supports and calmer atmosphere. Teaches the visual language and danger states.

### Band B — Cloudline Works (Height 12-29)
The route enters cloud layers and open scaffolding. More exposed cantilevers, cable anchors, glass/rubber inserts and stronger parallax. Lighting cools while amber guidance remains consistent.

### Band C — Stratosphere Spine (Height 30+)
Sparse high-altitude structural ribs, antenna/service assemblies, colder sky and stronger depth haze below. Geometry feels more exposed but never visually ambiguous. Cyan gameplay-state readability and warm navigation lights remain consistent with lower bands.

Transitions are gradual; the three bands must read as one world, not three unrelated themes. Cosmetic environment themes may reskin authored surfaces and atmosphere but may not alter collision, target size, reachability or competitive truth.
