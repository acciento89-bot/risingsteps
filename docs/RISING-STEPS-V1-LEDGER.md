# Rising Steps V1 Ledger

Status: `[ ]` open · `[~]` implemented/not device verified · `[x]` verified · `[!]` blocked

## P00 Reset/product
- [x] P00-T01 native iOS/Android direction
- [x] P00-T02 12-step realm loop locked
- [x] P00-T03 portrait concept composition
- [x] P00-T04 bundle IDs
- [x] P00-T05 legacy runtime removed from current tree

## P01 Unity foundation
- [x] P01-T01 Unity/C# structure
- [x] P01-T02 editor scene bootstrap
- [x] P01-T03 portrait + 60 FPS
- [x] P01-T04 safe-area Canvas
- [x] P01-T05 first editor compile
- [ ] P01-T06 iOS dev build
- [ ] P01-T07 Android dev build

## P02 Controls/camera
- [x] P02-T01 touch joystick
- [x] P02-T02 jump
- [x] P02-T03 keyboard fallback
- [x] P02-T04 player-controlled orbit camera
- [x] P02-T05 instant fall recovery
- [~] P02-T06 compact-phone ergonomics
- [ ] P02-T07 physical iOS verification
- [ ] P02-T08 physical Android verification

## P03 Rising Steps gameplay
- [x] P03-T01 12-step generated vertical slice
- [x] P03-T02 ordered landing progression
- [x] P03-T03 crystal rewards
- [x] P03-T04 realm progress meter
- [~] P03-T05 reachability/balance runtime pass
- [ ] P03-T06 final step pattern library
- [ ] P03-T07 portal/realm completion flow
- [ ] P03-T08 multi-realm progression

## P04 UI
- [x] P04-T01 Height HUD
- [x] P04-T02 crystal HUD
- [x] P04-T03 Shop button/panel
- [x] P04-T04 Daily button/panel
- [x] P04-T05 Style button/panel
- [x] P04-T06 Daily session claim action
- [x] P04-T07 style preview switch
- [ ] P04-T08 settings
- [ ] P04-T09 DE/EN
- [ ] P04-T10 reduced motion

## P05 Production art
- [~] P05-T01 fantasy palette/material foundation
- [~] P05-T02 procedural floating-island foundation
- [ ] P05-T03 final character + animations
- [ ] P05-T04 authored island/step kit
- [ ] P05-T05 waterfalls/cloud kit
- [ ] P05-T06 portal VFX
- [ ] P05-T07 optimized lighting/post FX
- [ ] P05-T08 screenshot-quality gate

## P06 Audio/haptics
- [ ] P06-T01 jump/landing
- [ ] P06-T02 perfect/realm cues
- [ ] P06-T03 fall/recovery
- [ ] P06-T04 haptics
- [ ] P06-T05 ambience/music

## P07 Persistence/progression
- [ ] P07-T01 versioned profile
- [ ] P07-T02 crystals
- [ ] P07-T03 styles
- [ ] P07-T04 realm unlocks
- [ ] P07-T05 migration tests

## P08 Retention/monetization
- [ ] P08-T01 persistent daily streak
- [ ] P08-T02 daily challenge
- [ ] P08-T03 achievements
- [ ] P08-T04 cosmetic catalog
- [ ] P08-T05 StoreKit sandbox
- [ ] P08-T06 Play Billing sandbox
- [ ] P08-T07 restore/retry

## P09 QA/release
- [ ] P09-T01 EditMode tests
- [ ] P09-T02 PlayMode climb/fall tests
- [ ] P09-T03 iPhone safe-area matrix
- [ ] P09-T04 Android aspect matrix
- [ ] P09-T05 30-minute stability
- [ ] P09-T06 performance/thermal
- [ ] P09-T07 App Store assets
- [ ] P09-T08 Play Store assets
- [ ] P09-T09 TestFlight RC archive + upload
- [ ] P09-T10 TestFlight processing + internal tester assignment
- [ ] P09-T11 TestFlight install/smoke test on physical iPhone
- [ ] P09-T12 staged release

## Next open task
P09: native iPhone/Duo/soak and internal distribution proof. Physical install/sandbox remain device gates.


