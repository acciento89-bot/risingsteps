# Rising Steps approved distinct worlds implementation plan

> Execution: source implementation in this chat; coordinated Unity/native gates belong to the root delivery worker. User approved the October 10 board and execution. No additional approval pause, commit, version bump, paid package or provider.

**Goal:** Preserve the clear cream/green atlas and the tested climb while making the actual meadow, waterfall and temple environments unmistakably different.

**Architecture:** Extend IslandArt, RealmArt, PortalArt and SkyArt. Keep CoursePatterns, surface collider geometry, marker indices, controller, profiles and commerce authoritative and unchanged. Geometry uses existing cached atlas materials and owned runtime meshes, then the existing static batch boundary. Gradient sky contains no illustrated course; real scenery supplies silhouettes.

**Reference:** `/Volumes/SSK SSD/Kamilunavo/Artifacts/KamilunavoDelivery-20261009/concepts-20261010/rising-steps-3-ansichten.png` and docs/ART-DIRECTION.md.

## Constraints and acceptance
- Actual 3D environments; no substituted screenshots or entire-course billboards.
- Meadow: green grass/daisies/tree, pale limestone, clear blue sky; no cascade pool kit.
- Water: pale terraced limestone, turquoise shallow edge pools, lilies/reeds/curtain falls, cool sky; no meadow tree/daisy kit.
- Temple: dry ochre paving, carved columns, stairs, broken lintels and golden destination arch, lavender/peach sky; no meadow or waterfall kit.
- Unchanged 12 reachable landing colliders, free central route, player-controlled View and desktop orbit; movement-before-jump synthesis remains excluded.
- Cream/green atlas hierarchy, DE/EN and compact portrait/landscape 48-point targets retained.
- Generated assets remain owned by their island/scenery roots. Bounded geometry and shared cached materials; physical performance remains a separate measured gate.

## Task 1: Real world kit with evidence
- [x] Add RisingConceptValidation.Validate with actual mesh/collider, vegetation separation, lighting and asset destruction assertions; baseline should reject meadow falls and copied flora.
- [x] Keep IslandArt's landing fan untouched; replace non-colliding cliff silhouettes and terrain detail with realm-specific strata/tiles.
- [x] Dress water with terraced pool banks/reeds/lilies and layered falling water; dress temple with stone paving, carvings, staggered ruin structures and no wet/green kit; enrich meadow tree/flowers at perimeter.
- [x] Match portal stone to realm and enrich the actual column/arch destination without changing arrival behavior.
- [x] Replace panorama gameplay sky with cool/blue or lavender/peach gradients; retain owned cloud material.
- [ ] Validate all generated normals, central-route detail bounds, one collider per route island, zero decorative colliders, bounded vertices, mesh/material destruction, distinct palettes.

## Task 2: Atlas readability and runtime acceptance
- [x] Retain atlas art composition and clarify DE/EN realm descriptions; retain actual unlock/star states and fixed Continue.
- [x] Extend existing runtime course probe to reject copied flora/sky background and require actual unique structures; retain movement-before-jump rendered camera tests.
- [ ] Root runs RisingValidation.ValidateAll, CommerceValidation.Validate, Kamilunavo.RisingSteps.Editor.CommerceRecoveryValidation.Validate; BuildAutomation.BuildMacPreview then isolated RISING_QA camera/tutorial/course capture runs.
- [ ] Root inspects compact portrait/landscape screenshots against board, actual 36 controller landings, lifecycle and synthetic mouse boundary; records remaining physical/performance limitations.

## Review focus
Realm rebuild destroys original and batched meshes/materials. Flora or ruin details never block the landing disk. Water surfaces do not obscure landing boundaries. Golden arch stays visible from route camera. Compact DE/EN labels do not overlap image or actions. No generated sky material persists after world-root destruction. No unauthorized Unity/native work or release mutation.

## Focused lower-rock finish pass — 2026-10-10
- Preserve the seeded 24-point landing fan, vegetation random stream and exact upper two cliff rings.
- Add independently seeded asymmetric lower strata: coherent per-ring radial variation, shear and uneven ledges; use separated rock-face vertices for physical faceted normals.
- Replace outlined Voronoi cells with one restrained seamless 256×256 neutral mineral-grain texture, shared across the finite stone kit (65,536 texels, mipmapped RGB24).
- Fixture asserts upper-lip coordinates, lower mass asymmetry/height variation/faceted normals, finite/bounded vertices and zero cliff colliders. Root owns actual editor and before/after rendered validation.
