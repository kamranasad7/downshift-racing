using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;

public class ProgressionPlayTests : PlayModeCleanup
{
    GameObject BuildFlatGround(float y)
    {
        var ground = new GameObject("Ground");
        var col = ground.AddComponent<EdgeCollider2D>();
        col.points = new[] { new Vector2(-50f, y), new Vector2(200f, y) };
        return ground;
    }

    GameObject SpawnHatchback(Vector3 pos)
    {
#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
#else
        GameObject prefab = null;
#endif
        return Object.Instantiate(prefab, pos, Quaternion.identity);
    }

    [UnityTest]
    public IEnumerator BankingPersistsCoinsAndBest()
    {
        var tempPath = GameSession.PathOverride;

        var ground = BuildFlatGround(0f);
        var car = SpawnHatchback(new Vector3(4f, 1.5f, 0f));
        var runGo = new GameObject("RunManager");
        var run = runGo.AddComponent<RunManager>();
        run.vehicle = car.GetComponent<VehicleController>();
        run.resultsDelay = 0.3f;
        car.GetComponentInChildren<RoofCrashDetector>().runManager = run;

        yield return null;
        Wallet.Coins = 7;

        float startX = car.transform.position.x;
        float moveDeadline = Time.time + 3f;
        while (car.transform.position.x < startX + 0.05f && Time.time < moveDeadline)
            yield return new WaitForFixedUpdate();

        run.NotifyCrash();

        float deadline = Time.time + 5f;
        while (run.State != RunState.Results && Time.time < deadline)
            yield return null;

        Assert.AreEqual(RunState.Results, run.State);
        Assert.Greater(run.DistanceM, 0f);
        Assert.IsTrue(run.IsNewBest);

        var saved = SaveStore.Load(tempPath);
        Assert.AreEqual(7, saved.coins);
        Assert.Greater(saved.bestDistanceM, 0f);
        Assert.Less(saved.bestDistanceM, 3f);

        Object.Destroy(ground); Object.Destroy(car); Object.Destroy(runGo);
        if (File.Exists(tempPath)) File.Delete(tempPath);
    }

    [UnityTest]
    public IEnumerator UpgradeAppliesToSpawnedVehicle()
    {
        var tempPath = GameSession.PathOverride;

        var save = new SaveModel();
        save.upgradeTiers[(int)UpgradeTrack.Brakes] = 2;
        SaveStore.Save(save, tempPath);
        GameSession.Reset();

        var ground = BuildFlatGround(0f);
        var car = SpawnHatchback(new Vector3(0f, 1.5f, 0f));
        var vc = car.GetComponent<VehicleController>();

#if UNITY_EDITOR
        var baseline = UnityEditor.AssetDatabase.LoadAssetAtPath<VehicleStats>("Assets/Data/Hatchback.asset");
#else
        VehicleStats baseline = null;
#endif

        Assert.Greater(vc.stats.brakeMaxTemp, baseline.brakeMaxTemp);

        yield return null;

        Object.Destroy(ground); Object.Destroy(car);
        if (File.Exists(tempPath)) File.Delete(tempPath);
    }
}
