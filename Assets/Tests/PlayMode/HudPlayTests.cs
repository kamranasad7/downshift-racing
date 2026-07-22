using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;
using Downshift.EditorTools;

public class HudPlayTests : PlayModeCleanup
{
    [UnityTest]
    public IEnumerator HudTracksVehicleAndShowsResults()
    {
        var ground = new GameObject("Ground");
        var col = ground.AddComponent<EdgeCollider2D>();
        var pts = new Vector2[60];
        for (int i = 0; i < pts.Length; i++)
            pts[i] = new Vector2(i * 4f, -i * 4f * 0.2f);
        col.points = pts;

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

        Assert.IsFalse(hud.resultsPanel.activeSelf, "results panel hidden at start");
        Assert.AreEqual("G1", hud.gearText.text);

        VehicleInput.KeyboardBrake = true;
        for (int i = 0; i < 120; i++) yield return new WaitForFixedUpdate();
        VehicleInput.KeyboardBrake = false;
        Assert.Greater(hud.brakeFill.fillAmount, 0.0005f, "brake gauge should show heat");
        Assert.AreEqual($"{Mathf.FloorToInt(run.DistanceM)} m", hud.distanceText.text);

        vc.GearUp();
        yield return null;
        Assert.AreEqual("G2", hud.gearText.text);

        run.NotifyCrash();
        float deadline = Time.time + 3f;
        while (run.State != RunState.Results && Time.time < deadline) yield return null;
        yield return null;
        Assert.IsTrue(hud.resultsPanel.activeSelf, "results panel shows on results state");

        Object.Destroy(ground); Object.Destroy(car); Object.Destroy(runGo);
        Object.Destroy(hud.gameObject);
    }
}
