using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;

public class HeatEffectsPlayTests : PlayModeCleanup
{
    [UnityTest]
    public IEnumerator BrakingHeatTriggersEmberEmissionAndColdEngineHasNoSmoke()
    {
        var slope = new GameObject("Slope");
        var col = slope.AddComponent<EdgeCollider2D>();
        var pts = new Vector2[80];
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
        Assert.IsNotNull(vc);
        vc.stats = Object.Instantiate(vc.stats);
        vc.stats.brakeFadeStartTemp = 0.01f;

        var heat = car.GetComponentInChildren<HeatEffects>();
        Assert.IsNotNull(heat, "prefab should wire up HeatEffects");

        VehicleInput.KeyboardBrake = false;
        VehicleInput.UiBrake = false;
        for (int i = 0; i < 60; i++) yield return new WaitForFixedUpdate();

        VehicleInput.KeyboardBrake = true;
        for (int i = 0; i < 30; i++) yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Greater(vc.Brakes.Temp, vc.stats.brakeFadeStartTemp, "brake temp should exceed fade start");
        Assert.Greater(heat.brakeEmbersFront.emission.rateOverTime.constant, 0f, "front embers should emit while braking hot");
        Assert.Greater(heat.brakeEmbersRear.emission.rateOverTime.constant, 0f, "rear embers should emit while braking hot");
        Assert.AreEqual(0f, heat.engineSmoke.emission.rateOverTime.constant, "cold engine should not smoke");

        VehicleInput.KeyboardBrake = false;
        Object.Destroy(slope);
        Object.Destroy(car);
    }
}
