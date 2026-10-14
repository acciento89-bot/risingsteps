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
- [x] P01-T06 iOS dev build
- [x] P01-T07 Android dev build

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
- [x] P03-T05 reachability/balance runtime pass
- [x] P03-T06 final step pattern library
- [x] P03-T07 portal/realm completion flow
- [x] P03-T08 multi-realm progression

## P04 UI
- [x] P04-T01 Height HUD
- [x] P04-T02 crystal HUD
- [x] P04-T03 Shop button/panel
- [x] P04-T04 Daily button/panel
- [x] P04-T05 Style button/panel
- [x] P04-T06 Daily session claim action
- [x] P04-T07 style preview switch
- [x] P04-T08 settings
- [x] P04-T09 DE/EN
- [x] P04-T10 reduced motion

## P05 Production art
- [x] P05-T01 fantasy palette/material foundation
- [x] P05-T02 procedural floating-island foundation
- [x] P05-T03 final character + animations
- [x] P05-T04 authored island/step kit
- [x] P05-T05 waterfalls/cloud kit
- [x] P05-T06 portal VFX
- [~] P05-T07 optimized lighting/post FX
- [~] P05-T08 screenshot-quality gate

## P06 Audio/haptics
- [~] P06-T01 jump/landing
- [~] P06-T02 perfect/realm cues
- [~] P06-T03 fall/recovery
- [~] P06-T04 haptics
- [~] P06-T05 ambience/music

## P07 Persistence/progression
- [x] P07-T01 versioned profile
- [x] P07-T02 crystals
- [x] P07-T03 styles
- [x] P07-T04 realm unlocks
- [x] P07-T05 migration tests

## P08 Retention/monetization
- [x] P08-T01 persistent daily streak
- [x] P08-T02 daily challenge
- [x] P08-T03 achievements
- [x] P08-T04 cosmetic catalog
- [ ] P08-T05 StoreKit sandbox
- [ ] P08-T06 Play Billing sandbox
- [~] P08-T07 restore/retry

## P09 QA/release
- [x] P09-T01 EditMode tests
- [x] P09-T02 PlayMode climb/fall tests
- [~] P09-T03 iPhone safe-area matrix
- [ ] P09-T04 Android aspect matrix
- [~] P09-T05 30-minute stability
- [ ] P09-T06 performance/thermal
- [~] P09-T07 App Store assets
- [~] P09-T08 Play Store assets
- [x] P09-T09 TestFlight signed archive + internal-only upload (build3; physical acceptance remains open)
- [x] P09-T10 TestFlight processing + internal tester assignment (build3)
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

## Optional commerce integration — 2026-10-08
- Native adapters use Unity IAP5.4.4, GoogleMobileAds11.5.0, EDM1.2.187; actual iOS13.11.0 GMA /3.1.0 UMP export and Xcode workspace simulator build succeeded. Own Info.plist GAD identifier verified plus50 SKAdNetworks.
- CommerceRules/RewardRules23 red→green assertions: starter/replay/restore/persistence rollback, premium revocation/selection, cooldown/five-per-UTC-day and invalidated stale ad loads. Free earned four styles migrate into eight-style profile. Premium cosmetics only; no gameplay advantage, forced ads or mock prices.
- Native SDK QA player retained outside disposable build cache; actual36-step routes passed again and1800-second rendering/memory soak running. Functional QA uses isolated profiles and skips network purchasing, so it does not prove genuine store transactions or completed native ad views. Separate normal SDK startup still pending.
- Apple starter product draft6820413465 / com.kamilunavo.risingsteps.starter saved, Germany1.99EUR,175territories,DE/EN localized. Sky collection draft, actual transaction/review screenshots and live consent remain pending. No AppReview/public submission.
- Latest editor suite9067 PASS with fixture lifecycle assertion removed (direct reflection setup instead of Unity SendMessage outside Play mode).

## Whole-change review and fix pass
Fresh read-only review of9ae3921..429c8cb found no Critical and three Important findings. All reproduced before fixing:
- UTC streak in negative offset: bundled Mono TZ=America/Los_Angeles actual RisingRules RED(1day/200instead2/210), GREEN with AdjustToUniversal.
- Modal notes/gaps could not start scroll: actual HUD fixture viewport raycast RED→GREEN; transparent gameplay root remains non-intercepting.
-375x667point phone: actual HUD fixture52-unit Menu scaled below48points RED→GREEN; control and modal-row minimum sizes use native density/canvas scale,49point floor to absorb pixel rounding. Dynamic header spacing avoids overlap.
Whole editor suite9071 PASS pluscommerce23 PASS; native SDK export8 includes the fixes and explicit scroll/lifecycle/fresh-boot reload probes. Native player validation of this source follows. No re-review: executing-plans requires one fresh review followed by one regression-covered fix pass.
SDK6 baseline actual real-time soak:1800.239s,105846frames, errors0; allocated125786301→126302222bytes, peak126355995; p95frame16.88008ms on simulator/host while other source work ran.30phasecaptures. This is simulator evidence for the SDK6 rendering/memory baseline; it predates the review UI/date fixes and proves neither physical thermal performance nor real commerce.
Ruling: physical-device ergonomics/heat/haptics remain open — host simulation cannot establish them — cost if wrong: internal tester may find device-only issues.
Ruling: real purchase/restore/reward/live-consent acceptance remains open — QA disables network commerce and approved app privacy URL is pending — cost if wrong: catalog/account configuration may still prevent transactions.
Ruling: Duo inner acceptance remains open — synthetic pane is only geometry proof, existing official emulator inner renderer stalled — cost if wrong: a device-specific rendering/input defect may remain.
Ruling: soak is recorded against SDK6 source rather than mislabeled as final UI-fix source — strongest honest evidence, native final targeted regressions follow — cost if wrong: a long-run issue introduced by final small UI changes would require additional soak.
Ruling: catalogs/signing/TestFlight/tester work follows source gate — permitted internal-delivery scope — cost if wrong: external processing can delay installation.
Ruling: public release readiness is not claimed — user only authorizes internal delivery — cost if wrong: later public-release gates require further work.

## Final compact native regression checkpoint — 2026-10-08 10:40 Berlin
- QA8 native iPhone SE3/iOS26.5,375x667 logical points: actual36 controller landings/three portals, lifecycle callbacks, modal text/gap drag, minimum49-point Menu/Close, native orientation all passed. Full harness then failed its left-pane-only assertion; actual UsableRegion correctly selected the fractionally larger right pane. This was a QA assumption defect, not a blocked-area overlap. QA9 permits either non-overlapping usable pane and editor9072+commerce23 passed; native rerun follows.
- Fresh native process with the same isolated compact profile restored step1 and actual controller within.12m of saved island (RISING_RELOAD_PASS); independent normal SDK startup reached the actual production home without abort. Store connection callback warnings were observed; neither real purchase nor rewarded video acceptance is claimed.
- Actual normal home and landscape achievements captures retained. Shop drafts saved: starter6820413465 Germany1.99EUR; skycollection6820419090 Germany2.99EUR; each175territories andDE/EN. Still no public submission, real sandbox transaction or live consent acceptance.
- Source main25b0db7 remote HEAD matched; all Assets/Packages/ProjectSettings inputs tracked. Generated Unity Library and obsolete SDK6/PerfectDrop simulator players removed only after source backup and QA evidence retention. Perfect Drop6 signed IPA/AAB/archive retained.
- NativeQA8Release dSYM output andQA9Debug linker each encountered actual ENOSPC. Simulator retries omit distribution-only dSYMs, retain finished players, and remove generated build intermediates. QA9Debug retry BUILD SUCCEEDED. Device archive symbols remain a separate release validation gate.

## QA9 compact native result
Native iPhone SE3/iOS26.5 completedRISING_QA_PASS90 with9072 editor/23commerce input source. Actual36 steps/three portals, no uncommanded startup fall (independent telemetry verifier), natural fall/checkpoint, modal lifecycle callbacks, description drag, orientations and49-point controls all passed. Both-side exclusion probe passed;43 captures retained in task work/rising-native-compact9. iPad and genuine Duo pose attempt follow. Native callback simulation is explicitly distinct from actual OS background/resume; fresh-process checkpoint restoration separately passed underQA8.
Storage recovery additionally removed only idle Gradle9.0.0/9.3.1 generated transforms (no daemon active), preserving downloaded dependency jars and all source/build evidence.

