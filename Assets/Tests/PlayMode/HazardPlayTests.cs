using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;

public class HazardPlayTests : PlayModeCleanup
{
    GameObject BuildFlatGround(float y)
    {
        var ground = new GameObject("Ground");
        var col = ground.AddComponent<EdgeCollider2D>();
        col.points = new[] { new Vector2(-10f, y), new Vector2(200f, y) };
        return ground;
    }

    (GameObject car, RunManager run, Rigidbody2D rb) SpawnCarWithRocks(float rocksX)
    {
#if UNITY_EDITOR
        var carPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
        var rocksPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Rocks.prefab");
#else
        GameObject carPrefab = null, rocksPrefab = null;
#endif
        Assert.IsNotNull(carPrefab);
        Assert.IsNotNull(rocksPrefab);

        var car = Object.Instantiate(carPrefab, new Vector3(0f, 1.2f, 0f), Quaternion.identity);
        Object.Instantiate(rocksPrefab, new Vector3(rocksX, 0f, 0f), Quaternion.identity);

        var runGo = new GameObject("RunManager");
        var run = runGo.AddComponent<RunManager>();
        run.vehicle = car.GetComponent<VehicleController>();
        car.GetComponentInChildren<RoofCrashDetector>().runManager = run;

        return (car, run, car.GetComponent<Rigidbody2D>());
    }

    [UnityTest]
    public IEnumerator SlowOverRocksStaysOnRoad()
    {
        var ground = BuildFlatGround(0f);
        var (car, run, rb) = SpawnCarWithRocks(15f);

        float deadline = Time.time + 20f;
        while (car.transform.position.x < 20f && Time.time < deadline && run.State == RunState.Descending)
        {
            rb.AddForce(Vector2.right * 8000f);
            if (rb.linearVelocity.x > 2f) rb.linearVelocity = new Vector2(2f, rb.linearVelocity.y);
            yield return new WaitForFixedUpdate();
        }

        Assert.AreEqual(RunState.Descending, run.State, "slow rock crossing must not crash");
        Assert.Greater(car.transform.position.x, 20f, "crawling car should clear the rocks");

        Object.Destroy(ground); Object.Destroy(car); Object.Destroy(run.gameObject);
    }

    [UnityTest]
    public IEnumerator FastIntoRocksDestabilizesOrCrashes()
    {
        var ground = BuildFlatGround(0f);
        var (car, run, rb) = SpawnCarWithRocks(15f);

        bool destabilized = false;
        float maxAngVel = 0f;
        float deadline = Time.time + 4f;
        while (Time.time < deadline)
        {
            rb.linearVelocity = new Vector2(14f, rb.linearVelocity.y);
            yield return new WaitForFixedUpdate();
            maxAngVel = Mathf.Max(maxAngVel, Mathf.Abs(rb.angularVelocity));
            if (Mathf.Abs(rb.angularVelocity) > 200f) destabilized = true;
            if (run.State != RunState.Descending) break;
        }

        Assert.IsTrue(run.State != RunState.Descending || destabilized,
            $"fast rock impact should crash or destabilize the car (maxAngVel={maxAngVel}, state={run.State})");

        Object.Destroy(ground); Object.Destroy(car); Object.Destroy(run.gameObject);
    }
}
