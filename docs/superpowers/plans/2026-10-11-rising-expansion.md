# Rising Steps expansion implementation plan

> **For agentic workers:** Use executing-plans for root-owned changes; existing bounded art agents implement their assigned files. Steps use checkboxes.

**Goal:** Real 30-level campaign, reliable parent navigation, clearer tutorial labels and improved actual Unity character/landscapes.
**Architecture:** Keep the twelve-jump controller/camera unchanged. Add per-level progress to the existing profile with Schema1 migration; route generation remains deterministic. HUD retains parent pages and scroll independently of refresh.
**Tech Stack:** Unity6000.6.4f1, built-in renderer, C#, native iOS/Android.
**Spec:** User screenshots and requests2026-10-11; approved Oct10 concept boards. Default three existing realms with ten distinct levels each; user can steer the optional campaign question.

## Global constraints
- Preserve existing wallet, purchased styles, tutorial and current checkpoint; unknown future schema remains read-only.
- Native phone touch targets>=49logical points, DE/EN, both orientations, safe areas.
- Root alone runs Unity/builds; no parallel native jobs on8GB Mac.
- Existing user deletion docs/concepts/PRIMARY-CONCEPT.png remains unstaged.
- No public store submission; verified source/private backups/internal TestFlight only.

## Review focus
- Nested settings refresh/language/orientation must preserve its parent and scroll.
- Closing a submenu must retain pause; explicit Resume resumes.
- Older completed realms migrate their first level without granting unearned level stars.
- Locked levels and duplicate completion cannot grant progress twice.
- Thirty routes must differ and remain reachable by the actual motor.

### Task1: Parent navigation
Files: Assets/Scripts/UI/RisingHud.cs; Assets/Editor/RisingNavigationValidation.cs.
- [ ] Write fixture invoking real settings/home/shop/style callbacks, toggles and rebuild, asserting Back parent/paused and explicit Close/resume.
- [ ] Run fixture against baseline and inspect expected failure.
- [ ] Implement parent-page+scroll history; same-page refresh does not push; Close explicitly resumes; Back returns previous page. Rename tutorial labels.
- [ ] Run fixture plus existing touch/tutorial/UI checks and actual landscape/portrait renders.

### Task2: Campaign content and migration
Files: Core/RisingProfile.cs,RisingSave.cs,RisingRules.cs; Gameplay/CoursePatterns.cs,RisingCourse.cs; new Core/RisingCampaign.cs; UI/RisingHud.cs; Editor/RisingCampaignValidation.cs.
Interfaces: RisingCampaign.LevelsPerRealm=10; LevelId(realm,level); IsUnlocked(profile,realm,level); Course.StartLevel(realm,level); CoursePatterns.LevelPoints(realm,level).
- [ ] Add failing checks for30unique routes, reachability, perlevel stars/unlocks, idempotency, old-save economy/style preservation and future read-only guard.
- [ ] Confirm expected baseline failures before production changes.
- [ ] Implement Schema2 perlevel progress and deterministic varied routes with first route compatibility; unique localized titles/tasks; level picker and next-level completion.
- [ ] Validate all30routes in engine, campaign save round trips, actual controller traversal and screenshots in each world.

### Task3: Art integration
Files: agents own RunnerArt/Animator; IslandArt,RealmArt,PortalArt,SkyArt plus isolated fixtures/helpers.
- [x] Real Unity baseline RED both art contracts recorded baseline-rising.log.
- [ ] Integrate source, run joint GREEN then inspect genuine player renders vs concept before acceptance.

### Task4: Delivery
- [ ] Review changes and run required original/editor/runtime checks.
- [ ] Commit only verified changes/main, retain source and proof backup.
- [ ] Build/sign new native versions, verify archive and Android signatures, assign existing internal TestFlight group with specific notes.