## Final tablet / official Duo native evidence
- iPadPro11M5/iOS26.5 QA9:RISING_QA_PASS90, native portrait1668x2420/landscape2420x1668, actual36 controller landings/three portals, natural checkpoint recovery, modal description drag and48-point safe controls passed.43 captures inspected/retained; independent startup telemetry verifier PASS.
- Existing official iPhoneDuoC29D1BDB..iOS27.1: bootstatus reported Data Migration Failed but install/launch succeeded on outer1398x2034 display; full90 checks and startup telemetry PASS, actual portrait/landscape outer frames retained.
- Actual innerLCD-1/display3(2007x2853) captures black both initially and after documented simctl screenConfig outer-off/inner-on power probe. This does not establish a genuine unfolded pose or a Unity rendering cause. DeviceHub UI control timed out(-10005), so no fold-pose transition was accessible. Inner acceptance remains open; synthetic pane proof is not substituted. Restored outer power and shut down device. No generic resizing/new fake Duo device used.
- Newly created disposable compactQA simulator deleted only after captures/logs/profile backup; finalQA9 player retained. Completed native simulator export removed as regenerable cache before device Release export.

## Signed iOS internal build1 — 2026-10-08
DeviceRelease export9072 editor/23commerce PASS; ownGADID+50SKAdNetwork entries verified; QA harness absent from generated shipping C++. Xcode27.1 ReleaseARCHIVE SUCCEEDED, proper UnityFramework dSYM retained. LocalAppStoreConnectIPA export succeeded: com.kamilunavo.risingsteps1.0(1),AppleDistributionPiotrKaminski/TKG684N5GL,get-task-allowfalse; decoded temporary copy passed codesign--verify--deep--strict. Cloud-workspace inspection copy acquired FinderInfo xattrs; originalZIP unchanged and temp verification avoids that metadata artifact. IPA SHA25610b99c89edcd0aaf7fb3c6661af9bc502b5961c48965c571427085715e17d5aa retained in taskoutputs; archive/private/tmp/RisingSteps-Release-Build1.xcarchive retained.
CLI upload failed Apple'sActor/relationships/providerId credential mapping; authenticatedXcodeOrganizer TestFlightInternalOnly upload then completed successfully with one vendorUnityRuntime.framework missing-dSYM warning(UUID392D7A7F-6A4F-3A7A-8788-4089FA96FF38). No fabricated symbols; no public or externalTestFlight release. Processing/internal tester assignment follows.

## Android native release1 built and verified
Unity6000.6.4f1 BuildAndroidRelease exited0,9072 editor/23commerce assertions PASS. Actual ARM64/minSDK26 AAB1.0(1) validated by bundletool1.18.2; manifest com.kamilunavo.risingsteps and ownAdMob~6808290736 verified. Native GMA25.4.0/UMP4.0.0 and AndroidX pins reproduced from checked-in portable Gradle templates; generated EDM resolution snapshot recorded. Unsigned AAB SHA2569db1dd73fc06c4323ca52797e58fb061bcdab722729f51ce515f022ab3cba85a; payload hashes recorded before existing central-key signing. No Play upload.
Internal groupKamilunavoIntern ec681c3b-3166-4239-ab1b-7916d257a829 created with manual build assignment, existingPiotr tester added(1tester); Apple build processing remains pending. Own disposable PerfectDropDuo QA clone1B54590C.. removed after own profile backup and source/build/evidence retention; officialDuoC29.. retained.

## Internal delivery and complete source backup — 2026-10-08 11:55 Berlin
- Apple processed build1 UUID7ae64ad6-8e9e-4642-9122-df64b3946f2c; export compliance saved, actual ASC status **Im Test**. Assigned to existing Kamilunavo Intern group ec681c3b-3166-4239-ab1b-7916d257a829 with1 existing tester. No external/public release. Attempted German build notes did not persist (4000 characters remaining); notes are not claimed saved.
- Central Android signing workflow37757979575 succeeded against source7169931e851e1a3de65be44a75aee54c39a338c7 and unsigned asset621368637. Downloaded final AAB1.0(1), SHA25652abaa049ae646150752803886879d3f888f44a32ac251473c0c7123b51914bb; actual jarsigner verification PASS, exact universal certificate SHA2567985BD6B33711BACA7E6BA722C2B3870EBBC802F7DB4A7BC1206BDAE51C4D5D6, payload hashes identical to before signing, correct package and own AdMob identifier. No local key restored and no Google Play upload.
- All Unity Assets, Packages and ProjectSettings including graphics, meta files, native plugins, SDK manifest and portable Android resolver inputs were tracked and pushed on canonical main7169931; remote treec29e076defcbb628632816f3f9f308be93b13308 verified before deleting generated Library. Final IPA/AAB/archive and QA9 player/captures/logs retained outside disposable caches. Full source is reconstructable; generated builds do not replace source backup.
- Genuine sandbox purchase/restore/reward acceptance, approved app privacy/consent URL, physical device/thermal testing and official Duo inner pose remain explicit gates. This internal delivery is not a public release readiness claim.


## Jump camera follow-up, 2026-10-08 evening — desktop verified, native pending

User reported automatic turning while jumping. Fresh baseline Mac player reproduced visible pitch feedback with an unchanged actual joystick: 3.858582 degrees of camera drift during a real CharacterController jump. The stored orbit angles had not changed; LookRotation toward the moving target from a damped camera position changed the rendered movement basis. Production fix keeps the rendered rotation equal to the player's orbit angles while retaining positional damping and manual orbit.

Fresh fixed Mac player camera probe passes 70 checks across forward/diagonal inputs and a deliberately requested 35-degree orbit: measured rendered yaw/pitch drift is zero in all three cases. Actual UI raycasts own joystick/jump, held joystick remains unchanged, actual controller rises. The first full course probe successfully completed all 36 steps/three portals and natural fall recovery but failed its scroll assertion because desktop orientation requests do not resize a window. Corrected the development-only helper to resize desktop windows explicitly; fresh fixed2 build exits0 and course-green3 now PASS88, including actual landscape achievements drag, division layout and restored portrait. No physical touch/native acceptance, alternate-render-rate evidence or new TestFlight delivery claimed. Evidence in delivery workspace work/rising-camera-red1, rising-camera-green1 and rising-camera-course-green3 plus corresponding build/player logs; the failed earlier full runs remain retained.

## Lossless archive storage, 2026-10-08

The final Xcode archive is preserved as `outputs/archives/RisingSteps-Release-Build1.xcarchive.tar.gz` in the delivery workspace. All 61 regular files were compared byte-for-byte with the original before removing the uncompressed temporary archive. SHA-256: `ade02d85d663a28ca3c0a420341b25ec9d130c79ae9f8de630a6b57888b3c464`. The matching proof manifest is beside the archive. Durable owner-only backup: unpublished draft release asset `622084122`; the server digest matches. Restore with tar extraction before opening in Xcode. Original `/private/tmp` archive paths in earlier entries are historical. Unity source, signed binaries and QA evidence remain preserved.

## Native jump camera correction acceptance, 2026-10-08

Source09268cf28b3a33461f5edda6c03db32c2497f1dc, fresh iOS development export and ARM64 native simulator build succeed. Actual official iPhone16ProMax camera probe **70PASS**: fixed real joystick + UI jump callbacks, forward/diagonal/manual yaw35, zero measured rendered yaw/pitch drift across all three18-frame samples. Full course **90PASS**: all36 actual CharacterController advances,3portals, natural fall/checkpoint, real scroll drag, native rotation, reserved pane, lifecycle/pause/input checks. Local work/rising-camera-native-green1 and rising-camera-native-course-green1 retain captures/CSV/PASS; outputs/RisingSteps-Camera-Simulator-QA.app retained and files/symlinks verified before removing generated Derived. The iOS command-line output-path override was ignored; the local harness now archives the actual default RisingQA folder under a unique run ID rather than trusting absent output. Existing TestFlight remains1.0(1); updated release2 pending. No physical touch/thermal/commerce claim.

