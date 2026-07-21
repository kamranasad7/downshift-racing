using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;

public class RunFlowPlayTests
{
    [TearDown]
    public void ResetInput()
    {
        VehicleInput.KeyboardBrake = false;
        VehicleInput.UiBrake = false;
    }

    GameObject BuildFlatGround(float y)
    {
        var ground = new GameObject("Ground");
        var col = ground.AddComponent<EdgeCollider2D>();
        col.points = new[] { new Vector2(-50f, y), new Vector2(200f, y) };
        return ground;
    }

    [UnityTest]
    public IEnumerator RoofContactTriggersCrashThenResults()
    {
        var ground = BuildFlatGround(0f);
#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
#else
        GameObject prefab = null;
#endif
        var car = Object.Instantiate(prefab, new Vector3(0f, 3f, 0f), Quaternion.Euler(0f, 0f, 170f));
        var runGo = new GameObject("RunManager");
        var run = runGo.AddComponent<RunManager>();
        run.vehicle = car.GetComponent<VehicleController>();
        run.resultsDelay = 0.5f;
        car.GetComponentInChildren<RoofCrashDetector>().runManager = run;

        RunState? firstEvent = null;
        run.StateChanged += s => { if (firstEvent == null) firstEvent = s; };

        float deadline = Time.time + 8f;
        while (run.State == RunState.Descending && Time.time < deadline)
            yield return new WaitForFixedUpdate();

        Assert.AreEqual(RunState.Crashed, run.State, "upside-down landing should crash");
        Assert.AreEqual(RunState.Crashed, firstEvent);

        deadline = Time.time + 3f;
        while (run.State != RunState.Results && Time.time < deadline)
            yield return null;
        Assert.AreEqual(RunState.Results, run.State, "crash should advance to results after delay");

        Object.Destroy(ground); Object.Destroy(car); Object.Destroy(runGo);
    }

    [UnityTest]
    public IEnumerator BlownEventDrivesBlownUpState()
    {
        var ground = BuildFlatGround(0f);
#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
#else
        GameObject prefab = null;
#endif
        var car = Object.Instantiate(prefab, new Vector3(0f, 1.5f, 0f), Quaternion.identity);
        var vc = car.GetComponent<VehicleController>();
        vc.stats = Object.Instantiate(vc.stats);
        vc.stats.engineMaxTemp = 0.01f;
        vc.stats.redlineRpm = 1f;
        var runGo = new GameObject("RunManager");
        var run = runGo.AddComponent<RunManager>();
        run.vehicle = vc;
        run.resultsDelay = 0.5f;
        car.GetComponentInChildren<RoofCrashDetector>().runManager = run;

        var slope = new GameObject("Push");
        car.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(8f, 0f);

        float deadline = Time.time + 8f;
        while (run.State == RunState.Descending && Time.time < deadline)
            yield return new WaitForFixedUpdate();

        Assert.AreEqual(RunState.BlownUp, run.State, "engine over-temp should blow up");
        Assert.IsTrue(vc.EngineBlown);

        Object.Destroy(ground); Object.Destroy(car); Object.Destroy(runGo); Object.Destroy(slope);
    }
}
