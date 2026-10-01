# Rising Steps — Compact Phone Visual QA — 2026-10-01

Canonical place: `~/Documents/Roblox/RisingSteps/risingsteps-final2.rbxlx`

GitHub source: `00097f06f905d2b95aefe7fc1fa2c5a45195b79b`

Device Simulator: **iPhone XR — 896 × 414 landscape**

The VisualOnly Studio QA harness advanced the canonical course to exact acceptance phases and emitted a marker before every capture. Captures were taken from the active device simulator only after the matching `[RisingStepsQA] VISUAL_HOLD <phase>` marker appeared.

## Accepted captures

| Phase | Runtime marker | Review |
| --- | --- | --- |
| Tutorial | 03:57:01.906 | Spawn composition remains centered on the avatar and climb direction; no empty-baseplate view. |
| Early | 03:57:10.188 | Height 3. Avatar, current platform and upcoming route remain visible; touch joystick/jump targets do not cover the route. |
| Mid | 03:57:18.206 | Height 20. Warning banner remains readable while avatar and next steps stay visible. |
| High | 03:57:26.227 | Height 38. Avatar and next reachable geometry remain readable despite denser high-altitude structure and transient PB/Perfect feedback. |
| Warning | 03:57:34.238 | Warning platform uses visible material/surface/state treatment in addition to color. |
| Result | 03:57:42.239 | RUN OVER panel and Retry are immediately readable without destroying scene context. |
| Shop | 03:57:50.520 | Rising Supply modal is readable at phone width; primary actions remain large enough and hierarchy is clear. |
| Cosmetics | 03:57:58.538 | Cosmetic rows show distinct authored previews (for example Impact Ring, Cloud Pulse, Arc Spark, Amber Forge) and clear BUY/EQUIPPED states; no empty preview boxes. |
| Complete | 03:58:06.556 | Visual harness completed without a relevant runtime script error. |

## Phone acceptance

- Avatar remains visible throughout gameplay captures.
- Current/next reachable geometry remains readable at early, mid and high altitude.
- Height/PB/Streak hierarchy remains legible and does not cover the avatar.
- Touch controls remain separated from the gameplay route.
- Warning state is not color-only.
- Result and shop are intentionally modal and remain usable within the compact viewport.
- Cosmetic/shop preview surfaces are no longer placeholders or empty color boxes.
- No default Roblox baseplate, debug helper geometry or raw default UI is visible inside the shipped game viewport.

This closes the **compact-phone visual/screenshot acceptance** only. Tablet, desktop composition, controller parity and published-place rejoin remain independent gates.
