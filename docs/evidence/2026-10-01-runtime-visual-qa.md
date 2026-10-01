# Runtime visual QA — 2026-10-01

## Canonical runtime source

- Canonical local place: `~/Documents/Roblox/RisingSteps/risingsteps-final2.rbxlx`
- Visual camera-fix source commit: `cb8df15d1c3fff05df2b57b81c50a2a673c63848`
- Device Simulator phone profile: iPhone XR, 896×414
- Desktop pass: native Studio desktop viewport
- Visual sequence: Tutorial → Early → Mid → High → Warning → Result → Shop

## Curated evidence

### Compact phone
- `phone-tutorial.png`
- `phone-early.png`
- `phone-mid.png`
- `phone-high.png`
- `phone-warning.png`
- `phone-result.png`
- `phone-shop.png`

### Desktop
- `desktop-early.png`
- `desktop-mid.png`
- `desktop-high.png`
- `desktop-result.png`
- `desktop-shop.png`

## Findings

The first phone capture set exposed a real camera defect: rapid vertical progression could leave the avatar clipped at mid altitude and fully outside the frame at high altitude. The camera was corrected to hard-reframe on authoritative height jumps of three or more steps, use a less aggressive future-step focus weight on compact viewports, and widen compact framing.

The post-fix phone pass verifies:
- avatar remains fully visible at early, mid and high altitude;
- the next reachable geometry remains visible and readable;
- Height/PB/Streak remain legible without covering the avatar or current landing step;
- the retracting-step warning remains non-color-only through the explicit warning banner and authored warning treatment;
- Result/Retry fits inside the compact viewport;
- the Shop fits inside the compact viewport with touch-sized controls.

The desktop pass verifies:
- avatar remains visible at early, mid and high altitude;
- added viewport space exposes more world/route context rather than stretching the phone UI;
- warning, result and shop surfaces remain proportioned and readable;
- no empty baseplate, placeholder UI, debug geometry, obvious z-fighting, floating props or visible generation pop appears in the curated frames.

## Runtime evidence used together with these screenshots

The canonical Studio QA harness already verifies 42/42 sequential landings across all three difficulty bands, Warning→Expired, streak build/break, physical fall failure, Revive, Retry/respawn, cosmetic purchase/equip, reduced-motion toggle, security rejection and stable 755 BaseParts / 21 Beams+Lights. The mandatory soak completed 1201.4 seconds across 164 full climb→fall→retry cycles without a relevant runtime script error.

## Remaining external/device gates

- Tablet touch/device viewport
- Controller input parity
- Low/mobile graphics-quality visual pass
- Published-private-place rejoin and real paid receipt
- Creator Dashboard/store submission and final private-place screenshots/version record
