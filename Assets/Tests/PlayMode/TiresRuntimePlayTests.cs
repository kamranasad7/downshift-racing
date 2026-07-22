using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;

public class TiresRuntimePlayTests : PlayModeCleanup
{
    [UnityTest]
    public IEnumerator TiresTierRaisesRuntimeWheelFriction()
    {
        var save = new SaveModel();
        save.upgradeTiers[(int)UpgradeTrack.Tires] = 3;
        SaveStore.Save(save, GameSession.PathOverride);
        GameSession.Reset();

#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
        var baseline = UnityEditor.AssetDatabase.LoadAssetAtPath<VehicleStats>("Assets/Data/Hatchback.asset");
#else
        GameObject prefab = null;
        VehicleStats baseline = null;
#endif
        var car = Object.Instantiate(prefab, new Vector3(0f, 1.5f, 0f), Quaternion.identity);
        var vc = car.GetComponent<VehicleController>();
        Assert.IsNotNull(vc);

        var wheelCol = car.GetComponentInChildren<WheelJoint2D>().connectedBody.GetComponent<Collider2D>();
        Assert.IsNotNull(wheelCol.sharedMaterial, "wheel collider should have a runtime physics material");
        Assert.Greater(wheelCol.sharedMaterial.friction, baseline.wheelFriction,
            "tires upgrade should raise runtime wheel friction above the base asset value");

        yield return null;

        Object.Destroy(car);
    }
}
