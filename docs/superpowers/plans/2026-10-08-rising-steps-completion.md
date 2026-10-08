# Rising Steps Completion Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Finish the existing fantasy island game to a verified internal native delivery.
**Architecture:** Preserve CharacterController and player-controlled OrbitCamera; separate persisted pure rules, course/world composition, contextual HUD and feedback. Development-only QA uses isolated saves and real controller inputs.
**Tech Stack:** Unity6000.6.4f1, Built-in rendering, C#, UGUI2.6.0, pinned native SDKs only for optional commerce.
**Spec:** docs/superpowers/specs/2026-10-08-rising-steps-completion.md

## Global Constraints
- Main canonical; existing bundle com.kamilunavo.risingsteps/ASC6819872415; no new ASC app entry, no public release.
-12 steps/realm, realm0..2, stars1/2/3 as spec, ordered duplicate-proof landings; daily100..160/75 challenge, UTC dates and idempotence.
- Free camera yaw, immediate checkpoint recovery,48-point safe controls, DE/EN/reduced motion/high contrast.
- Commit actual graphics/metas/packages/settings; only generated caches disposable after remote tree verification.

## Review Focus
- First touch on transparent root must reach camera; simultaneous fingers never steal joystick ownership (task2 runtime gesture/raycast check).
- Saved completed/current run must restore exact realm and safe step, not respawn at origin (task1 roundtrip + task3 reload).
- Modal/focus loss must clear pending jump and freeze active timer (task2 input lifecycle + task3 probe).
- UTC day crossing/replayed claims and purchased style reselection must not mint/spend twice (task1 pure rules).
- All course patterns must be physically reachable without invisible decorative collisions (task1 math + task3 real controller climb).

### Task1: persisted adventure rules and course integration
Create Core/RisingProfile.cs, RisingSave.cs, RisingRules.cs; Gameplay/CoursePatterns.cs; Editor/RisingValidation.cs. Modify RisingCourse.cs/PlayerMotor.cs/GameBootstrap.cs only at their integration points.
Interfaces: RisingSave.Parse(string)->RisingProfile, Load(), Save(RisingProfile); RisingRules.ClaimDaily(RisingProfile,DateTime)->bool, SelectStyle(RisingProfile,int)->bool, Land(RisingProfile,int,bool)->bool, Complete(RisingProfile,DateTime)->int; CoursePatterns.Points(int realm,int seed)->Vector3[13]. RisingCourse owns profile and exposes Height/Player/Steps/Paused plus StartRun(int,bool), Respawn(), ClaimDaily(), SelectStyle(int), CompletePortal().
- [x] Write missing-rule red tests: daily same/earlier/next day,7-day cap; duplicate/skipped landings; stars/unlock; owned-style replay; parse/range/array normalization; save roundtrip and seeded reachability.
- [x] Run batch editor execute RisingValidation.Validate and record failure before product implementation.
- [x] Implement pure rules/save/patterns; run tests green.
- [x] Integrate current motor/course, persistent checkpoint/portal, suppress duplicate callbacks and reset velocity on recovery; compile, runtime route baseline; commit.

### Task2: concept art, contextual controls and feedback
Create Visuals/SkyArt.cs, IslandArt.cs, RunnerArt.cs, RunnerAnimator.cs, PortalArt.cs, RisingFeedback.cs; UI/RisingHud.cs, HudIcons.cs; QA/RisingRuntimeQa.cs. Modify UiFactory/VirtualJoystick/PressButton/OrbitCamera/GameBootstrap and native haptics plugins. Assets/Art stores generated raster textures/provenance; Resources stores fonts/audio/shaders.
Interfaces: RisingHud.Initialize(RisingCourse), ModalOpen, Refresh(), ShowHome(), ShowSettings(), ShowDaily(), ShowStyle(); motor Paused/ResetMotion(); joystick ResetInput() and pointer ownership; feedback Jump/Land/Perfect/Fall/UI/Complete. World builders return actual meshes and collide only on route islands. Runner animator consumes velocity/grounded, never rotates camera.
- [x] Add runtime failure probes for modal pause/stale jump, actual UI raycasts, transparent-root camera access, daily/style idempotence and saved checkpoint reload.
- [x] Generate/inspect project-bound sky/terrain/foliage art via imagegen, author runner/islands/temple geometry; commit all assets/metas and provenance.
- [x] Implement48-point responsive contextual UI/DE-EN/settings, safe area; fix root raycast interception and multi-touch ownership; inspect compact/tablet/landscape/division surfaces.
- [x] Add sound/haptics/VFX/reduced-motion/high-contrast, actual runner animation; run probes and inspect actual game captures; commit.

### Task3: complete runtime/native delivery
Modify Editor/BuildAutomation.cs/ProjectBootstrap.cs for preserved build numbers, explicit target/release/simulator paths and icons. Add development-only QA input bridge/real-time soak; docs ledger and store delivery evidence.
Interfaces: BuildMacPreview(), BuildIOSSimulatorQa(), BuildIOS(), BuildAndroidRelease(), -versionName/-buildNumber/-buildOutput; QA -qaRising/-qaRisingSoak with isolated output/profile, no release probes.
- [x] Drive actual CharacterController/joystick/jump through12 steps for each pattern, portal next/replay, forced fall/recovery, pause/lifecycle/reload/daily/style. No teleport success claims.
- [x] Native iPhone captures/UI raycast/input matrix; official Duo available poses, explicit blocked state if unavailable.30-minute real-time soak and shader/Console checks; commit evidence.
- [x] Add optional commerce using own IDs/catalog and verified adapters, native SDK compilation/restore/replay checks. Keep genuine sandbox/reward/consent blockers open.
- [x] Fresh whole-change reviewer checks rules/input/save/native gates; fix justified findings with regressions.
- [x] Signed Release archive/internal TestFlight process/tester and Android AAB existing central key; verify signatures/payload/manifest. Update ledger with precise proven/unverified states.
- [x] Commit/push full Unity project and compare remote tree; retain final builds/QA evidence, clean regenerable caches, continue next game per authorized order.
