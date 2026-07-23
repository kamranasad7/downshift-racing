# Downshift Hazards & Road Signs Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: superpowers:subagent-driven-development. Checkbox steps.

**Goal:** The spec's brake-check mechanic: physical road hazards (rocks, ridges, washboard) spawned by distance, telegraphed by warning signs, survivable slow and violent fast — making brake failure actually fatal.

**Architecture:** Pure `HazardPlacer` (deterministic, chunk-based, mirrors PickupPlacer) + procedural hazard/sign prefabs built by editor code + spawning in `TerrainStreamer`. Hazards are REAL static colliders (non-trigger) so consequences emerge from physics, not scripts: crawl over them and you rumble; hit them at speed and the suspension/angular chaos does the rest. Signs are visual-only.

**Constraints:** as previous plans (namespaces, no comments, single-line commits, tunables in TerrainConfig, don't alter user-tuned values). Suites at start: EditMode 49, PlayMode 14.

---

### Task D1: HazardPlacer math

**Files:** `Assets/Scripts/Terrain/HazardPlacer.cs`, TerrainConfig additions, `Assets/Tests/EditMode/HazardPlacerTests.cs`

- TerrainConfig new fields: `hazardEveryMeters = 350f`, `hazardMinSpacingMeters = 150f`, `hazardRampMeters = 3000f`, `hazardStartMeters = 250f` (no hazards before this — tutorial grace), `signLeadMeters = 25f`, `stationClearMeters = 60f` (no hazard within this distance of a station placement).
- `enum HazardKind { Rocks, Ridge, Washboard }`
- `static List<(float x, HazardKind kind)> HazardPlacer.PlacementsForChunk(int chunkIndex, TerrainConfig c)` — deterministic (Perlin jitter like PickupPlacer), spacing lerps from hazardEveryMeters down to hazardMinSpacingMeters over hazardRampMeters, kinds rotate pseudo-randomly by seeded noise, positions before `hazardStartMeters` skipped, and any position within `stationClearMeters` of a `PickupPlacer` Station placement is dropped.
- `static List<float> HazardPlacer.SignsForChunk(int chunkIndex, TerrainConfig c)` — one sign x = hazard x − signLeadMeters for every hazard whose SIGN falls in this chunk (hazard may be in next chunk — handle the boundary case).
- TDD (red first): determinism; spacing shrinks with distance; nothing before hazardStartMeters; station exclusion (construct config where a hazard would land near a station, assert dropped); sign lead distance exact; chunk-boundary sign case (sign in chunk k for hazard in chunk k+1).
- EditMode target 55+. Commit: `add deterministic hazard and warning sign placement math`

### Task D2: Prefabs, spawning, physics discrimination

**Files:** `Assets/Editor/HazardAssetsBuilder.cs`, `Assets/Scripts/Visual/` (sign bob optional none), TerrainStreamer spawning, `Assets/Tests/PlayMode/HazardPlayTests.cs`

- Builder (menu `Downshift/Build/Hazard Prefabs`, always-rebuild like pickups): Rocks = 2 grey circles (r ~0.28/0.2 world) side by side as solid CircleCollider2D on a root at road height; Ridge = square rotated 45° half-buried (BoxCollider2D, ~0.5 wide, sticking ~0.25 above road); Washboard = 5 small bumps (r 0.12) spaced 1.2 over ~5m under one root. All colliders NON-trigger, static (no Rigidbody2D). Sign = pole (thin quad 1.2 tall) + diamond board (rotated square, warm yellow #E8B84B with dark border quad) — NO collider.
- TerrainStreamer: spawn hazards (`HazardPlacer.PlacementsForChunk`) at `TerrainProfile.Height(x)` (sitting ON road) and signs (`SignsForChunk`) at height + 1.6, into the existing per-chunk "Pickups" parent (rename parent to "Spawned") — same lifecycle. New public prefab fields wired by TerrainAssetsBuilder.
- Rebuild prefabs + Run scene headlessly/MCP.
- PlayMode tests (the discriminating pair, one test method each):
  - Slow over rocks: flat ground + rocks hazard at x=15, car crawls (forced velocity capped 2 m/s), RunManager wired; assert car passes x>20 with State still Descending.
  - Fast into rocks: same setup, forced 14 m/s; assert within 4s the car either crashes (State != Descending) OR |angularVelocity| exceeded 200 deg/s at some point (destabilized). This proves speed-dependence without over-constraining physics.
- Screenshots: sign visible one screen before rocks on the road; all three hazard kinds rendered.
- PlayMode target 16. Commit: `add physical road hazards with warning signs and spawning`

## Out of scope
- Environment-flavored hazards, hazard art skins (post style-decision), density difficulty settings beyond config defaults.
