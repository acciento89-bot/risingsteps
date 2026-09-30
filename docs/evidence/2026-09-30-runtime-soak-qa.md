# Runtime Soak QA — 2026-09-30

Canonical local place:
`~/Documents/Roblox/RisingSteps/risingsteps-final2.rbxlx`

Soak source:
- Source commit under soak: `1fffdfc2ab0ba1b408359810e3f4cad9bbace696`
- Runtime log: `0.741.19.7411056_20260930T211533Z_Studio_67723_last.log`
- Studio PlayServer/PlayClient

## Result

**PASS**

- Elapsed QA soak: **1201.4 seconds**
- Complete cycles: **164**
- Each cycle: reset → 42 sequential server-authoritative landings → budget assertion → real fall-boundary failure → Retry → respawn/ready
- BaseParts at every report: **755 / 900**
- Beams + Lights at every report: **21 / 96**
- No relevant ScriptContext/runtime errors in the soak log
- No unexplained Roblox Studio crash in the controlled run

External Studio RSS samples fluctuated instead of showing monotonic growth. The process remained alive through the complete soak.

This evidence closes:
- P13-T10 mandatory 20-minute stability/performance climb
- P14-T06 generator/cleanup performance and bounded memory/part count
