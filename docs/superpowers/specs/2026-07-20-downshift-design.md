# Downshift — Design Spec

**Date:** 2026-07-20
**Status:** Approved design, pre-implementation
**Working title:** Downshift (store name chosen before release; renaming is trivial)

## One-liner

A Hill Climb Racing-style 2D physics driving game, inverted: you descend endless mountain roads, and the skill is shedding speed — juggling brake temperature and engine-braking heat with gears — instead of managing throttle and fuel.

## Goals

- Commercial release: free-to-play, ads + IAP (Hill Climb Racing model)
- Android first (Google Play), iOS later
- Solo developer, Unity (existing experience)

## Core Gameplay

### The fantasy

Gravity is the antagonist. Acceleration is free and unstoppable; the game is about controlling it. Every slope asks the player to blend two ways of shedding speed, each of which overheats a different part of the vehicle.

### Vehicle physics

- `Rigidbody2D` chassis + two `WheelJoint2D` wheels (Box2D — same physics family as HCR)
- No driver ragdoll. A **roof collider on the chassis is the crash trigger** — the run ends when the car lands on its roof
- No throttle. Gravity is the only propulsion; terrain always trends downhill

### The two heat gauges

**Brake temp (soft fail):**
- Rises while brake is held; rise rate scales with speed
- Cools slowly when released; coolant pickups and service stations cool it instantly
- Past the fade threshold, braking force weakens progressively; fully overheated brakes do almost nothing until cooled below a re-engage point
- Never ends the run directly — it creates a **runaway state** where terrain and hazards deliver the crash

**Engine temp (hard fail):**
- Current gear maps wheel speed to RPM; low gear at high speed = redline
- RPM above redline builds engine heat; below it, engine cools
- Max engine temp = **blowup, run ends on the spot**

The asymmetry is deliberate: brakes are safe but short-term finite; engine braking is sustainable but fatal when mismanaged.

### Controls (3 touch targets, landscape)

- Left thumb: **brake pedal** (hold)
- Right thumb: **gear down / gear up** buttons
- **Airborne, the gear buttons become air-tilt** (gear-down = nose up, gear-up = nose down) — replaces HCR's gas/brake air rotation without a fourth button
- No gas button. No other inputs during a run.

### Fail states

1. **Crash** — car flips onto its roof (over-rotation in air, nose-dive pitchpole into an upslope, runaway tumble)
2. **Engine blowup** — engine temp maxed
3. Brake fade is not a direct fail; it leads to (1)

One **rewarded-ad continue per run**: respawn stationary at the crash point with both gauges cooled.

### Scoring

- **Distance is the score** (per-environment best tracked)
- Coins float on the track, denser on risky lines (jumps, cliff edges)
- Small bonuses: airtime, flips, sustained near-redline "heat management" streaks

## World

### Endless terrain

- Chunk-based procedural generation using Unity **SpriteShape** splines
- Seeded noise shaped by a difficulty curve: with distance, average grade steepens, sustained steeps lengthen (brake-endurance tests), crests/drops sharpen, cooling gets sparser
- **Stuck-proofing (hard constraint):** with no gas button, the generator must guarantee forward progress — net elevation always decreases, and any local rise is small enough that a car entering at walking speed still clears it. No dips a stationary car cannot roll out of.
- Chunks stream ahead of the player and are pooled/recycled behind

### Environments (coin-unlocked stages)

Each is a visual set + a terrain/physics personality, with its own best-distance record:

| Environment | Personality |
|---|---|
| Countryside Foothills | Starter; gentle grades, forgiving |
| Alpine Pass | Long sustained steeps; rockfall hazards |
| Icy Ridge | Low grip — friction braking treacherous, engine braking shines; ice-rut hazards |
| Volcano Road | Ambient heat — both gauges cool slower; lava-rock hazards |

(These four ship at launch; names, order, and numbers are tunable during balancing.)

### Pickups & service stations