## Shared commerce recovery correction, 2026-10-09

Regression probes reproduce both missing late-consent presenter gate and missing same-session reconnect gate (RED). Minimal correction follows the already validated OneMoreFloor/Repair adapters: consent update completion presents only in the currently open paused shop and never over an existing purchase; synchronous presenter failure releases UI state. Reconnection retains one controller/event subscription set, blocks overlapping pending async connects after failure callbacks, and allows retry after5s. Shop adds the reconnect action and polls its availability without requiring a new app session. Fresh editor **11 recovery checks PASS** and **23 existing commerce checks PASS**. Local work/rising-commerce-recovery-{red,green}.log retained. Refreshed player/native delivery and actual SDK purchase/reward acceptance pending. Product IDs/economy/save schema unchanged.

Fresh combined desktop player follow-up,2026-10-09: complete36actual CharacterController advances/3portals/natural fall/checkpoint/scroll/rotation/lifecycle checks88PASS. work/rising-camera-commerce-review. No genuine native commerce/physical performance claim.

Normal desktop SDK-disabled shop inspected in portrait: reconnect, restore, optional reward and prepare actions scroll into view above the fixed return button; unavailable state is honest and no prices or entitlements were fabricated.

Shipping follow-up,2026-10-09: fresh1.0(2) device Release archive/internal-only IPA export succeed. Strict/deep Apple Distribution signature/teamTKG684N5GL/get-task-allow=false verified; IPA SHA2561b5d4c17475e70548c185590e498bdbd1b2886f41d3caa9137d2a42b5e9b371f. Production generated code excludes QA. Xcode Organizer TestFlightInternalOnly upload07:02Berlin completed with the known UnityRuntime.framework missing vendor dSYM warning. Apple processing/tester availability still to verify. Archive all61files and symlinks byte-verified, SHA2563c98be8b5a9ec4dfcfec43029ef9af3de494b359a2f2ae5bc05c92e0df828c89; retained outputs/archives/RisingSteps-Camera-Commerce-Build2.xcarchive.tar.gz. Unpublished draft407525647 durable IPA/archive backup server digests verified; only owned generated export/archive/Organizer duplicate removed. Android2 and actual SDK commerce/physical acceptance remain pending.

Android2 follow-up: initial native C++ compile exhausted disk; explicit controlled second start stopped before further exhaustion. Removed only task-created QA simulator629D06A3 after native player hashes/proofs and isolated preferences were retained, plus superseded old native players/duplicate exports. User original simulators untouched. Third unchanged Unity source build exits0 and complete releaseAAB succeeds. Stripping local debug JAR wrapper preserves all574payloads. Pinned bundletool confirms com.kamilunavo.risingsteps/version1.0/code2/own AdMob Android ID/non-debuggable; all6ARM64 ELF PT_LOAD align16KB. Central signing37888664705success; jarsigner/universal SHA1+SHA256cert pins/source provenance/574payloads/manifest verified. Final signed SHA256b50c0b43dbb28082d1e62bf351950e5f693fb8cdeaf13b1c98053a4f33d035b5, durable unpublished draft407525647 finalpackage/proof digests verified. Apple browser session expired, final internal TestFlight2tester availability awaiting owner login. No Play/public release or physical/genuine-commerce acceptance.


## Internal build2 availability,2026-10-09
Restored authenticated App Store Connect. Platform-provided encryption questionnaire saved; existing internal Kamilunavo Intern explicitly assigned through build detail. Build55179b24-e6cc-42ab-97ea-422819fd2131 version1.0(2) now shows Im Test/group Kamilunavo Intern/1invitation. Detail confirms1internaltester and saved German test notes for camera rotation during movement/jump, orientations, checkpoints and reload. Receipt work/rising-testflight-build2-status.json. No public release or App Review submission; physical build2 installation/genuine commerce acceptance remain external.

## Physical touch report response — 2026-10-09, pending coordinated Unity/native gate
User reports camera backwards rotation during movement+jump on physical build1.0(2). Previous rendered-quaternion probes invoked UI handlers directly and never supplied real raw multitouch to OrbitCamera.Read, so their70PASS does not establish physical gesture ownership.

Baseline sourcef573e1ae1510b39287b5a869c272b285a8547ed9. Deterministic standalone input-boundary reproduction compiles the actual legacy OrbitCamera: joystickID11 and jumpID22 remain UI-owned, unintended free-view fingerID33 begins then sweeps200pixels left. RED: stored yaw−30degrees while joystick/jump remain held. This confirms that ambient touches can rotate the camera; it does not prove which exact finger the physical report involved or an EventSystem callback-order defect. Fixture: tools/regressions/TouchCameraProbe.cs. No Unity process/device launched in this source pass.

The directly requested control correction supersedes free-screen swipe orbit: touch orbit now requires an explicit View-pad pointer down. UI owns that pointer through drag/up; other movement/jump fingers cannot capture, steal or release it. View deltas use logical points rather than device pixels to avoid Retina3x sensitivity. Right-mouse orbit remains. New routes reset to forward yaw once; movement never changes player-owned yaw per frame. Modal/focus input reset releases the View pointer. Actual current Camera+View source passes standalone ambient, ownership, foreign-release, own-release,3x sensitivity, pause reset and forward reset checks.

[~] Home replaced with a cream/cyan sky atlas, illustrated grass/waterfall/temple realm previews showing actual stars/unlocks, a fixed gold Continue/Start action, and grouped secondary adventure tiles. All contextual pages share Rising's bright illustrated identity.
[~] Playable tutorial: actual joystick movement≥0.25m, actual accepted grounded Jumped event, then the next ordered landing. Skip/replay are available; normal replay preserves the checkpoint and crystals. At a finished route, explicitly replaying the tutorial starts another ordinary run in that realm. TutorialDone is an additive field in schema1; old progressed saves infer done without changing currency/styles/entitlements. Fresh/skip/completed states remain persistent. No bonus/economy/provider change.

Static fallback compile of all runtime code and both validation classes succeeds against installed Unity API/UI assemblies with only the two native commerce presenter boundaries stubbed. Existing obsolete FindFirstObjectByType warnings and stub unused-event warnings only. This is syntax/API evidence, not a Unity import/build or real SDK validation. git diff --check passes.

Queued full gate: RisingValidation.ValidateAll includes RisingTouchTutorialValidation.Validate(9ownership/migration assertions); BuildAutomation.BuildMacPreview performs the same suite before building. RISING_QA=1/RISING_QA_CAMERA=1 adds real EventSystem raycast/ExecuteEvents ownership gestures before the existing jump/rendered camera probe. RISING_QA=1/RISING_QA_TUTORIAL=1 on a fresh isolated RISING_QA_ID captures home, all three tutorial states, completion, replay, progress home, scrolled actions, and style. Ordinary RISING_QA exercises36controller landings/3portals/modal/fall/orientation/48point View controls. Physical two-thumb swipe/jump+third-finger testing and visual inspection on compact portrait/landscape remain required.

Coordinated Unity import gate exited0 but logged repeated MissingComponentException for RealmArt. This invalidates that apparent gate pass. Custom UI graphics now create CanvasRenderer before Graphic.OnEnable, including the existing icon/panel/text/joystick/jump factories. ValidateUI asserts renderer ownership for realm art and icons; ValidateAll captures unexpected Error/Exception/Assert logs and throws after canvas flushing. Tutorial assertion log count corrected from10to the actual9. Root rerun queued; no build acceptance claimed.

Native compact tutorial PASS11 and ownership/editor9076+9 established before the gallery revision. Visual review of actual home-progress.png found overlapping hero text and repeated flat diagrams, so those screenshots are not a premium art acceptance. Revision uses original authored Resources sky panorama/icon as distinct aspect-preserving meadow/waterfall/temple crops. Hero text ends at53%, image starts57%; gallery shader replaces fat gold pill edges with10unit corners/subtle edge. Added6gallery geometry/resource/style assertions. Shader import/build/native visual rerun queued with root; no build run by source owner.

