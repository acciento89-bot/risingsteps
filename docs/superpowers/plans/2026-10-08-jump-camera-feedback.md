# Jump-camera feedback implementation plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Inline execution; canonical main per AGENTS.md.

**Goal:** Eliminate automatic view/input turning during jumps while preserving manual orbit.

**Architecture:** Keep OrbitCamera as the owner of manual angles and PlayerMotor as the existing camera-relative controller. First instrument actual rendered orientation under fixed-thumb input; change the smallest confirmed cause. Existing development-only RisingRuntimeQa owns regression probes.

**Tech Stack:** Unity6000.6.4f1, C#, CharacterController, legacy touch/UGUI, Built-in rendering.

**Spec:** docs/superpowers/specs/2026-10-08-jump-camera-feedback.md.

## Global constraints
- Camera orbit is player-controlled; never force yaw behind the character every frame.
- Existing speed6.2/jump9.2/gravity22 and course rewards/progression remain unchanged.
- No release/debug UI; development-only probes and isolated QA saves.
- One heavy Unity/native job or one booted simulator at a time on this8GB host.
- Commit verified slices to main, verify full remote Unity backup before generated cleanup. No public release.

## Review focus
- Fixed thumb during ascent/descent/strafe; render angles follow only player intent.
- Intentional orbit at multiple angles still defines camera-relative movement.
- Jump/joystick raycasts remain UI-owned and do not start camera gestures.
- Pause/focus/restart clear input and preserve progress/recovery.
- Different render rates and real native code paths remain explicit verification targets.

### Task 1 restore player-owned jump camera
- [x] Add actual rendered-angle/fixed-thumb regression to Assets/Scripts/QA/RisingRuntimeQa.cs; save telemetry/capture before assertion.
- [x] Run fresh Mac production baseline and observe correct behavioral RED; distinguish orbit-state and rendered-angle values.
- [x] Apply smallest confirmed correction in Assets/Scripts/Camera/OrbitCamera.cs, keep input/physics balance.
- [x] Fresh desktop camera GREEN70 and actual full course/UI GREEN88, including true window resizing for desktop landscape probes.
- [ ] Run regression GREEN and existing actual controller/course/UI/editor suite; inspect captures. Obtain focused fresh review if required by execution workflow.
- [ ] Build fresh native candidate and signed internal updates/backups; record physical/commerce/fold limitations in canonical ledger.
