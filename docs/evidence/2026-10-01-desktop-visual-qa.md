# Rising Steps — Desktop Visual QA — 2026-10-01

Canonical place: `~/Documents/Roblox/RisingSteps/risingsteps-final2.rbxlx`

Visual QA source:
- gameplay-state captures from the canonical VisualOnly harness
- shop/cosmetics captures from source `c6a25b1e9708787af0d6e33b9dffc0cce07d5ee8`; that commit only front-loads Studio-only visual phases and does not alter production presentation

Viewport: Roblox Studio desktop play viewport, device simulation disabled.

## Accepted states

| Phase | Review |
| --- | --- |
| Tutorial / spawn | Avatar is the visual anchor and the initial route is readable; no empty-baseplate presentation. |
| Early | Avatar remains large enough to read; next reachable platforms and landing centers are clear; HUD stays outside the route. |
| Mid | Height/PB/Streak remain readable; LOWER STEP RETRACTING warning is prominent without covering the avatar/next target. |
| High | High-altitude structure, transient NEW PB/PERFECT feedback and upcoming platforms remain simultaneously readable. |
| Warning | Warning state changes surface/material treatment as well as color and remains legible against the sky/world. |
| Result | RUN OVER and Retry are immediate and readable while preserving scene context. |
| Shop | Rising Supply uses intentional spacing, grouped sections, clear primary actions and a controlled modal width rather than stretched phone layout. |
| Cosmetics | Distinct authored previews for trail/landing/theme items are visible with clear Included/Equipped/Buy states; there are no empty preview boxes. |

## Desktop acceptance

- Avatar and next 2–3 reachable steps remain legible through low/mid/high altitude.
- HUD does not cover the avatar or intended landing targets.
- Added desktop space is used for gameplay view and a controlled shop modal rather than merely scaling phone UI.
- Warning, result and shop states use a consistent production component kit.
- No default Roblox UI panels, placeholder art, empty-baseplate composition, obvious debug/helper geometry or raw unstyled Parts appear as shipped presentation.
- Platform kit shows landing targets, edge treatment, braces/supports and non-identical structural treatment.
- High-altitude feedback remains transient and does not permanently obscure navigation.

Together with `docs/evidence/2026-10-01-phone-visual-qa.md`, this closes the compact-phone + desktop screenshot-quality portion of P09/P10/P13. Tablet, physical touch input, controller parity, low-graphics pass and published-place validation remain separate gates.