Compact native camera ownership gestures pass, but old rendered-camera fixture fails its first joystick raycast. Source trace: forward case(0,1) created its initial pointer exactly at joystick rect.yMax, a boundary excluded by native rectangle hit testing. That QA path never starts the tutorial (Close/StartRun only); no tutorial skip was added. Corrected fixture now raycasts an interior0.75radius Began, executes actual UI pointer-down, then owner-drags to the original full-radius position before jump. Native diagnostic logs down/boundary positions, screen rect, containment, actual hit names and tutorialActive. Added16geometry assertions across normal,compact,landscape,narrow-pane tutorial bounds and interior-before-edge touch fixture. Physical input behavior/production controls unchanged. Root native rerun pending.

Gallery Metal compiler RED: `unexpected token point` at shader34/36, followed by cascading abs-intrinsic error; build's shader gate correctly rejects the emitted app. `point` is a reserved HLSL primitive keyword. Renamed the local distance-field coordinate to `localPoint`, matching the existing validated Panel shader. No other shader behavior changed. Root warm rebuild/Metal verification pending.


## Coordinated camera ownership, sky atlas and guide acceptance — 2026-10-09
The actual Unity SDK/editor gate passes9098 review regression checks,9 touch/tutorial and23 commerce checks. Metal gallery shader first failed because HLSL reserves the local name point; renamed it localPoint and reran the actual build successfully. Fresh standalone rendered tutorial/home passes11, ownership/camera passes78, full actual CharacterController course passes96 including36 steps, three portals, natural fall/checkpoint, actual scroll/orientation/reserved pane and lifecycle inputs. Reviewed home-progress screenshot shows aspect-preserving authored scenic thumbnails with separate hero text/image regions and a restrained cream sky atlas.
The native compact tutorial also passed11 before the final gallery art update. The native ownership probe initially reached a legacy CameraFeedback failure at the exact joystick rectangle edge. This was a QA hit-test fixture defect, corrected by pointer-down inside the joystick followed by owned drag to the original full-radius position; actual production input ownership is not suppressed. The final corrected camera78 and course96 are desktop callbacks, not physical raw-touch/SDK acceptance. Native final gallery/camera rerun and TestFlight build3 remain pending. Movement and jump cannot acquire camera orbit; dedicated view pad owns its pointer and releases on modal/focus. Both build numbers prepared3, saved progress retained.

## Presentation build 3 internal delivery — 2026-10-09
- Final source cb28e8b327191dce65722f280b656d9a3cdf9990: dedicated BLICK pointer ownership, scenic island atlas and observed-action tutorial. Actual Mac runtime gates: tutorial11, camera78 and course96 assertions; 36 real controller landings and all three portals. Physical simultaneous two-thumb input still requires acceptance on the user's iPhone.
- Unity iOS export and Release archive succeeded. Distribution IPA SHA256 751d910af69f32372bf378d4c60c4bc85564e4dbabb70e922445d98ce638a85c verified deep signature/team/build and excluded native QA callbacks. Archive byte verification retained61 files. IPA, compressed archive, proofs and Organizer upload metadata are backed up in the unpublished draft risingsteps-build3-presentation-tutorial.
- App Store Connect observed build c8541710-6775-41a8-8ace-2fe5b4bb95fa / 1.0(3), Im Test, existing Kamilunavo Intern group,1 invitation. German device test notes saved. Known UnityRuntime vendor dSYM upload warning does not prevent the successful internal upload. No public submission.
- Android3 release compilation succeeded; signed AAB SHA256 690112d57264234f728f6410d0a7308436b536eb6bd0f4f282b35b3b57b18976. Existing central certificate pins and575 unchanged payload entries verified. Signing run37976344832 passed on retry after transient Google INTERNAL500, with no credential or access changes. Signed bundle, provenance, certificate and local proof backed up in the same draft; server asset digests verified. No Play upload.
- Generated native exports, derived compilation and Library caches removed only after retained verified binaries and durable backups. Physical performance, real ad reward and sandbox purchase/restore are not proven by these harnesses.

## Second physical camera report and distinct realms — 2026-10-09
The user narrowed the defect to holding movement before pressing Jump. The previous UI-only camera tests missed Unity's default two-touch right-mouse synthesis. Actual OrbitCamera source in the deterministic input-boundary probe reproduced RED yaw−30degrees. The desktop mouse-orbit branch now excludes mobile platforms and any active touch input. Dedicated BLICK pointer control remains available, mouse orbit still works on a desktop without touches, and player-owned yaw is never reset per frame.
The source probe GREEN covers movement-before-Jump, reversed finger order, synthesized mouse tail on mobile, desktop touch synthesis, genuine desktop right mouse, ambient ownership, explicit View capture/release and logical-point sensitivity. Actual Unity Mac build passes9112 core/art/review plus9touch/tutorial and23commerce assertions. Fresh actual player camera147 checks cover held movement for5frames before Jump and Jump-before-movement through real UI raycasts/callbacks with rendered quaternion tracking. This is not a physical raw-touch replay.
Selected worlds previously rebuilt only the route; distant islands always used realm0. The owned realm scenery now rebuilds with the selected theme, with geometry/material distinctions rather than only menu images. Fresh full course111PASS includes36 real CharacterController landings, three portals, a natural fall/checkpoint, cloud-root replacement, unique realm architecture, orientations/scroll/reserved pane. Captures realm0/1/2-start.png were inspected: green meadow, cool pools/falls, warm sandstone columns/ruins. Work evidence: root work/rising-revision4-* and rising-camera-move-before-jump-red4/green4.log. Internal build4/native export/upload and physical touch acceptance remain pending; older TestFlight3 does not contain these corrections.


## Verified internal presentation revision — 2026-10-09
Fresh native **1.0(4)** delivery from source `5efc93b13d7f4c7d7673502714a0e2027b302875`. iOS Release archive/export succeeded, strict/deep Apple Distribution signature and teamTKG684N5GL verified, get-task-allow=false, QA runner excluded. IPA SHA256 `e512c8fd43ade9b3ca8ec210285b83b7fbc33b2db1437829bb02905f3498dfa9`. Compressed archive all61files byte-verified, SHA256 `e147c43186db05d5b074907e09cff91a29340c9bd82b73f1bce83300a1062afe`; full archive and upload metadata have verified durable digests in unpublished draft `risingsteps-build4-presentation-tutorial`. Xcode GUI InternalOnly upload completed with the known vendor UnityRuntime.framework dSYM warning; own symbols preserved. Authenticated ASC UI confirms **Im Test**, existing Kamilunavo Intern group,1tester and saved DE test notes, build ID `e4862a1e-0354-4c1d-bcc7-988f283d3b3d`.
Android manifest/version/own AdMob and all6ARM64 ELF16KB alignments validated. Central signing run37987103423success; both existing universal certificate pins match, all575non-signature payload entries and manifest byte-identical. Final signed AAB SHA256 `af040cd5ae3db9ea8b61d791356809ea406c2647037e86f408abb631f1e2d512`; final package/provenance and proof asset digests freshly verified on durable unpublished draft.
Native compile inputs were retained after disk exhaustion; identical generated Gradle inputs successfully packaged after removing only owned regenerable compile caches. OneMoreFloor additionally retried native generation after a debug-symbol copy exhausted disk; RepairEmpire retried an unchanged Xcode export after its linker exhausted disk. These environmental failures were not counted as passing runs.
All nine coordinated runtime acceptance receipts are retained locally at `/Users/piotrkaminski/Developer/KamilunavoDelivery-20261009/work/runtime-acceptance-manifest.json`; actual rendered screenshot checks and DE/EN portrait/landscape flows are documented above. Full Unity sources are on canonical main. Owned desktop/simulator QA output bundles, Library/export/DerivedData and duplicate uploaded archives removed only after newer acceptance and retained verified artifacts; original user simulator devices, player profiles, receipts and keys untouched.
No public App Review submission or Play publication. Real physical iPhone multi-finger input/performance and genuine provider purchase/reward acceptance remain distinct device gates. These builds implement specific tested graphics/UI improvements; they do not establish full cinematic equivalence to concept stills. Delivery record: `docs/INTERNAL-PRESENTATION-REVISION.json`.

## Approved distinct-world source revision — 2026-10-10, coordinated gates pending

