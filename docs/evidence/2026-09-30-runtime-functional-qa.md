# Runtime Functional QA — 2026-09-30

Canonical local place:
`~/Documents/Roblox/RisingSteps/risingsteps-final2.rbxlx`

Source under test:
- Full-height QA harness commit: `c31e7fcbe861158e951ac95085c94393d7b0624d`
- Runtime log: `0.741.19.7411056_20260930T205334Z_Studio_23cb3_last.log`
- Studio PlayServer/PlayClient test
- No relevant ScriptContext/runtime errors in the acceptance run

## Runtime PASS evidence

The Studio-only server-authoritative QA harness completed successfully and emitted `[RisingStepsQA] COMPLETE`.

Verified:
- profile loaded and course ready
- presentation budgets: 755 BaseParts / 900 budget, 21 Beams+Lights / 96 budget
- sequential server-authoritative landings
- Perfect grading and streak accumulation
- PB update
- Warning → Expiring → Expired state path
- Miss breaks streak
- cosmetic purchase and equip
- environment-theme purchase and equip
- reduced-motion setting mutation
- Developer Product grant path
- duplicate receipt idempotency and no double grant
- failure state
- Revive command, assisted-run semantics and respawn recovery
- second failure → Retry → reset to Height 0 / Streak 0 / not failed
- all 42 initial steps accepted in sequence
- actual generated band mapping:
  - Step 12: `service-decks`
  - Steps 13–30: `cloudline-works`
  - Steps 31–42: `stratosphere-spine`
- BasePart budget remains 755 after full 42-step climb
- daily reward path
- persistent profile payload contains PB 42 and equipped cosmetics
- profile flush path
- invalid/extreme position is rejected by the security path

## Production bugs found and fixed by runtime QA

1. Retry/Revive character reload race:
   direct `LoadCharacter()` could collide with Roblox respawn timing. Character reloads now use a centralized asynchronous `LoadCharacterAsync()` path.

2. Failed course remained temporarily Ready:
   failure now sets `course.Ready = false`; only a valid respawn rebind can make the course Ready again.

3. QA workflow contamination:
   stale Rojo servers and temporary checkouts were removed. The canonical Documents place is the only local runtime source.

## Still separate gates

This evidence does not replace:
- real new-session DataStore rejoin in a published private experience
- real MarketplaceService paid receipt
- touch/tablet/controller device acceptance
- final phone/desktop visual screenshots
- 20-minute stability/performance run
- Creator Dashboard privacy/publish/store-art submission
