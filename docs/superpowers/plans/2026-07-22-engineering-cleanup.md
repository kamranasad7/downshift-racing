# Downshift Engineering Cleanup Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: superpowers:subagent-driven-development. Checkbox steps.

**Goal:** Retire the deferred review findings accumulated across the prototype and progression phases.

**Tech/constraints:** as previous plans; suites currently EditMode 41 / PlayMode 12; comment/commit rules; do not change tuned asset values.

---

### Task C1: Runtime fixes

**Files:** `Assets/Scripts/Run/RunManager.cs`, `Assets/Scripts/Vehicle/VehicleController.cs`, `Assets/Scripts/Terrain/TerrainProfile.cs`, tests.

1. **DistanceM spawn offset:** RunManager records `_startX = vehicle.transform.position.x` in Start; `DistanceM` = max(0, maxX - _startX). EditMode-testable? RunManager is a MonoBehaviour — cover via existing PlayMode flows (FullLoop asserts bestDistance > 0 still passes; add assert in ProgressionPlayTests that saved best is plausibly small (< 5) for its short run — proves offset applied since spawn x is 4).
2. **Post-fail shutdown:** VehicleController gains `public void Shutdown()` → sets `_shutdown`; FixedUpdate early-returns (release motors once) when `_shutdown || _blown`; creep force not applied when shut down. RunManager calls `vehicle.Shutdown()` on Crashed and BlownUp transitions.
3. **TerrainProfile div-by-zero guard:** if `gradePerMeter <= 0` → trend is plain `-baseGrade * x` (skip cap math). EditMode test: config with gradePerMeter 0 returns finite strictly-descending heights.
4. **Upgrade clone lifetime:** VehicleController tracks `_statsClone` when upgrades applied; `OnDestroy` destroys it.

Suites: EditMode 42+, PlayMode 12 green (fix ProgressionPlayTests assert count as needed). Commit: `cleanup runtime fixes distance offset shutdown terrain guard clone lifetime`

### Task C2: Test/tooling hardening

**Files:** `run-tests.ps1`, `run-playtests.ps1` (replace both with parameterized `run-tests.ps1 [-Mode EditMode|PlayMode]` and a thin `run-playtests.ps1` calling it), station integration PlayMode test.

1. **Script dedup + version sort:** single script with `-Mode` param (default EditMode), results file per mode; Unity discovery sorts versions correctly (`[version]` cast on the numeric prefix, e.g. `6000.3.20` from `6000.3.20f1`, descending). Keep stale-results delete + missing-results throw. `run-playtests.ps1` becomes `& "$PSScriptRoot\run-tests.ps1" -Mode PlayMode @args`. NOTE: cannot execute while editor is open — verify by review + syntax check (`powershell -NoProfile -Command "Get-Command -Syntax"` style parse or pwsh -n), and the final headless verification happens whenever the editor next closes; state this in the report.
2. **Station real-offset integration test:** PlayMode test builds terrain-like ground at a fixed height, spawns Station prefab at ground + 1.4 (the streamer's real offset), drives the car through at moderate speed, asserts CoolFull happened (heat brakes first via cloned-stats trick). PlayMode 13.

Suites: EditMode green, PlayMode 13. Commit: `dedupe test scripts with version sort and station reach integration test`

## Notes / wontfix decisions

- HUD speed vs distance perception (B1 observation): SpeedMs is velocity magnitude (includes vertical) — correct for a downhill game; distance is horizontal x by design. No change.
- "MainCam" SpawnNames entry is used by SpeedCameraPlayTests — kept.