## Unity 6.6 bootstrap verification - 2026-10-06
- [x] Project imported and compiled successfully with Unity 6000.6.4f1.
- [x] Canonical Assets/Scenes/Main.unity generated and registered in Build Settings.
- [x] iOS and Android application identifiers are configured in PlayerSettings.


## Mobile platform build verification - 2026-10-07
- [x] Android IL2CPP development APK builds successfully with Unity 6000.6.4f1.
- [x] Android manifest verified: application ID `com.kamilunavo.risingsteps`, versionName `1.0`, versionCode `1`.
- [x] Unity iOS Xcode export builds successfully.
- [x] Generic iOS device Debug build succeeds in Xcode 27.0 with automatic signing.
- [x] Code signature verified: identifier `com.kamilunavo.risingsteps`, Apple Team `TKG684N5GL`.
- [ ] Store-ready 1024x1024 app icon and final release/archive validation remain release tasks.
- [ ] Local iOS Simulator QA is blocked by the currently installed CoreSimulator runtime mismatch; device builds are not blocked.

##2026-10-08 autonomous completion checkpoint (main e786fc2)
Pure persisted adventure rules, three realms, actual portal and contextual UI implemented. Actual Mac development player passed36 CharacterController landings, three portal completions, saved completion/checkpoint, forced-fall recovery, daily guard, modal pause/stale-input and joystick ownership (51 assertions). Combined editor rules/UI/input/geometry29655 assertions PASS. Generated sky/material art, authored real3D runner/islands/temple, DE/EN and feedback committed. Native/iPhone/Duo/soak/commerce/internal delivery remain open; this is not release acceptance. Evidence task work/rising-mac-qa3/PASS.txt and rising-runtime3.log. Art inspection refinements in progress: preserve smooth vertices, arch back-face and soft particles.

##2026-10-08 native baseline and remaining defects
Native iPhone17Pro/iOS26.5 via Xcode27.1 SDK actually passed36 controller landings, three portal completions with3/3 stars, natural fall recovery and48-point UI/pane checks (77 assertions). This run exposed an immediate-restart startup collision race: the final route had already suffered one uncommanded fall before the natural-fall probe (fallsBefore1). An independent telemetry regression check fails on that trace. Hypothesis/fix under verification: explicitly synchronize newly authored collider transforms before player placement/control. Native orientation settling also needs a condition wait rather than a fixed0.7s capture. No clean native acceptance claim until rerun.
Own unpublished AdMob apps/50Crystals rewarded blocks now created and confirmed: iOS ca-app-pub-8944085355624754~8613336153, reward /8421764469; Android ca-app-pub-8944085355624754~6808290736, reward /7045103615. Test units remain selected for internal builds; proper privacy/consent/native SDK integration and genuine completion still open.

## Native core gate — 2026-10-08 morning
- iPhone17Pro/iOS26.5 simulator core-native-5: RISING_QA_PASS80; actual CharacterController36 landings/three portals, natural fall/checkpoint, persisted completion, modal input/timer, pointer ownership, orientation dimensions settled,48 logical-point controls and synthetic reserved pane.43 saved screenshots inspected including world viewport restricted to usable pane. Simulator does not prove physical performance/haptics or actual Duo inner acceptance.
- Quick native regression: five immediate route restarts PASS without uncommanded falls. Native4 RED showed route-build frame delta .333 s applying2m movement before first input update; ResetMotion now settles controller on the ground before accepting the loading frame as movement. SyncTransforms alone did not fix it.
- Runtime asset lifetime RED showed original island meshes surviving root destruction. ArtLifetime owns generated meshes/instance materials; MeshArt.Batch owns combined buffers; green9067 combined editor assertions. Real30-minute rendering/memory soak remains pending.
- Build5 is QA iteration, not uploaded build number. App Store Connect presently contains no Rising Steps builds.
- Own AdMob unpublished app/reward IDs created: iOS app~8613336153 reward/8421764469; Android app~6808290736 reward/7045103615 (publisher8944085355624754). Internal test units remain enabled. Own consent/privacy URL and genuine native reward/purchase acceptance remain open.
