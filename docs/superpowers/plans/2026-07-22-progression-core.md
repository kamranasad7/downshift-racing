# Downshift Progression Core Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Coins persist between runs, buy upgrades in a garage that actually change the car, and the game loops menu → run → results → menu.

**Architecture:** Pure C# `SaveModel` + `EconomyModel` (TDD, no MonoBehaviours) with a thin `SaveStore` JSON file wrapper. Upgrades apply as multipliers to a cloned `VehicleStats` at run start. A `Menu` scene built by an editor builder hosts PLAY + garage; `RunManager` banks coins to the save on results.

**Tech Stack:** Unity 6000.3.20f1, existing project at D:\Projects\Unity\DownshiftRacing (branch core-prototype). EditMode tests via MCP run_tests or .\run-tests.ps1; PlayMode likewise.

## Global Constraints

- Namespace `Downshift` runtime, `Downshift.EditorTools` editor tools, tests in existing test asmdefs.
- No code comments unless genuinely non-obvious. Single-line commit messages, no AI attribution.
- No gameplay/economy numbers hardcoded in logic — all tunables in `EconomyConfig` ScriptableObject.
- Pure models never reference MonoBehaviour. TDD: red evidence before implementation (compile error counts).
- Do NOT modify `Assets/Data/Hatchback.asset` base stats values (user's tuned feel) — upgrades apply at runtime to a clone.
- Save file: `Application.persistentDataPath + "/save.json"`, but all logic testable with an injected path.
- Current test counts before this plan: EditMode 34, PlayMode 8.

---

### Task A1: SaveModel and JSON store

**Files:**
- Create: `Assets/Scripts/Meta/SaveModel.cs`
- Create: `Assets/Scripts/Meta/SaveStore.cs`
- Test: `Assets/Tests/EditMode/SaveTests.cs`

**Interfaces:**
- Consumes: nothing
- Produces:
  - `class SaveModel { public int coins; public float bestDistanceM; public int[] upgradeTiers = new int[4]; public int schemaVersion = 1; }` (plain serializable class)
  - `static class SaveStore`: `SaveModel Load(string path)` (missing/corrupt file → fresh model), `void Save(SaveModel m, string path)`, `string DefaultPath => Application.persistentDataPath + "/save.json"`

- [ ] **Step 1: Write failing tests**

```csharp
using System.IO;
using NUnit.Framework;
using Downshift;

public class SaveTests
{
    string TempPath() => Path.Combine(Path.GetTempPath(), "downshift_test_save.json");

    [SetUp]
    public void Clean()
    {
        if (File.Exists(TempPath())) File.Delete(TempPath());
    }

    [Test]
    public void LoadMissingFileGivesFreshModel()
    {
        var m = SaveStore.Load(TempPath());
        Assert.AreEqual(0, m.coins);
        Assert.AreEqual(0f, m.bestDistanceM);
        Assert.AreEqual(4, m.upgradeTiers.Length);
    }

    [Test]
    public void RoundTripPersistsFields()
    {
        var m = new SaveModel { coins = 123, bestDistanceM = 456.7f };
        m.upgradeTiers[2] = 3;
        SaveStore.Save(m, TempPath());
        var loaded = SaveStore.Load(TempPath());
        Assert.AreEqual(123, loaded.coins);
        Assert.AreEqual(456.7f, loaded.bestDistanceM, 0.001f);
        Assert.AreEqual(3, loaded.upgradeTiers[2]);
    }

    [Test]
    public void CorruptFileGivesFreshModel()
    {
        File.WriteAllText(TempPath(), "{not json!!");
        var m = SaveStore.Load(TempPath());
        Assert.AreEqual(0, m.coins);
    }
}
```

- [ ] **Step 2: Run EditMode tests, confirm FAIL (compile error)**
- [ ] **Step 3: Implement**

`SaveModel.cs`:
```csharp
namespace Downshift
{
    [System.Serializable]
    public class SaveModel
    {
        public int coins;
        public float bestDistanceM;
        public int[] upgradeTiers = new int[4];
        public int schemaVersion = 1;
    }
}
```

`SaveStore.cs`:
```csharp
using System.IO;
using UnityEngine;

namespace Downshift
{
    public static class SaveStore
    {
        public static string DefaultPath => Application.persistentDataPath + "/save.json";

        public static SaveModel Load(string path)
        {
            try
            {
                if (!File.Exists(path)) return new SaveModel();
                var m = JsonUtility.FromJson<SaveModel>(File.ReadAllText(path));
                if (m == null || m.upgradeTiers == null || m.upgradeTiers.Length != 4) return new SaveModel();
                return m;
            }
            catch
            {
                return new SaveModel();
            }
        }

        public static void Save(SaveModel m, string path)
        {
            File.WriteAllText(path, JsonUtility.ToJson(m));
        }
    }
}
```

- [ ] **Step 4: Run EditMode tests, confirm PASS (37 total)**
- [ ] **Step 5: Commit** `add save model and json store with corrupt file fallback`

---

### Task A2: EconomyConfig and upgrade math

**Files:**
- Create: `Assets/Scripts/Meta/EconomyConfig.cs`
- Create: `Assets/Scripts/Meta/Upgrades.cs`
- Modify: `Assets/Editor/DefaultAssetsBuilder.cs` (create `Assets/Data/Economy.asset`)
- Test: `Assets/Tests/EditMode/UpgradesTests.cs`

**Interfaces:**
- Consumes: `SaveModel.upgradeTiers`, `VehicleStats`
- Produces:
  - `enum UpgradeTrack { Brakes = 0, Radiator = 1, Gearbox = 2, Tires = 3 }`
  - `class EconomyConfig : ScriptableObject` fields: `int baseUpgradeCost = 100;` `float costGrowth = 1.6f;` `int maxTier = 8;` `float brakesCapacityPerTier = 0.10f;` `float radiatorCoolPerTier = 0.10f;` `float gearboxEngineBrakePerTier = 0.06f;` `float tiresGripPerTier = 0.06f;`
  - `static class Upgrades`: `int CostFor(int currentTier, EconomyConfig c)` = `round(baseUpgradeCost * costGrowth^currentTier)`, -1 if `currentTier >= maxTier`; `bool CanBuy(SaveModel s, UpgradeTrack t, EconomyConfig c)`; `bool Buy(SaveModel s, UpgradeTrack t, EconomyConfig c)` (deduct + increment, false if can't); `VehicleStats ApplyTo(VehicleStats baseStats, SaveModel s, EconomyConfig c)` → `Object.Instantiate(baseStats)` clone with: Brakes tier → `brakeMaxTemp`, `brakeFadeStartTemp`, `brakeReengageTemp` all × `(1 + tier*brakesCapacityPerTier)`; Radiator tier → `engineCoolPerSecond` × factor and `engineHeatAtRedline` ÷ factor where factor = `(1 + tier*radiatorCoolPerTier)`; Gearbox tier → `engineBrakeTorque` × `(1 + tier*gearboxEngineBrakePerTier)`; Tires tier → `wheelFriction` × `(1 + tier*tiresGripPerTier)` and `suspensionDamping` × same factor clamped ≤ 1.

- [ ] **Step 1: Write failing tests** (cost growth, cap at maxTier returns -1, Buy deducts and refuses when poor or capped, ApplyTo scales the right fields and does NOT mutate the base asset)

```csharp
using NUnit.Framework;
using UnityEngine;
using Downshift;

public class UpgradesTests
{
    EconomyConfig C()
    {
        var c = ScriptableObject.CreateInstance<EconomyConfig>();
        c.baseUpgradeCost = 100;
        c.costGrowth = 2f;
        c.maxTier = 3;
        c.brakesCapacityPerTier = 0.5f;
        return c;
    }

    [Test]
    public void CostDoublesPerTier()
    {
        var c = C();
        Assert.AreEqual(100, Upgrades.CostFor(0, c));
        Assert.AreEqual(200, Upgrades.CostFor(1, c));
        Assert.AreEqual(400, Upgrades.CostFor(2, c));
        Assert.AreEqual(-1, Upgrades.CostFor(3, c));
    }

    [Test]
    public void BuyDeductsAndIncrements()
    {
        var c = C();
        var s = new SaveModel { coins = 250 };
        Assert.IsTrue(Upgrades.Buy(s, UpgradeTrack.Brakes, c));
        Assert.AreEqual(150, s.coins);
        Assert.AreEqual(1, s.upgradeTiers[0]);
        Assert.IsFalse(Upgrades.Buy(s, UpgradeTrack.Brakes, c));
        Assert.AreEqual(150, s.coins);
    }

    [Test]
    public void BuyRefusesAtMaxTier()
    {
        var c = C();
        var s = new SaveModel { coins = 999999 };
        s.upgradeTiers[0] = 3;
        Assert.IsFalse(Upgrades.Buy(s, UpgradeTrack.Brakes, c));
    }

    [Test]
    public void ApplyToScalesBrakesWithoutMutatingBase()
    {
        var c = C();
        var baseStats = ScriptableObject.CreateInstance<VehicleStats>();
        baseStats.brakeMaxTemp = 100f;
        var s = new SaveModel();
        s.upgradeTiers[0] = 2;
        var applied = Upgrades.ApplyTo(baseStats, s, c);
        Assert.AreEqual(200f, applied.brakeMaxTemp, 0.001f);
        Assert.AreEqual(100f, baseStats.brakeMaxTemp, 0.001f);
    }
}
```

- [ ] **Step 2: Run EditMode, confirm FAIL**
- [ ] **Step 3: Implement exactly per the Produces contract above**
- [ ] **Step 4: Add Economy.asset creation to DefaultAssetsBuilder (same guarded pattern as Terrain/Hatchback), run builder headlessly or via MCP execute_menu_item, verify asset exists**
- [ ] **Step 5: Run EditMode, confirm PASS (41 total)**
- [ ] **Step 6: Commit** `add economy config and upgrade purchase and stat application math`

---

### Task A3: GameSession glue and run-end banking

**Files:**
- Create: `Assets/Scripts/Meta/GameSession.cs`
- Modify: `Assets/Scripts/Run/RunManager.cs`
- Modify: `Assets/Scripts/Vehicle/VehicleController.cs`
- Test: `Assets/Tests/PlayMode/ProgressionPlayTests.cs` (new)

**Interfaces:**
- Consumes: SaveStore, Upgrades, EconomyConfig, Wallet, RunManager events
- Produces:
  - `static class GameSession`: `static SaveModel Save` (lazy `SaveStore.Load(PathOverride ?? SaveStore.DefaultPath)`), `static string PathOverride` (tests), `static EconomyConfig Economy` (loaded from `Resources.Load<EconomyConfig>("Economy")` — move/copy `Economy.asset` into `Assets/Resources/Economy.asset` via DefaultAssetsBuilder), `static void Persist()`, `static void Reset()` (nulls cached save so next access reloads; for tests)
  - `VehicleController`: in `Awake`, if `GameSession.Economy != null` replace `stats = Upgrades.ApplyTo(stats, GameSession.Save, GameSession.Economy)`
  - `RunManager`: when entering Results — `GameSession.Save.coins += Wallet.Coins;` `GameSession.Save.bestDistanceM = Mathf.Max(best, DistanceM);` `GameSession.Persist();` expose `public bool IsNewBest` set during that banking.

PlayMode test: set `GameSession.PathOverride` to temp file, `GameSession.Reset()`, spawn car+RunManager on flat ground, force some `Wallet.Coins`, call `run.NotifyCrash()`, wait for Results, assert save file contains banked coins and best distance; also assert `VehicleController` stats clone has upgraded brakeMaxTemp when save has a Brakes tier (write save with tier 1 first, respawn car, check `vc.stats.brakeMaxTemp > baseline`). Full test code to be authored by implementer following these assertions — keep the PlayModeCleanup base class pattern and TearDown `GameSession.PathOverride = null; GameSession.Reset();`.

- [ ] Steps: failing PlayMode test → implement → PlayMode 9 passing → EditMode still 41 → commit `add game session save banking and upgrade application at spawn`

---

### Task A4: Menu scene with garage

**Files:**
- Create: `Assets/Scripts/UI/MenuController.cs`
- Create: `Assets/Editor/MenuBuilder.cs`
- Create (generated): `Assets/Scenes/Menu.unity`
- Modify: `Assets/Editor/TerrainAssetsBuilder.cs` (register Menu scene in build settings at index 0)

**Interfaces:**
- Consumes: GameSession, Upgrades, EconomyConfig, SceneManager
- Produces:
  - `MenuController : MonoBehaviour` public refs: `coinText`, `bestText`, per-track: `tierText[4]`, `costText[4]`, `buyButtons[4]` (arrays), `playButton`. `Start` refreshes labels from `GameSession.Save`; buy button click → `Upgrades.Buy` + `GameSession.Persist()` + refresh; PLAY → `SceneManager.LoadScene("Run")`.
  - `MenuBuilder.Build()` menu item `Downshift/Build/Menu Scene`: canvas (same scaler pattern as HudBuilder), title label "DOWNSHIFT", coin + best labels, 4 rows (track name, tier, cost, BUY button with persistent listener pattern is NOT possible for parameterized calls — wire buy buttons via a small serialized helper: give MenuController a public method `BuyTrack(int track)` and use `UnityEditor.Events.UnityEventTools.AddIntPersistentListener(button.onClick, menu.BuyTrack, trackIndex)`), PLAY button → persistent listener to `MenuController.Play`.
  - Run scene's results panel gains a MENU button (modify HudBuilder): persistent listener to new `RunManager.ToMenu()` which `SceneManager.LoadScene("Menu")`.
  - Scene registered at build index 0 (Menu first, Run second — adjust RunManager.Restart if it relies on buildIndex: change `Restart()` and the R-key path to `SceneManager.LoadScene("Run")` by name for robustness).

Verification: MCP — load Menu scene, screenshot, check labels/buttons wired (YAML or live inspection); PlayMode: existing suite still green (Run scene unchanged except results MENU button).

- [ ] Steps: implement → rebuild Run + Menu scenes headlessly/MCP → screenshot both → EditMode 41 / PlayMode 9 green → commit `add menu scene with garage upgrades and scene flow wiring`

---

### Task A5: Full-loop PlayMode test

**Files:**
- Create: `Assets/Tests/PlayMode/FullLoopPlayTests.cs`

Scene-based test: load Menu scene via `EditorSceneManager.LoadSceneInPlayMode` (or `SceneManager.LoadScene` with build settings), assert MenuController present and labels non-empty; simulate `menu.Play()`; wait for Run scene load; assert VehicleController + RunManager alive; `run.NotifyCrash()`; wait Results; assert results panel active and save file updated; call `run.ToMenu()`; wait; assert back in Menu with updated coin label. Uses GameSession.PathOverride temp save. Follow PlayModeCleanup pattern plus scene restore (`SceneManager.LoadScene` back to the test's original scene is unnecessary — Unity test runner isolates, but destroy leaked DontDestroyOnLoad objects if any).

- [ ] Steps: write test → run → fix wiring issues it exposes → PlayMode 10 green → commit `add full menu run results menu loop playmode test`

---

### Task A6: Plan wrap-up

- [ ] Update ledger, re-run both suites, push, take MCP screenshots of Menu + Run + Results for the morning report.

## Out of Scope

- Vehicles beyond Hatchback (roster comes with content plan)
- Rewarded-ad double coins / continue (release plan; `RunManager` banking leaves the hook)
- Cloud save