The approved October 10 three-view board drives a real-world geometry revision while retaining the cream/green illustrated atlas. Water and temple no longer copy meadow grass/daisies/trees; meadow no longer copies falls. Water receives pale terraced limestone, physical turquoise perimeter pools, reeds/lily pads/blossoms and layered curtain falls. Temple receives dry ochre paving joints, carved friezes, columns/stairs, purple banners and broken ruin lintels. The reachable destination becomes thick segmented masonry with a golden keystone/cornice. Realm-specific distant layouts and gradient blue/cool/lavender skies replace the gameplay panorama; the painted atlas remains menu art.

The authoritative course landing fan/collider, CoursePatterns, motor, ordered progression, save schema/economy, free route and IAP are unchanged. Existing original/batched mesh ownership and owned sky/cloud material destruction are extended by the art probes. New geometry remains bounded to under30,000vertices per tested route island including the portal; cached atlas materials and static batching remain the mobile rendering approach. This is a source budget constraint, not measured device performance.

Tests-first coordinated editor RED exits1 at `Concept world: gameplay sky contains no painted course background`; the new validation inspected the actual baseline sky rather than a mocked renderer. Evidence: root delivery `RisingSteps/concept-red.log`. `RisingConceptValidation.Validate` now checks actual distinct flora/architecture, original collider plane/count, zero scenic colliders, normals/vertex budget, segmented arch, original+batched mesh destruction and world sky/cloud material destruction; integrated into `RisingValidation.ValidateAll`. Runtime QA additionally requires genuine terrace/reed/lily and banner/ruin/stair geometry and rejects copied flora/falls/backgrounds.

The actual-source standalone camera boundary compiled and passed movement-before-jump synthesized right mouse, reversed touch order, mobile mouse tail, desktop touchscreen, genuine desktop orbit, ambient third-finger exclusion, explicit View ownership/foreign release, logical-point sensitivity, own release and reset. Production camera/input code was not changed. `git diff --check` passes. No Unity/native jobs, commit/version bump, external assets/provider or release action were performed by this source worker. Original primary-concept image deletion is preserved.

Root acceptance entrypoints: `RisingValidation.ValidateAll` (includes concept checks), `CommerceValidation.Validate`, `Kamilunavo.RisingSteps.Editor.CommerceRecoveryValidation.Validate`, and `BuildAutomation.BuildMacPreview`. Fresh isolated `RISING_QA=1` course run exercises36actual controller advances/three portals/fall/lifecycle/scenery replacement/orientation/48point targets. `RISING_QA_CAMERA=1` retains held-movement-before-Jump rendered ownership tests; `RISING_QA_TUTORIAL=1` captures actual atlas/tutorial flows. Actual compact DE/EN portrait/landscape screenshots must be compared with the board. Physical raw-touch/performance/thermal and genuine commerce acceptance remain separate gates. GREEN/editor/rendered acceptance is pending the coordinated root worker.

## Focused organic cliff finish — 2026-10-10, rendered gate pending

Actual polished captures still showed centered cone-shaped smooth cliffs and repeated low-resolution polygon outlines. IslandArt now constructs independently seeded offset lower rock strata with coherent angular radius variation and uneven ledges; each face owns vertices for physical faceted lighting. Meadow, cascade and temple retain distinct strata profiles. The two upper lip rings preserve their exact prior radius/elevation, and the original radius generation, ground fan/collider and vegetation random stream are unchanged. No course, controller/input, world dressing, camera, save or commerce source changed in this pass.

One shared 256×256 mipmapped RGB24 stone texture replaces the 128-pixel Voronoi outlines with low-contrast seamless mineral grain and sparse faint open fractures. This finite shared kit uses 65,536 texels (approximately 256 KiB including mipmaps); no per-island texture allocation or imported asset. Surface colors/gloss remain under the existing material kit.

Focused editor fixtures assert exact top rings, asymmetric lower mass, uneven strata heights, varied normals, finite vertices, fewer than2,048cliff vertices, zero rock colliders and the updated 65,536texel shared-texture ceiling. An isolated numeric harness executes the actual extracted BuildCliff source across all39route islands (13per realm): PASS deterministic geometry, exact lip coordinates, nondegenerate facets, asymmetric lower mass (minimum0.04358units) and at least120distinct lower vertex heights. This is a managed geometry probe, not Unity rendered acceptance. Whole actual runtime and editor source compile against installed UnitySDK references: exit0, existing obsolete API/preprocessor warnings only. Whitespace check passes. Root owns RisingValidation.ValidateAll, fresh build and actual before/after capture inspection.

## Coordinated engine and rendered evidence — 2026-10-10

- [x] Actual Unity editor final concept validation passes 30,504 checks; `mac-organic-final-build.log` exits 0 and produces the actual Mac development player.
- [x] Final `runtime-organic-final/PASS.txt`: 125 checks, actual CharacterController lands all 36 islands across meadow, cascade and temple, reaches three portals, saves progress and recovers from a natural fall. World meshes, clouds and owned materials replace cleanly.
- [x] Inspected final `realm0-start`, `realm1-start`, `realm2-start` captures: meadow grass/daisies/tree; pale stepped mineral pools/reeds/falls; dry ochre ruins/banner/columns. Asymmetric faceted lower cliffs replace centered cone strata; original two lip rings and landing collision remain unchanged. One shared 256x256 mipmapped stone texture replaces tiny outlined stone tiles.
- [x] Actual camera input receipt `runtime-camera-final/PASS.txt`: 147 checks; six rendered move-before-jump and reverse-order scenarios, maximum measured yaw/pitch drift both 0. The final cliff/material-only pass does not modify that input/camera source.
- [x] Actual fresh first-play tutorial/replay/skip/save receipt `runtime-organic-tutorial/PASS.txt`: 11 checks; inspected atlas and tutorial captures.
- [x] Separate engine `CommerceRecoveryValidation.Validate`: 11 checks in `commerce-recovery-final.log`.
- [x] Independent source review confirmed original collider/RNG/lifetime bounds. A review finding about Mathf.SmoothStep threshold semantics was corrected before the final engine build. Original user deletion of the primary concept image remains unstaged.
- [ ] New native release builds, physical raw multi-touch/thermal/performance and genuine commerce remain separate. Existing build 4 TestFlight status belongs to the earlier source, not this revision.

All receipts and actual screenshot files are retained under `/Volumes/SSK SSD/Kamilunavo/Builds/VisualRevision-20261010/RisingSteps`. Functional scripted PointerEventData and native game callbacks establish game-side behavior, not OS touch delivery or physical-device FPS. The approved concept remains the visual direction; the captures are actual Unity output and do not claim photorealistic equivalence to an illustration.


## Verified internal concept-polish delivery — 2026-10-10

Native **1.0(5)** is built from source `fbc08fb1cccb9b564fad7ca43e83e22717a2eef1`. Fresh iOS Release archive/export, strict Apple Distribution signature, team TKG684N5GL, get-task-allow=false, ARM64 app/framework and excluded QA runner were verified. IPA SHA256 `8c9079135e11df589c7c36ca3e3c3945e22f2cdf8af9baf9d9cd00db7a44e8a4`. The compressed archive was verified byte-for-byte across 61 files and retained on the APFS SSD, with a server-digest-verified private draft backup `risingsteps-build5-concept-polish`. Xcode InternalOnly upload completed. Authenticated App Store Connect UI confirms **Im Test**, the existing **Kamilunavo Intern** group with one tester and saved German test instructions. New build ID `800cea11-6538-4bdf-aa41-3b3e9fd59bfd`.

