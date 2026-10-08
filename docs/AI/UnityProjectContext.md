# Rising Steps Unity context

<!-- unity-onboarding:generated:start -->
Analyzed 2026-10-08 at source `9ae39219ba1c48e1763a633588817e498dfbc875`; root `/Users/piotrkaminski/Developer/RisingSteps`.

Confirmed: Unity6000.6.4f1, Built-in rendering (Standard materials, no SRP package), legacy Input Manager with custom UGUI joystick/jump and swipe orbit. UGUI2.6.0 and Test Framework1.8.0; no networking, native billing/ads, localization framework, or Unity MCP package/tools detected. No first-party assembly definitions or automated tests found.

Enabled startup scene `Assets/Scenes/Main.unity` contains `GameBootstrap`, which constructs camera, capsule runner, HUD, touch controls and `RisingCourse` at runtime. `PlayerMotor` uses CharacterController and ordered collision landings. `OrbitCamera` follows position with yaw/pitch changed only by player input; preserve this behavior. `RisingCourse` currently generates12 steps and decorations with visible Unity cylinders/spheres; it holds currency/style/height only in memory. ClaimDaily currently adds100 on every call. Settings/inbox/audio callbacks are empty; Shop is preview text. Final realm completion/portal flow, persistent profiles, real daily guard, DE/EN, audio/haptics/VFX and concept-quality art remain open.

Code roots: Core/GameBootstrap; Gameplay/RisingCourse, PlayerMotor, StepMarker; Camera/OrbitCamera; Input/VirtualJoystick and PressButton; UI/UiFactory and SafeAreaFitter; Editor/ProjectBootstrap and BuildAutomation. No custom shaders, authored production character or animation controllers detected. Style directly changes the capsule material.

Conventions: `Kamilunavo.RisingSteps` feature namespaces, sealed MonoBehaviours, private underscore fields, compact existing methods. BuildAutomation currently builds development targets only; explicit release/version/simulator exports will be needed for TestFlight. Prior ledger records Android IL2CPP and signed iOS development build success; fresh runtime/device evidence is not established by this source inspection.

Constraints: main canonical; portrait/touch-first, safe areas and48-point targets, next2–4 steps visible, no forced camera yaw, contextual Shop/Daily/Style, immediate recovery, no legacy platform artifacts. Binding concept `docs/concepts/PRIMARY-CONCEPT.png`. Read README/MASTER-PLAN/ART-DIRECTION/ledger before implementation.

Unknowns: measured reachability, native runtime performance, physical input, lifecycle, final store privacy. Current parameters (jump8.4, gravity22, speed5.8, step center distance4.2–5 plus lateral drift and rise.75–1.05) warrant a physics-derived reachability test before content expansion; center spacing alone does not prove an unreachable edge-to-edge jump.

Evidence inspected: AGENTS.md, README.md, docs/MASTER-PLAN.md, ART-DIRECTION.md, RISING-STEPS-V1-LEDGER.md; ProjectVersion/EditorBuildSettings/GraphicsSettings; Packages/manifest.json; representative first-party source files listed above. Read-only onboarding changed this document only; no Unity assets modified.
<!-- unity-onboarding:generated:end -->
