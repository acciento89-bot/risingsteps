# Rising Steps Network Authority

## Boundary
Competitive state is server-owned. The client may request actions and render server state; it never declares Height, Streak, currency, ownership, receipt success, valid step IDs or a successful landing.

## V1 remotes
- `PlayerCommand` — client to server. Requests retry, revive use and future gameplay actions. Server validates run state and payload.
- `ShopCommand` — client to server. Requests equip/purchase-related actions. Server validates allowlists, ownership and purchase state.
- `RequestBootstrap` — client to server RemoteFunction. Returns a sanitized initial state snapshot.
- `StateSnapshot` — server to client. Sends authoritative display state.

## Rules
- No remote accepts an arbitrary score/currency grant.
- No client payload is trusted for receipt completion.
- Rate limiting and payload schemas are hardened in P12 before release.
- Runtime gameplay validation is added with the mechanic phases; this document defines the authority contract now so later code cannot invert it.
