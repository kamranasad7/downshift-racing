using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;
using Downshift.EditorTools;

public class PausePlayTests : PlayModeCleanup
{
    [UnityTest]
    public IEnumerator TogglePausePausesAndResumesThenNoOpsAtResults()
    {
        var ground = new GameObject("Ground");
        var col = ground.AddComponent<EdgeCollider2D>();
        col.points = new[] { new Vector2(-50f, 0f), new Vector2(200f, 0f) };

#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
#else
        GameObject prefab = null;
#endif
        var car = Object.Instantiate(prefab, new Vector3(2f, 1.5f, 0f), Quaternion.identity);
        var vc = car.GetComponent<VehicleController>();
        var runGo = new GameObject("RunManager");
        var run = runGo.AddComponent<RunManager>();
        run.vehicle = vc;
        run.resultsDelay = 0.5f;
        car.GetComponentInChildren<RoofCrashDetector>().runManager = run;

        var hud = HudBuilder.Build(vc, run);
        yield return null;

        Assert.IsFalse(hud.pausePanel.activeSelf, "pause panel hidden at start");

        run.TogglePause();
        yield return null;
        Assert.AreEqual(0f, Time.timeScale, "pausing should freeze time scale");
        Assert.IsTrue(run.IsPaused);
        Assert.IsTrue(hud.pausePanel.activeSelf, "pause panel shows while paused");

        run.TogglePause();
        yield return null;
        Assert.AreEqual(1f, Time.timeScale, "resuming should restore time scale");
        Assert.IsFalse(run.IsPaused);
        Assert.IsFalse(hud.pausePanel.activeSelf, "pause panel hides after resume");

        run.NotifyCrash();
        float deadline = Time.time + 3f;
        while (run.State != RunState.Results && Time.time < deadline) yield return null;
        Assert.AreEqual(RunState.Results, run.State);

        run.TogglePause();
        yield return null;
        Assert.AreEqual(0f, Time.timeScale, "results should keep time scale frozen");
        Assert.IsFalse(run.IsPaused, "TogglePause should no-op outside Descending state");

        Object.Destroy(ground); Object.Destroy(car); Object.Destroy(runGo);
        Object.Destroy(hud.gameObject);
    }
}
