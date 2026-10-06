# Camera / retry / claim corrective pass - 2026-10-06

## Scope

This corrective pass addresses three player-reported production defects without changing the approved Fantasy Sky Realms art direction:

- the camera continuously rotating itself behind the avatar;
- Retry feeling delayed/unresponsive after a failure;
- Daily Claim appearing to do nothing.

## Canonical source

- Gameplay corrective commit: `83588ee` (`fix: manual camera and responsive retry claim flow`).
- Production-parity metadata commit: `ccacfce`.
- Canonical branch: `main`.
- Production build marker: `Workspace.RisingStepsBuildRevision = "2026-10-06-camera-retry-claim"`.

## Camera correction

- Production camera now stays on Roblox `Enum.CameraType.Custom`.
- The client no longer switches to `Scriptable`.
- The client no longer overwrites `Camera.CFrame` every RenderStepped frame.
- The player therefore controls camera yaw/rotation normally while the avatar remains the camera subject.
- Static regression test: `tests/camera-manual-control.spec.luau`.

## Retry correction

- The failed run no longer waits on the old delayed respawn/reset branch.
- `Retry` routes to one authoritative `restartFailedCourse` path.
- Authoritative run state is reset immediately, then the living character is repositioned/rebound or a character reload is requested when required.
- The HUD acknowledges the click immediately with `RESTARTING...` and includes a recovery guard so the button cannot remain permanently disabled.
- Static regression test: `tests/retry-claim-response.spec.luau`.

## Daily Claim correction

- Both Daily Claim buttons use one guarded request path.
- Click acknowledgement is immediate via `CLAIMING...`.
- Duplicate clicks are blocked while a claim is pending.
- A 1.25-second bootstrap re-sync recovers from a throttled/lost response instead of leaving the control apparently dead.
- The existing server-authoritative daily reward path remains unchanged.

## Verification

Fresh canonical-source verification passed after the correction:

- StyLua check: pass.
- Selene: 0 errors / 0 warnings / 0 parse errors.
- Pure Luau tests: 24 files passed.
- Reachability simulation: 16,000 placements passed.
- Release-readiness: pass.
- Rojo build: pass.

Studio functional QA completed successfully. Relevant runtime gates included:

- `retry_respawn_ready`: PASS.
- `retry_resets_run`: PASS with Height 0 / Streak 0 / Failed false.
- `daily_reward_path`: PASS.
- full 42-step climb: PASS.
- `FUNCTIONAL_COMPLETE` and `COMPLETE`: emitted.

## Production parity and publish

- Existing Universe: `10768815106`.
- Existing Place: `133160458509988`.
- No new Place or Experience was created.
- Studio initially loaded published version `v24`.
- Rojo sync was explicitly accepted for the canonical `RisingSteps` session.
- A direct Studio parity probe reported:
  - `retry=true`
  - `scriptable=false`
  - `claiming=true`
- Studio publish state reached `PublishSuccessful`.
- Studio reported `Published new changes in "Steigende Schritte" to Roblox.`
- Published Place version: `v25`.

