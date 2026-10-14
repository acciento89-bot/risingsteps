# Rising Steps App Review Release Plan

> **For agentic workers:** Use superpowers:executing-plans inline. The user explicitly authorizes finishing App Store Connect, ads, purchases, screenshots and submission; continue without another approval loop.

**Goal:** Submit the accepted Rising Steps build and its actual in-app products to Apple App Review with current screenshots and complete truthful store information.

**Architecture:** Preserve the accepted graphics/gameplay and existing StoreKit/Google SDK adapters. Resolve production catalog/ad-unit configuration, then build and validate iOS, capture actual screenshots, upload and verify Apple's persisted submission state.

**Tech Stack:** Unity6000.6.4f1, Unity IAP5.4.4, Google Mobile Ads11.5.0, Xcode27.1, existing Apple team/App Store Connect/AdMob.

**Spec:** Current user release request, AGENTS.md, docs/INTERSTITIALS-20261011.md and accepted graphics receipt docs/QA/rising-nine-realm-parity-mountain-20261014.json.

## Global constraints

- Bundle com.kamilunavo.risingsteps; Apple6819872415; teamTKG684N5GL. Preserve user-deleted primary Concept.
- Automatic ads only after at least3successful complete courses and180seconds, at completed-result boundaries; none in tutorial or during play.
- Per-game permanent Remove Ads target GermanyEUR1.99; premium products and Restore visible in Shop. Actual localized Store prices only.
- Real iOS/native screenshots; no Concept art masquerading as gameplay. DE/EN metadata, privacy and reviewer access must match actual behavior.
- Serialize Unity/native/Simulator on this8GBMac. No credential/token output; no erasing existing valuable artifacts or user saves.
- Any release-only configuration/version change requires source-bound visual review revalidation before upload.

## Review focus

- Test ad flags or blank production interstitial ID silently shipping.
- Missing first-IAP metadata/price/availability/review screenshot or omission from the app submission.
- Misleading screenshot, stale old build, or internal QA save/network flag in production.
- Missing consent/privacy declaration or post-ad/purchase pause/input/audio recovery.
- Reporting upload or draft as submitted; verify actual Apple persisted review status.

## Task1: Production commerce and store preflight

- [x] Inspect live app/build/product/ad-unit states through existing authenticated access; record safe evidence.
- [x] Fill actual production units and three product localizations/prices/availability/review assets. Preserve established starter/collection pricing where already configured.
- [x] Validate3course/180second policy, entitlements/restore/idempotency and privacy/native ownership; fix only demonstrated blockers.

## Task2: iOS release and actual screenshots

- [x] Select next unused build number; export actual release and Simulator builds. Verify IDs, SDKs, signing and privacy manifests.
- [x] Run iOS startup/gameplay/HUD/menu/save checks and inspect actual native images; capture valid iPhone/iPad store dimensions as required.
- [x] Refresh exact-source visual evidence for release-only changes, run required aggregate checks, archive/export/upload and inspect processing.

## Task3: App Store Connect completion and submission

- [x] Complete actual localized descriptions/keywords/URLs/category/age rating/reviewer info/privacy, current screenshots, build selection and all first-IAP submission items.
- [x] Submit authorized app/products for review and read back exact Apple state; retain redacted receipt and canonical ledger evidence.
- [x] Commit/push verified source/metadata records, preserving unrelated user deletion; report exact outcome and any concrete remaining blocker.

**Current release evidence:** Build1.0(6), ASC build0b592f6f-cc6b-47f5-9097-065ad522ef60 VALID/APP_STORE_ELIGIBLE and attached. Archive/export/upload succeeded with existing release API key after Xcode account provider error. Actual serial iOS Simulator checks205/196/201/205 pass. Published DE/EN Rising consent and App Privacy. Native StoreKit prices load correctly; direct off-device purchase helper is not entitled in unsigned Simulator UI-runner, so exercise actual in-game buttons next. DE opaque screenshot packaging corrects Unity alpha output; source RGB/dimensions unchanged. All evidence lives at Builds/RisingRelease-20261014. Submission and English captures remain pending.

**User steering,2026-10-14:** Finish this Rising Steps submission first, then inspect and resolve the latest Apple rejection of One More Floor. This takes precedence over the previous RepairEmpire-next order; do not infer Apple’s reason before reading the actual message.

**Final:** App and three IAP versions submitted; Apple persisted WAITING_FOR_REVIEW at2026-10-14T20:50:19.635Z. Twenty screenshots COMPLETE. Native transaction limitations are explicit in QA/rising-app-review-release6-20261014.json. Commit/push follows immediately; next task is user-requested One More Floor rejection.
