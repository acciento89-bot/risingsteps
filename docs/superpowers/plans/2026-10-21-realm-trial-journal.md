# Realm trial journal implementation plan

> REQUIRED SUB-SKILL: superpowers:executing-plans, native inline; no children, all executions root-only.

**Goal:** Add explicit personal recorded climbs with new per-course active-time/split history and optional compatible best-run guide, independently of existing star records.
**Architecture:** Plain C# deterministic tracker/book/guide models; separate validated Unity JSON/file adapter; one narrow existing-course observer; partial existing HUD menus. No physics/economy/camera/art/profile rewrite.
**Tech:** Unity6000.6.4f1, existing UGUI/JsonUtility, System-only managed test harness. No package changes.
**Spec:** docs/superpowers/specs/2026-10-21-realm-trial-journal-design.md

## Global constraints
- Old uncommitted concept deletion untouched; isolated bcbcaa1 branch, no old checkout modification.
- Root owns all compilers/editor/runtime/devices; no product before observed RED, no UI integration before pure GREEN.
- Separate8MiB schema1;200keys,last5stat attempts,keyed clean PB,20guides/30000samples;5Hz/1500samples/3600active seconds.
- Existing profile/schema3/stars/crystals/styles/IAP/ads/routes/colliders/camera/assets/settings/version remain unchanged.
- DE/EN,48point/native safe areas/clear jump route, opt-in default-off guide/reduced motion; no source-only native/Duo/visual claims.

## Review focus
- Background/resume/loading delta charged unfairly: pure first-active reanchor tests plus actual OS callbacks.
- Partial/resumed/daily route incorrectly creates PB: constructor/start gates and real accepted12split/portal tests.
- Guide mismatches route or interpolates invalid samples: key/bounds/coverage/endpoint tests and root no-collider fixture.
- Future/corrupt/disk failure overwrites history: actual Unity codec and temp-file failure tests, publication only after verified write.
- New menu/guide obstructs controls or changes old stars/commerce: root real HUD/native/genuine course comparisons, no mocked progression claim.

### Task1: Pure tracker/book/guide
Files: new Assets/Scripts/Core/Mastery/MasteryModels.cs, MasteryTrial.cs, MasteryBook.cs; tools/regressions/MasteryTests.cs and run-mastery-tests.sh.
- [ ] Write behavioral test main FIRST, script always compiles explicit test source even when model glob empty.
- [ ] Root standalone Mono/csc actual RED expected missing MasteryTrial/Book/Guide symbols; no Unity editor.
- [ ] Implement minimum System-only model/rules after root RED; new .meta files unique GUIDs as root tooling requires.
- [ ] Root managed GREEN before observer/UI production; correct failures from actual output.

### Task2: Storage/runtime bridge
Files: new MasteryArchive.cs(Unity codec/file), Gameplay/RisingMastery.cs(observer/owned guide), Editor/RisingMasteryValidation.cs. Add component in GameBootstrap before HUD initialization; old physics/rules/profile unchanged.
- [ ] Tests-first actual codec/header/schema/byte preservation/temp failure and lifecycle/no-collider/root tests; root observes required missing behavior RED before bridge implementation.
- [ ] Atomic separate archive, expected revision, owner cleanup; ordered runtime events and active clock/actual sample capture, no per-frame serialization/allocations.
- [ ] Root Unity GREEN/import/compiler and genuine runtime route/pause/background/ghost comparisons. Source-only checks never stand in for engine proof.

### Task3: HUD/native/review/handoff
Files: RisingHud class becomes partial; new RisingHud.Mastery.cs uses existing Begin/Row/Note navigation, realm album/course stats/guide switch/start and result splits; narrow existing level/result/render callbacks.
- [ ] DE/EN, current route-compatible guide and old-profile/tutor/presenter guards; shared iOS/Android source.
- [ ] Root Unity/native build plus real UI/camera/input/course/guide/persistence/resizing/nativeDuo/visual+icon gates; independent whole branch review.
- [ ] Root verification before local scoped commit, no push/version/store. Per-app RisingStepsUpdate exact files/logs/native limits and factual metadata.