- **Coolant pickup** (single pickup type): partially cools **both** gauges — no inventory decisions at speed
- **Service-station arch**: drive through at any speed, fully restores both gauges instantly
- Both get rarer with distance — this is part of the difficulty ramp
- Zero UI, no stopping, HCR-fuel-can style

### Hazards ("brake checks")

- Sparse, telegraphed road damage: potholes, ruts, washboard gravel, fallen rocks — environment-flavored
- Survivable slow, violent fast: at crawl speed you rumble through; at runaway speed they bounce the car into an unstable tumble
- **Road signs telegraph each hazard about a screen ahead** ("STEEP GRADE", "ROUGH ROAD") — with working brakes they're a skill check; with faded brakes they're the fatal consequence of runaway
- Density and severity scale with distance

## Progression & Economy

### Garage (mixed HCR-style roster)

Each vehicle is a different answer to the heat problem. Launch roster direction (tunable):

- Rusty hatchback — starter, mediocre everything
- School bus — heavy and scary, big brake capacity
- Sports car — fast and fragile, razor-thin redline
- Tuk-tuk — the joke vehicle
- Semi truck — engine-braking king, awful friction brakes

Vehicles unlock with coins. All per-vehicle numbers live in `VehicleStats` ScriptableObjects.

### Upgrades — 4 tracks per vehicle, ~8 tiers each

1. **Brakes** — heat capacity, fade resistance
2. **Radiator** — engine cooling rate, redline tolerance
3. **Gearbox** — extra gears, stronger engine braking
4. **Tires & suspension** — grip, landing stability

Escalating coin costs per tier.

## Monetization (HCR model)

- **Rewarded ads:** double run coins, the one-per-run continue, daily bonus
- **Interstitials:** between runs, frequency-capped (caps in a config SO)
- **No banners during gameplay**
- **IAP:** coin packs, remove-ads
- Google Mobile Ads Unity SDK + UMP consent flow; Unity IAP

## Technical Architecture

- **Unity 6.3 LTS**, 2D URP template, C#, landscape orientation
- **Heat model:** `BrakeSystem` + `EngineSystem` as pure, deterministic C# math over the stats SO (heat in/out per second as functions of speed, gear, brake input) — unit-testable and graphable in an editor tool without running the game
- **Terrain:** `TerrainGenerator` streaming pooled SpriteShape chunks; difficulty curve parameters in a ScriptableObject; coins/hazards/pickups pooled — no mid-run instantiation after warmup
- **Run flow:** state machine `Descending → (Crashed | BlownUp) → ContinueOffer → Results → Menu`, owned by a `RunManager` that emits events; HUD/audio/ads subscribe — no cross-system reaching
- **Camera:** Cinemachine follow with speed-based zoom-out and look-ahead (sight distance must grow with speed)
- **Feel:** engine audio pitch tracks RPM; brake squeal + glow scale with brake temp; particle effects for brake glow/smoke, engine steam, blowup
- **HUD:** brake gauge, engine/RPM gauge, current gear, speed, distance, coins
- **Persistence:** JSON save in `persistentDataPath` (coins, unlocks, upgrade tiers, best distances, settings); no accounts/cloud at v1
- **Tuning rule:** no gameplay numbers hardcoded — everything in ScriptableObjects (`VehicleStats`, difficulty curves, economy, ad caps)

## Testing

- **EditMode unit tests:** heat/economy math — brake fade curves, RPM-gear mapping, cooling rates, coin payouts, upgrade cost curves
- **PlayMode smoke test:** a car descends generated terrain without errors; pooling doesn't leak
- **Balancing:** manual, aided by an editor tool that graphs the heat model curves per vehicle/gear

## Out of Scope (v1)

- iOS release (architecture keeps it possible; Android ships first)
- Multiplayer / ghosts / leaderboards beyond local best distance
- Cloud save / accounts
- Obstacles beyond the telegraphed road-damage hazards
- Race/throttle button (explicitly cut — gravity only)
