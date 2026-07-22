using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;

public class StationReachPlayTests : PlayModeCleanup
{
    [UnityTest]
    public IEnumerator CarReachesStationAndCoolsFully()
    {
        var ground = new GameObject("Ground");
        var col = ground.AddComponent<EdgeCollider2D>();
        col.points = new[] { new Vector2(-10f, 0f), new Vector2(120f, 0f) };

#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
        var stationPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Station.prefab");
#else
        GameObject prefab = null, stationPrefab = null;
#endif
        Assert.IsNotNull(prefab);
        Assert.IsNotNull(stationPrefab);

        var car = Object.Instantiate(prefab, new Vector3(0f, 1.2f, 0f), Quaternion.identity);
        var vc = car.GetComponent<VehicleController>();
        var station = Object.Instantiate(stationPrefab, new Vector3(30f, 0f + 1.4f, 0f), Quaternion.identity);

        yield return null;

        var rb = car.GetComponent<Rigidbody2D>();
        VehicleInput.KeyboardBrake = true;
        rb.linearVelocity = new Vector2(10f, 0f);
        for (int i = 0; i < 50; i++) yield return new WaitForFixedUpdate();
        VehicleInput.KeyboardBrake = false;
        Assert.Greater(vc.Brakes.Temp, 0.01f, "setup: brakes should have heat before station");

        rb.linearVelocity = new Vector2(8f, rb.linearVelocity.y);
        float deadline = Time.time + 12f;
        while (car.transform.position.x < 40f && Time.time < deadline)
        {
            if (rb.linearVelocity.x < 5.5f) rb.linearVelocity = new Vector2(5.5f, rb.linearVelocity.y);
            yield return new WaitForFixedUpdate();
        }

        Assert.Greater(car.transform.position.x, 40f, "car should traverse the station");
        Assert.AreEqual(0f, vc.Brakes.Temp, 0.5f, "station should fully cool brakes");
        Assert.Less(vc.EngineTemp, 20f, "station cooling should leave engine far from hot despite post-station reheat");

        Object.Destroy(ground); Object.Destroy(car); Object.Destroy(station);
    }
}
