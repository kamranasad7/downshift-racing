# Downshift v0.1 playtest checklist

Open the project in Unity Hub (`D:\Projects\Unity\DownshiftRacing`), open `Assets/Scenes/Run.unity`, press Play.
Controls: Space = brake, Up/Down arrows = gear up/down (air-tilt while airborne), R = restart from results.

## From the plan

- [ ] 3+ minute continuous descent without errors in Console
- [ ] Braking always feels responsive below fade threshold
- [ ] Brake fade is survivable if anticipated (coast + engine brake)
- [ ] Engine blowup reachable by riding gear 1, avoidable by upshifting
- [ ] Crash reachable by jumping off a crest without air-tilt correction
- [ ] Air-tilt (gear buttons airborne) can save a bad jump
- [ ] Never stuck: 5 restarts, creep force always gets the car moving
- [ ] Chunk streaming seamless at max observed speed
- [ ] HUD readable at a glance; touch targets usable with thumbs (16:9)
- [ ] Distance/coins persist correctly to the results screen
- [ ] Frame rate stable in editor profiler (no per-frame allocations from streaming after warmup)

## Added during overnight execution (things automation could not judge)

- [ ] R key AND the RESTART button both reload the run from the results screen
- [ ] GEAR + / GEAR − / BRAKE on-screen buttons work by mouse click in Play mode
- [ ] HUD layout: gauges top-left, gear top-right, nothing overlapping at 16:9
- [ ] Service station (green arch) visibly restores both gauges when driven through
- [ ] Coolant (blue) partially cools both gauges; coin counter increments
- [ ] FEEL CHECK — gear-1 engine braking: free-roll speed self-limits to ~1.45 m/s in gear 1
      (measured). If the start feels dead-slow, lower `engineBrakeTorque` (260) or
      `gearRatios[0]` (3.2) in `Assets/Data/Hatchback.asset` and re-judge.
- [ ] FEEL CHECK — camera zoom-out and look-ahead at speed feel right (tune SpeedCamera
      values in the Run scene if not)

## After all items pass

Tune any ScriptableObject values you changed, then:

```powershell
git add -A
git commit -m "tune prototype balance values from first playtest"
git tag v0.1-prototype
git push origin core-prototype --tags
```
