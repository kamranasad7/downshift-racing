# Downshift Procedural Visual Pass Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development or superpowers:executing-plans. Steps use checkbox syntax.

**Goal:** The game reads as a stylized mountain descent instead of programmer-gray: sky gradient, parallax hills, shaded terrain, detailed car silhouette, heat particles, distinct pickups. No external art assets — everything procedural.

**Architecture:** Small runtime scripts (ParallaxLayer, CoinSpin) + extensions to existing builders (VehiclePrefabBuilder, TerrainAssetsBuilder, SpriteFactory). Terrain colors via vertex colors on our own mesh. Particles via Unity ParticleSystem configured in code on the vehicle prefab. Every task ends with an MCP play-mode screenshot judged against the acceptance text.

**Palette (dawn mountain):** sky top #2E3A59, sky horizon #B8886B, far hills #4A5578, near hills #5D6B8C, terrain surface #7A6A55, terrain deep #4A3F33, car body keeps #D94D33, accents cream #E8DCC8.

## Global Constraints

- Namespaces/comments/commit rules as before. Suites must stay green: EditMode 41, PlayMode 11.
- All colors/values as public fields on the components or builder constants — no magic inline colors in runtime logic.
- Screenshot verification via MCP for every visual change; include screenshot path + verdict in reports.
- Do not change physics values or `Hatchback.asset`.

---

### Task B1: Sky gradient and parallax hills

**Files:**
- Create: `Assets/Scripts/Visual/SkyGradient.cs` (generates a vertical-gradient Texture2D at runtime on a full-screen quad behind everything, parented to camera, or simpler: large SpriteRenderer with generated gradient sprite following camera x/y at far sorting order)
- Create: `Assets/Scripts/Visual/ParallaxLayer.cs` (`public Transform cam; public float factor; public float baseY;` LateUpdate: `transform.position = new Vector3(cam.position.x * factor + offset, baseY + cam.position.y * factorY, z)` — exact fields per implementer, keep simple)
- Create: `Assets/Scripts/Visual/HillLayerBuilder.cs` (runtime: builds a jagged hill silhouette mesh/sprite strip from Perlin noise, tiled wide enough and re-centered on camera in LateUpdate so it never runs out)
- Modify: `Assets/Editor/TerrainAssetsBuilder.cs` (BuildRunScene adds Sky + 2 hill layers wired to the camera)

Acceptance screenshot: sky shows vertical gradient (deep blue top → warm horizon), two hill silhouette layers move slower than terrain, no gaps at screen edges while driving.

- [ ] Implement, rebuild Run scene, suites green, screenshot verdict, commit `add sky gradient and parallax hill layers`

### Task B2: Terrain shading

**Files:**
- Modify: `Assets/Scripts/Terrain/TerrainStreamer.cs` — mesh gains vertex colors: top vertices `surfaceColor`, bottom vertices `deepColor` (gradient with depth); add a thin second strip along the surface (top ~0.6 world units) in `surfaceStripColor` (brighter path color) — either extra mesh rows or a second small mesh; public Color fields with palette defaults.
- Test: extend `Assets/Tests/EditMode/…` only if pure helpers added; otherwise PlayMode suite green suffices (mesh building already covered).

Acceptance screenshot: terrain reads as lit ground: brighter surface band, darker body fading with depth; chunk seams invisible.

- [ ] Implement, suites green, screenshot verdict, commit `add terrain vertex color shading with surface strip`

### Task B3: Car detail and heat particles

**Files:**
- Modify: `Assets/Editor/VehiclePrefabBuilder.cs` — body gets: cabin/window child (cream + dark window quad), front slope hint (rotated quad), bumper strip; wheels get hub dot (small circle child). Purely child sprites, collider unchanged.
- Create: `Assets/Scripts/Visual/HeatEffects.cs` — MonoBehaviour on vehicle root: references two ParticleSystems (brake glow embers at each wheel, engine smoke at front); reads `VehicleController.Brakes.Temp/stats.brakeMaxTemp` → embers emission rate scales from 0 (below fadeStart) to strong (at max); `EngineTemp/engineMaxTemp` → smoke from 60% up; on `Blown` event fires a one-shot dark burst. ParticleSystems constructed in the prefab builder in code (sprites: existing circle, small sizes, world simulation space).
- Modify: `Assets/Tests/PlayMode/VehicleRigPlayTests.cs` or new — one PlayMode assertion set: after forcing brake temp high (drive+brake as in existing test), embers system `isEmitting` true; keeps suite honest without visual flake (PlayMode 12).

Acceptance screenshot: car has cabin/windows/hubs; braking hard at speed shows ember glow at wheels; engine near max shows smoke. Blowup burst visually verified via forced low engineMaxTemp clone in play mode.

- [ ] Implement, rebuild prefab + scene, suites green (PlayMode 12), screenshots (normal + braking-hot), commit `add car body detail and heat particle effects`

### Task B4: Pickup identity

**Files:**
- Modify: `Assets/Editor/TerrainAssetsBuilder.cs` BuildOne/BuildPickupPrefabs — Coin: inner dot + `CoinSpin` (create `Assets/Scripts/Visual/CoinSpin.cs`: rotate around y-axis illusion via x-scale oscillation); Coolant: droplet (circle + small triangle cap), slight bob (same script, bob mode or separate small script — keep one script with two public bools); Station: real arch — two pillars + top beam + "SERVICE" label (world-space TMP or sprite bars only — bars preferred, no text), trigger unchanged (radius 0.6 circle at center must still overlap car path — keep collider identical).
- Delete + regenerate pickup prefabs (builders are idempotent-guarded — bump: delete assets first inside builder if shape changed: controller decision — builder now always rebuilds pickup prefabs (remove the early-return guard) so visual iterations propagate).

Acceptance screenshot: coins visibly spin, coolant reads as blue droplet, station reads as an arch you drive under. PlayMode suite still green (pickup collection tests unaffected — collider unchanged).

- [ ] Implement, rebuild prefabs + scene, suites green, screenshot verdict, commit `add pickup visual identity with spinning coins and service arch`

## Out of Scope

- Real art assets, animation curves polish, sound (content plan)
- Environment-specific palettes (content plan; fields exist to support them later)