Android native export and bundle validation passed with the expected manifest, version, own AdMob ID and six ARM64 libraries aligned for 16 KiB pages. The existing central-signing workflow [38075453004](https://github.com/acciento89-bot/risingsteps/actions/runs/38075453004) succeeded from the exact source commit; the original universal certificate SHA1/SHA256 pins match and all 575 non-signature bundle entries are byte-identical. Signed AAB SHA256 `744fd22e524bb6e101ab1b971e60add7c42b7df2290ca7dd476e9698a6996c71`. Signed package/provenance/proofs and desktop evidence are retained on the SSD and in the same private draft, with fresh server asset digests checked. No local private keystore was created or downloaded.

The coordinated desktop acceptance receipts and actual rendered views are recorded in `docs/INTERNAL-PRESENTATION-REVISION.json`. [Three actual Unity views](QA/2026-10-10/actual-game-views.png) show the present implementation, not an illustrated concept or an iPhone capture. All Unity sources are backed up on canonical main; the original user-deleted historical `docs/concepts/PRIMARY-CONCEPT.png` remains unstaged. Native caches/exports/archives are retained now that the SSD has sufficient capacity.

Acceptance remains bounded: desktop event-driven input and rendered QA do not establish physical iPhone raw multi-touch, thermal or 60 FPS acceptance; genuine provider purchase/reward transactions and full cinematic equivalence to the concept illustrations remain open. The known vendor UnityRuntime.framework dSYM warning does not prevent internal delivery; the game's own and UnityFramework symbols remain retained. No public App Review submission or Play publication was performed.

Specific camera limit: build 5 retains the camera/input code already in native build 4. Six real UI PointerEventData scenarios, including holding movement before pressing jump and the reverse, showed zero rendered yaw/pitch drift. This does **not** reproduce or resolve the user-reported raw-touch behavior on the physical iPhone; the saved TestFlight instructions explicitly request that check. The final material/cliff polish leaves this camera code unchanged.


## Review revisions and original music — 2026-10-11; TestFlight blocked

Four different original musical scores now ship as bounded imported loops with an independent Music choice and master Sound mute. Fresh local Unity aggregate/Mac build exit 0 includes 42 music checks and zero music validation errors. Detailed provenance, current limitations and evidence: `docs/MUSIC-DESIGN-20261011.md`. Intended music source gain .22, .35-second fade, separate focus/pause gates, one owned voice; no third-party music or samples. Physical audible mix acceptance remains pending.

Campaign source now has 10 distinct realms ×10 independently named/reachable courses, stable original30 score slots, schema1/2→3 migration and earned Temple10→Jungle reconciliation without replay/payment. Full editor checks cover100 paths/independent stars/unlocks, Collection2190, real scene thumbnails1043, ten distinct landmarks/horizons and ownership budgets. Actual ten-world player completed81 checks/60 physical controller jumps. The earlier454-check/360-jump/30-portal candidate does not validate all100 new courses: complete1200-jump/100-portal runtime traversal remains open. New actual realm renders show meaningful differences but still fall short of the approved high-detail concept.

All current visual release reports remain failed/pending. Passing tests, mesh counts or a local build is not graphical acceptance. Do not upload any new TestFlight build until exact-source actual Unity renders satisfy the user’s concept gate. The user-deleted historical docs/concepts/PRIMARY-CONCEPT.png remains unstaged.


## Full earned 100-course traversal — 2026-10-11

The retained RisingHundredMusicPreview actual Unity Mac player (pre-new-commerce source main89cb4f0) exited0 after1504 checks: all10realms×10levels,1200 real CharacterController landings and100 physical portal completions, earned unlocks, independent scores, parent Back/pause, schema3save reload. No fixture realm-unlock override in this campaign runner. Evidence: Builds/RisingExpansion-20261011/RisingHundredEarnedRuntime/PASS.txt and rising-hundred-earned-runtime.log. This supersedes the former NotRun100-course gap for that candidate; the newly approved advertising/Shop changes need fresh coordinated regression. All functional acceptance remains distinct from graphical concept acceptance and physical iPhone touch/performance. No new TestFlight upload.

## Approved Shop and completion ads — 2026-10-11

New source adds a visible direct Shop, permanent separate Remove Ads nonconsumable bit4 (Germany EUR1.99 requested catalog target; actual store metadata only), premium purchase and Restore before long lists. Forced interstitials are considered only at safe genuinely successful result boundaries after at least3 successful completions and at least180 seconds since session start/last actual fullscreen ad. Tutorials, driving/repair/active play, background, other native providers and unavailable ads are excluded; no deferred show. Voluntary rewarded videos remain optional and delay subsequent forced advertising. Purchase persistence/idempotency/partial-catalog revocation protection and parent Back are preserved. QA disables native SDK requests including Store reconnect/purchase/restore; privacy invalidates cached forced ads and generations.

New engine fixture: `RisingInterstitialValidation.Validate`. Independent source review findings were corrected before fresh root validation. Exact final validation/build/runtime receipt lives under `/Volumes/SSK SSD/Kamilunavo/Builds/RisingExpansion-20261011/Commerce-verified-evidence.json`; it distinguishes engine/player success from unrun real-device/store/graphics gates. Catalog creation/price, live units, actual device sandbox purchase/restore/refund and UMP/SDK lifecycle remain pending. All4 visual concept gates remain FAILED; no TestFlight upload or version increment.


## 2026-10-11 — actual Concept comparison and graphics refinement

Richer cliff shoulders, caustic pools, hanging moss, Temple masonry, visible layered horizons and owned runner cloth/hair refinements.

Mac build exit0; graphics326802 (maxIsland10803/maxHorizon42912); fresh native81 checks/60 actual controller jumps across10 realms exit0. The100-level earned traversal was not rerun for this art-only slice.

Sparse vegetation, washed cliff surfaces, empty sky composition and simplified runner anatomy still fall short of the Concept. **Visual gate stays FAILED; no TestFlight.** Exact proof/capture paths and remaining gaps: `docs/GRAPHICS-COMPARISON-20261011.md`.


## 2026-10-11 — original authored asset pass and actual Concept evidence

Original tailored tunic/hood, leather backpack/hardware, boot tongues/laces and swept hair; irregular upper mineral shoulders, folded waterfall ferns/vines and Temple herbs; owned linear mineral normal with explicit RGB Standard surface decoder. Original imagegen limestone albedo (unmodified source copied to project,256square repeat/readable importer); owned normals derive from that same surface.

Final Mac aggregate/build exit0; graphics399217/maxIsland16660/maxHorizon42912; runner5403 vertices/9004 triangles and31 default/32 premium batches; actual Mac81 checks/60controller jumps across10worlds exit0. No full100-course rerun or physical mobile performance claim.

Sparse vegetation, empty sky composition, repeated circular island tops and simplified actor anatomy/hair remain below the approved board; the original limestone albedo improves the former crumpled white surface. **Visual acceptance stays FAILED. No TestFlight or version bump.** Exact proof/scene-source digests: `docs/GRAPHICS-COMPARISON-20261011.md`; provenance: `docs/ORIGINAL-ASSETS-20261011.md`. User-deleted historical Concept PNG remains unstaged.


## 2026-10-14 — original assets and coordinated native graphics pass

Public Drop12 and Floor6 are submitted with all3IAPs each and verified WAITING_FOR_REVIEW; user authorizes next Rising work aiming approximate90–95% and own assets. One game at a time. Rising now has5new original bitmap assets, authored alpha-cut layered crown geometry, denser folded leaves/flowers/vines, layered horizon placement, quiet original stone normal, forest-green runner/rear-lock refinement, Cinzel menu titles/gold shader finish and original running/eye/pause glyphs. All live input/navigation callbacks, course centers/colliders, economy/profile/store/version unchanged.

Final exact source db6ca5e1de7c8535a1a432a164aca8e1e198f9d03e3eea7734ebe8c75b7d50d3; actual Editor aggregate/Mac build exit0, graphics548793checks/maxRoute29219/maxHorizon67205, runner5403vertices/9004triangles/31batches. Strict material/texture/geometry limits retained; texture-owner and readable/shader-property failures preserved then corrected. Final actual10worlds81checks/60controller jumps, camera147 and tutorial11 all pass; independent read-only review found no important issue. No physical-device/provider or newfull100traversal claim. Detailed source-bound receipts/provenance and actual comparison docs/GRAPHICS-COMPARISON-20261014.md.

**Visual gate stays FAILED**: current garden density, uniform round caps, faceted cliffs and simplified actor/atmosphere remain below the requested Concept. No numerical90% claim, no TestFlight/version bump. Original user Concept deletion remains unstaged.


## 2026-10-14 — actual landform and luminous portal rebuild

Current source8375be601bffb831ee36b0f000e3241b36382918a1f1769e8ceb8ab7dc8f288e expands Water into broad mineral shelves and Temple into rounded-square caps. Every original seeded boundary remains contained by the same new rendered/collision triangle fan; course centers/elevation/profile/economy remain. This intentionally replaces the former exact-old-coordinate art restriction, with independent legacy-triangle containment and rectangular-area acceptance. Broad lower rock bodies, aligned perimeter greenery/masonry, square Temple inlay, arched emissive membrane and one owned warm unshadowed point light replace repeated pointed disks/crossed portal sticks. The existing nine clouds are layered behind the route; a real Temple sun mesh adds no collider/light.

Actual pre-change contour RED is retained. Final Mac Editor aggregate/build exit0 includes new RISING_WORLD_REBUILD_PASS; old5texture/12palette/30kroute/70khorizon limits stay. Native Worlds81/60landings, earned Campaign1504/1200real controller landings/100portals/all100independent scores reloaded, camera147 and tutorial11 all pass on this exact frozen source. Read-only review found a pre-jump capture latency risk; input clears during capture and re-drives before jump, then final source was rebuilt/reviewed. No current important issue. Exact proof docs/QA/rising-landforms-portal-20261014.json; actual unpainted compare Artifacts/RisingLandmarkComparison-20261014. No physical hardware/FPS/provider or numerical90% claim.

**Overall visual acceptance remains FAILED**, particularly simplified actor, sparse full garden/water composition, angular detailing and material/atmospheric depth. No TestFlight or version increment. User Concept deletion remains unstaged.


## 2026-10-14 — original hair and character surfacing

Source b57ffa0a54d53e8617dc95adc7f203ebf0225b353d8ce42e55f5757727cc7e09: original HairFlow-v1, six broader rear locks,24side face/16side neck, matte warmer skin and dark hiking soles/laces. Skeleton/controller/collider and style bindings unchanged. Denser existing cutout crown planes and less washed mineral tint; sun moves toward portrait framing (actual sun visibility remains insufficient in current mid-route captures). Actual Editor build/aggregate pass; avatar5429vertices/9092triangles/31renderers/8materials/3textures, each texture stays within64square ceiling. Initial256import budget failure retained and corrected without relaxing the assertion. Actual tenworlds81checks/60landings, camera147, tutorial11 and read-only review pass. Old exact-source100course/1200jump proof stays bound to8375; unchanged geometry/behavior SHA files verified, no blanket claim of1200jumps on later source. Provenance and checks QA/rising-character-surfacing-20261014.json. Overall visual acceptance still failed, no upload.


## 2026-10-14 — native illustrated atlas menu

Exact source 30f3089ee8590ad926a569657177e7280165dfd6128ac058d97d5016e830412a. Original full-resolution AtlasHero-v3 has reserved quiet left wordmark space; live Cinzel stacked title/caption, small selection tab, wide rectangular10chapter cards, own dry Sunset TempleChapter-v2 and fixed visibleShop/goldContinue. No letter baked into the art and no menu painting presented as gameplay. NPOT nearest-resizing and narrow-caption clipping caught by independent review; all chapter/atlas imports now preserve aspect, narrow hero retains3to2 art plus paper caption band. Narrow card copy is concise without changing campaign IDs or selector/parent-back callbacks. Exact generatedpixel text-height checks avoid duplicated Canvas font scaling. Actual Editor/Mac aggregate/build exit0, navigation180/gallery2454, nativeWorlds81/60landings/camera147/tutorial11, zero captured runtimeexceptions; rereview no important issue. Same six geometry/behavior hashes as earned8375 campaign; no1200jump claim on this new source. Native comparison Artifacts/RisingAtlasComparison-20261014; originalprompts/hashes QA/rising-atlas-original-assets-20261014.json, tests QA/rising-atlas-menu-20261014.json. Overall graphics gate failed; no upload/version change.


## 2026-10-14 — beveled stone and original water finish

Exact source 758b763f5454f348ca1d893c516011c0b72138f7de13cde85cf50693e8b352cb: actual closed24vertex/44triangle beveled Temple masonry replaces raw perimeter cubes, independently checked positive winding/closed shared edges/unchanged bounding envelope/below landing plane. Original shoulder/outcrop normals softened; two real emissive braziers at checkpoint4/8/12 stand beyond the full fan, no lights/colliders. New original PoolCaustics-v1 texture replaces the existing slot via owned256square RGB24 copy, all previous five-texture/12material/30kroute/70khorizon ceilings held. Serialized unlit SolarSurface shader prevents fog washing out the real sun mesh, but framing is still occluded in actual approach capture and not accepted as done. Atlas caption paper improves contrast with no raycast. Pre-change raw cube-corner RED captured; actual Editor aggregate/buildexit0, graphics548169checks, nativeWorlds81/60landings/camera147/tutorial11; focused rereview found no material issue. All six campaign geometry/gameplay dependency hashes still match earned8375source; later exact-source1200landings not claimed. QA/rising-stone-water-20261014.json; own prompts/hashes QA/rising-pool-original-20261014.json; actual unpainted comparison Artifacts/RisingStoneWaterComparison-20261014. Overall visual release remains failed; no upload/version bump.

## 2026-10-14 — sculpted depth, world HUD and owned daylight rendering

Exact final source00d9dd0ab881b7e0f1a7adc2523837a56dcf06374340d06d2a94dbc8d2b2b354: original unchanged StratifiedLimestone-v3 albedo and derived linear normal response replace the existing mineral slots. Seven primary horizon masses use bounded20ring/32side eroded rounded-square walls; unequal fracture faces and beveled sediment beds replace repeated narrow round shoulders. A real bank/pool/fall and one relocated existing willow replace three tiny farthest shrubs, maintaining69398horizon vertices below the unchanged70k ceiling. The first bank placement was outside the portrait frame; the inspected final placement is partly visible at the left edge. Its entire actual mesh clears the protected central corridor and adds no collider. Lower cloud layers retain the same nine cards. Compact non-raycast localized world badge, real pause/height/wallet/input controls pass DE/EN portrait/landscape/narrow no-overlap and48point checks. The actor has bounded face detail, joined UV normals with opposing cloth faces protected, shorter uneven rear tufts,5562vertices/9346triangles/31renderers/8materials/3textures≤64square; no joint/controller/style identity changes.

Existing camera Bloom incorrectly emitted HDR values above1: an actual float GPU RED reproduced clipped highlights. The owned camera path now performs current Gamma-aware linear bloom, bounded filmic daylight tone mapping, restrained warmth/saturation and eight-neighbor depth-normal contact shade, with unsupported-depth early exit and protected temporary targets. No new render pipeline or post-processing package. A second actual GPU RED caught grey compressed daylight before the filmic correction. Final real GPU linear dark/middle/lit and warm-HDR plus current Gamma-neutral fixtures pass; they do not establish AO orientation or hardware performance. One existing sun uses Medium32m/two-cascade soft shadows; Overlay UI is unaffected.

Actual final Editor aggregate/buildexit0, graphics559643checks, Worlds81/60 real controller landings in10worlds, camera147 and tutorial11 all pass without captured runtime exceptions. Intermediate frozen native proofs remain separately recorded in QA/rising-depth-light-stages-20261014.json. A retained earlier daylight run failed because actual focus loss paused the motor mid-jump; production lifecycle behavior was not bypassed, and fresh native runs passed after one initial Mac window activation. Five game/portal files and the full IslandArt authority/build prefix still byte-match earned875116a; only its render-only Waterfall foam suffix differs. Previous1200landing/100portal earned acceptance stays bound to8375source, not reassigned to this art candidate. QA/rising-depth-strata-light-20261014.json and original source provenance QA/rising-stratified-original-20261014.json; unpainted actual comparison Artifacts/RisingStrataFramingComparison-20261014. Overall Concept likeness remains FAILED: repeated simplified architecture, sparse foliage, character/hair fidelity and scene composition still need work. No TestFlight upload/version bump; no physical FPS/thermal, real-ad or commerce transaction claim.


## 2026-10-14 — seven original chapter illustrations

User specifically rejects seven realms without their own pictures. Created an original2column/4row3:4illustration atlas: Jungle, Crystal, Frost, Desert, Volcano, CloudCoast and Starlit each have an independent3:2subject; eighth bonus garden unused. Full generated1086x1448PNG is copied byte-for-byte, imported768x1024with original aspect; original prompt/hash/provenance QA/rising-realm-chapter-original-20261014.json. Real rectangular realm cards now use matching independent inset UV subjects instead of the seven simple3D studio thumbnails. First three original chapter art and hero remain. Distinct images are menu illustrations, never substitutes for genuine gameplay. Original scene studio service is retained and independently validated1043checks via its own entry point, including shipping topology, local lights, cleanup and finite leases.

Frozen25a852d1ef4d4a81717c311bea1998ec7c12b3a2d48cfbcd82b3de4668fe3a51: realEditoraggregate/buildexit0 (realm collection2544); real native original chapter170checks across DE/EN portrait/landscape, actual ScrollRect drag handlers, actual UI raycast/click selection and parent Back/scroll restoration; Worlds81/60controllerlandings, Camera147 and Tutorial11 all pass. Actual28newcard screenshots retained, current card subjects inspected against generated original. No new scene leases or root/controller/level IDs. Read-only review no blockers. QA/rising-original-chapters-20261014.json. Menu pictures complete; actual world composition/character quality remains below approved Concept. Visual gate stillFAILED/noTestFlight. Next authorised coherent scene work is the original Waterfall garden reconstruction; RepairEmpire follows Rising, then withdrawn PerfectDrop replay/menu correction.


## 2026-10-14 — genuine coherent Waterfall garden / earned campaign validation

Replaced fourteen scaled scenic landing-island templates in Waterfall with an original broad turquoise valley basin, five eroded terrace banks/trees/falling curtains, modeled lilies/reeds and shared original caustics. Retained three distant Waterfall highlands; other realms unchanged. Original CascadeFlow-v1 full source PNG copied unchanged, imported256repeat; Coast retains its independent ocean texture. Real shared water shader has owned ripple/flow clocks frozen by pause/reduced motion. Missing outlet/spillway UVs corrected; independent actual unbatched tangent fixture validates four complete finite orthogonal water bases. No added collider/StepMarker; basin below lowest landing floor and all banks/flora outside protected x±4.8route corridor. Hard budgets unchanged: garden34411<35000 vertices; whole backdrop64610<70000. Scene owner releases meshes/materials; palette remains twelve materials/five textures.

Frozen2b147fa578218cbf4b60115ef7e8a85d34179d065d277299357eb6056bf88311: actual Editor aggregate/native Mac buildexit0. Actual native chapter170, worlds81/60controllerlandings, camera147, tutorial11 and full default125/36landings+natural fall all pass. QA stride screenshots read actual airborne pixels before pausing only screenshot encoding/one settling frame; no posed character/teleport/focus bypass. The scene-only RisingCourse hook changed, so full campaign was rerun:1704checks,1200real controller landings/100physical portals, each course launched by its real level-card callback with exact realm/level asserted. Fresh native process restores all100 independent earned scores at realm9/level9/step12. Four core/portal files unchanged since875116a and entire IslandArt authority/build prefix still byte-identical; authority audits/proofs recorded in QA/rising-water-garden-20261014.json. Read-only review found no remaining blocker after fixes. Retained failed intermediate missing-kit/budget/QA-compiler runs, none misreported as passing.

Actual untouched frame comparison: Artifacts/RisingWaterGardenComparison-20261014. More coherent water is visible; simplified repeated landforms, sparse vegetation, Temple composition and runner silhouette remain below approved Concept. Visual gate stillFAILED; no version bump/TestFlight/review submission. Next original runner modeling/persistent prefab, fabric and wordmark integration remains authorised; RepairEmpire follows Rising, withdrawn PerfectDrop correction afterward.


## 2026-10-14 — original persistent adventurer, fabric and brand

Own authored articulated3D character now persists as Resources/Models/RisingAdventurer-v2.prefab plus genuine mesh/material/texture sub-assets, rather than regenerating the same toy buffers every run. Curved flattened Bezier hair locks, softer rounded pack/folds, original neutral TravelFabric-v1 bitmap64 and retained original hair surface. Real12ray-per-vertex mesh visibility uses an authored BVH in an Editor-only preview scene; no project physics/world simulation mutation or runtime colliders. Surface shader uses sampled vertex ambient occlusion. Staged shape/normal/degenerate/budget/occlusion/prefab checks precede verified transactional asset publication with rollback. Mutable style materials belong to each avatar; immutable mesh/textures remain shared and survive avatar cleanup. Rig/controller bindings and eight style IDs remain exact. Budget5884vertices/9936triangles/31active renderers/8materials/3textures64, all prior ceilings retained.

Own transparent RisingWordmark-v1 heavy serif temple/island logo replaces thin displayed font wordmark; original RGBA bytes/alpha remain untouched, proportionally contained display crop only, no input raycast; semantic/fallback font title retained. Actual first/last original realm illustrations stay individually selectable. Source/prompt/hash receipts QA/rising-original-runner-wordmark-20261014.json. Frozen315d1e87a3db6c30576157eebe738b2c56084cea1ffa2da15972a694e7e30a3f: actualEditoraggregate/nativebuildexit0, runner31902 and realm gallery2562checks. Actual native chapters170, worlds81/60physical landings, camera147, tutorial11, default125/36landings+natural fall all pass. Added and passed203checks of all100previously completed level-card replay callbacks using the actual prior earned100 saved profile; exact requested course starts at first island without losing stars/wallet, no fixture unlocks. Prior full1200landing/100portal/cold100score evidence remains bound to2b147f; six game/island/portal source files still byte-identical. Read-only review no blockers. Retained early Edit-mode scene API and too-small hair-tip triangle failures, fixed without weakening the contracts.

Unpainted actual comparison Artifacts/RisingOriginalAdventurerComparison-20261014. Own assets are integrated, but hair arrangement, runner/pack silhouette, cliff/plant density and temple composition still fall short of approved Concept. Visual gateFAILED, no claimed90percent/no TestFlight/version bump. Continue graphical refinement of this game before RepairEmpire, then withdrawn PerfectDrop correction/resubmission.


## 2026-10-14 — refined original hair/hardware and fully opaque menu/wallet

Original ChestnutMicro-v2 neutral fibers imported64 replaces the colored striped hair detail. Genuine hand-shaped asymmetric top/rear Bezier locks overlap an irregular crown; smooth exposed back regions corrected after actual intermediate review. Geometry unchanged5884vertices/9936triangles. Existing permanent belt/pack buckle geometry uses brass metallic material; same-joint boot dark buffers merged:30active renderers/8materials/3textures64, no budget relaxation or joint/controller changes. Transactional authored prefab/mesh assets rebaked and reloaded, sampled mesh occlusion preserved. Original PNG copy/hash/prompt QA/rising-chestnut-original-20261014.json.

All menus now have actual plain rectangular opaque paper Image/UI-Default backgrounds, replacing transparent blue/RoundedPanel masking (alpha1 alone would still cut corners, caught by review and corrected). Gameplay HUD, joystick, jump and View hidden during menus, restored before unpause. Real visible localized48point home wallet/diamond directly opens Shop and returns to scrolled parent; narrow headline never overlaps. Heavy own logo and original ten realm cards stay. Full actualEditoraggregate/buildexit0: gallery2586, same strict runner budget; frozen d115ed3fb58e47fe73deb9caa4c253db6b1ef753868e5b8ad5c0526a44ec9599. Actual native chapter/wallet194 across DEENportrait/landscape, Worlds81/60controllerlandings, Camera147, Tutorial11, Default125/36landings+natural fall and203all100actually earned completed-level replay callbacks pass. No runtime exceptions captured. Exact core/island/portal sources still match7dceeb9; full1200landing/100portal/cold100score proof stays bound2b147f, not reassigned. Intermediate1db opaque candidate proofs and unsuccessful visual observation retained. Unity-native empty serialized YAML scalar spacing accepted narrowly via model-asset Git attributes; code whitespace checks remain.

QA/rising-adventurer-wallet-20261014.json; untouched actual comparison Artifacts/RisingAdventurerWalletComparison-20261014. Current menu and own figure are stronger; overall Concept gateFAILED: further world structure/botany/temple composition and actor fidelity needed. No90percent claim/TestFlight/version bump. Work remains serial Rising→RepairEmpire→withdrawnPerfectDrop corrections.
