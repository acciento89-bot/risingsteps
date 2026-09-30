# Rising Steps Build and Place Policy

## Canonical source
GitHub `main` is the only canonical source branch for V1. Roblox Studio edits that are not represented in source are disposable and must not become the sole copy of production logic.

## Local verification
From the repository root:
1. `rokit install`
2. `stylua --check src tests scripts`
3. `selene src tests scripts`
4. `lune run scripts/run-tests`
5. `lune run scripts/release-readiness`
6. `rojo build default.project.json --output build.rbxlx`

All six must pass before a code task is marked verified.

## Places
- **Development place**: used for Studio iteration, instrumentation and non-purchase runtime QA.
- **Private release place**: receives only a verified canonical build and is the source for final runtime evidence/screenshots.
- **Public place**: remains blocked until P15 gates are satisfied, including real receipt and persistence/rejoin evidence.

Do not maintain separate drifting source trees for dev and production places. Environment-specific IDs belong in configuration/secure deployment inputs, not forked gameplay logic.

## Build hygiene
Temporary builds, screenshots and captures live under `/tmp/risingsteps-qa`. Curated evidence only may be committed under `docs/evidence/`. Nothing temporary belongs on the user's Desktop.
