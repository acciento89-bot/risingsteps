# Rising Steps desktop keyboard/mouse QA — 2026-10-04

## Scope

This pass closes P14-T03 by combining the already verified keyboard runtime path with a real OS-mouse Shop interaction and by fixing a desktop CoreGui overlap discovered during acceptance.

## Keyboard evidence

The existing Studio input QA on the canonical place already verified:

- forward keyboard movement: **5.04 studs**;
- keyboard jump response: up to **+5.48 studs**;
- the same movement path completed the long 42-step progression/soak without sticky-edge or movement-state failure.

The old blocker was mouse UI interaction, not character movement.

## Desktop CoreGui defect found

In a normal desktop PlaySolo session, Studio's narrowed client viewport could be classified as `tablet` by width even though the session used desktop input. That placed Currency/SHOP in the top-right region occupied by Roblox's default player-list CoreGui.

The production rule is now input-aware:

- touch phone/tablet layouts keep their existing offsets;
- non-touch desktop/controller sessions reserve a 248 px right inset for top-right CoreGui even if the client viewport is narrower than the nominal desktop width;
- the layout remains on the top row (`18 px` top offset).

Pure-Luau viewport tests cover normal desktop and a narrow non-touch desktop viewport.

## Runtime mouse acceptance

Evidence:

- `docs/evidence/2026-10-04-desktop-input/coregui-layout.png` — Currency/SHOP visibly sit left of the Roblox player list with no overlap.
- `docs/evidence/2026-10-04-desktop-input/shop-open.png` — a native macOS CoreGraphics `MouseDown/MouseUp` on the visible SHOP button opens `RISING SUPPLY`.
- `docs/evidence/2026-10-04-desktop-input/shop-closed.png` — a native macOS mouse click on the visible close control returns to gameplay.

This is not the Studio virtual-input helper; the UI was exercised through native OS mouse events delivered to the running PlaySolo client.

CreatorErrors during the verified runtime: **0**.

## Acceptance

- Keyboard movement: accepted.
- Keyboard jump: accepted.
- Desktop Shop mouse open: accepted.
- Desktop Shop mouse close: accepted.
- Roblox player-list overlap: fixed and visually accepted.
- P14-T03 Desktop keyboard/mouse: **accepted**.
