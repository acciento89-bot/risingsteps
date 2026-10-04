# Rising Steps final store capture progress — 2026-10-04

## Accepted native capture

- `thumb-core`: captured with Roblox Studio `View -> Screenshot`, which writes the rendered 3D/client viewport directly instead of macOS window chrome.
- Source gameplay state comes from the VisualOnly QA thumbnail build derived from current `main` (`caaa6ba`).
- The captured frame visibly contains the avatar and the authored rising-step route; no Terrain Editor, Toolbox, Explorer, Properties, macOS desktop or Studio window chrome is baked into the native screenshot.
- Raw native capture path used for QA: `/tmp/rising-native-thumbs/thumb-core.png` (2162x1386).

## Remaining capture work

- `thumb-height` and `thumb-cosmetics` are not being falsely accepted yet. Multiple Studio runs reached both corresponding QA hold markers, but the automated native-screenshot trigger missed those transient windows.
- P15-T05A/T05B therefore remain open until both additional truthful gameplay captures are produced and reviewed.
