using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;

public class PickupPlayTests : PlayModeCleanup
{
    [UnityTest]
    public IEnumerator PickupsCollectAndDontFalseCrash()
    {
        var ground = new GameObject("Ground");
        var col = ground.AddComponent<EdgeCollider2D>();
        col.points = new[] { new Vector2(-10f, 0f), new Vector2(300f, 0f) };

#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
        var coinPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Coin.prefab");
        var coolantPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Coolant.prefab");
        var stationPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Station.prefab");
#else
        GameObject prefab = null, coinPrefab = null, coolantPrefab = null, stationPrefab = null;
#endif
        Assert.IsNotNull(coinPrefab);
        Assert.IsNotNull(coolantPrefab);
        Assert.IsNotNull(stationPrefab);

        var car = Object.Instantiate(prefab, new Vector3(0f, 1.2f, 0f), Quaternion.identity);
        var vc = car.GetComponent<VehicleController>();
        var runGo = new GameObject("RunManager");
        var run = runGo.AddComponent<RunManager>();
        run.vehicle = vc;
        car.GetComponentInChildren<RoofCrashDetector>().runManager = run;

        var c1 = Object.Instantiate(coinPrefab, new Vector3(8f, 1.0f, 0f), Quaternion.identity);
        var c2 = Object.Instantiate(coolantPrefab, new Vector3(14f, 1.2f, 0f), Quaternion.identity);
        var c3 = Object.Instantiate(stationPrefab, new Vector3(25f, 1.5f, 0f), Quaternion.identity);

        yield return null;
        int coinsBefore = Wallet.Coins;

        var rb = car.GetComponent<Rigidbody2D>();
        VehicleInput.KeyboardBrake = true;
        rb.linearVelocity = new Vector2(10f, 0f);
        for (int i = 0; i < 50; i++) yield return new WaitForFixedUpdate();
        VehicleInput.KeyboardBrake = false;
        Assert.Greater(vc.Brakes.Temp, 0.01f, "setup: brakes should have heat before pickups");

        float deadline = Time.time + 12f;
        while (car.transform.position.x < 30f && Time.time < deadline)
        {
            if (rb.linearVelocity.x < 6f) rb.linearVelocity = new Vector2(6f, rb.linearVelocity.y);
            yield return new WaitForFixedUpdate();
        }

        Assert.Greater(car.transform.position.x, 30f, "car should traverse all pickups");
        Assert.AreEqual(coinsBefore + 1, Wallet.Coins, "coin should collect exactly once");
        Assert.AreEqual(0f, vc.Brakes.Temp, 0.001f, "station should fully cool brakes");
        Assert.Less(vc.EngineTemp, 20f, "station cooling should leave engine far from hot despite post-station reheat");
        Assert.AreEqual(RunState.Descending, run.State, "pickup triggers must not false-crash the roof detector");

        Object.Destroy(ground); Object.Destroy(car); Object.Destroy(runGo);
        Object.Destroy(c1); Object.Destroy(c2); Object.Destroy(c3);
    }
}
