using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VehicleRigPlayTests
{
    [TearDown]
    public void ResetVehicleInput()
    {
        Downshift.VehicleInput.KeyboardBrake = false;
        Downshift.VehicleInput.UiBrake = false;
    }

    [UnityTest]
    public IEnumerator CarLandsAndRollsDownSlope()
    {
        var slope = new GameObject("Slope");
        var col = slope.AddComponent<EdgeCollider2D>();
        var pts = new Vector2[50];
        for (int i = 0; i < pts.Length; i++)
            pts[i] = new Vector2(i * 4f, -i * 4f * 0.15f);
        col.points = pts;

#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
#else
        GameObject prefab = null;
#endif
        Assert.IsNotNull(prefab);
        var car = Object.Instantiate(prefab, new Vector3(2f, 2f, 0f), Quaternion.identity);
        var chassis = car.GetComponent<Rigidbody2D>();
        float startX = car.transform.position.x;

        for (int i = 0; i < 300; i++) yield return new WaitForFixedUpdate();

        Assert.IsTrue(float.IsFinite(car.transform.position.x));
        Assert.IsTrue(float.IsFinite(car.transform.position.y));
        Assert.Greater(car.transform.position.x, startX + 2f, "car should roll downhill");
        float tilt = Mathf.Abs(Mathf.DeltaAngle(car.transform.eulerAngles.z, 0f));
        Assert.Less(tilt, 60f, "car should stay upright on the slope");
        foreach (var joint in car.GetComponents<WheelJoint2D>())
        {
            Assert.IsNotNull(joint.connectedBody);
            Assert.Less(Vector2.Distance(joint.connectedBody.position, chassis.position), 3f, "wheel should stay attached");
        }
        Object.Destroy(slope);
        Object.Destroy(car);
    }

    [UnityTest]
    public IEnumerator BrakingSlowsCarAndHeatsBrakes()
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
        var vc = car.GetComponent<Downshift.VehicleController>();
        Assert.IsNotNull(vc);
        Assert.IsNotNull(vc.stats);

        Downshift.VehicleInput.KeyboardBrake = false;
        Downshift.VehicleInput.UiBrake = false;
        for (int i = 0; i < 240; i++) yield return new WaitForFixedUpdate();
        float freeSpeed = vc.SpeedMs;
        Assert.Greater(freeSpeed, 1f, "car should gather speed rolling free");

        Downshift.VehicleInput.KeyboardBrake = true;
        for (int i = 0; i < 180; i++) yield return new WaitForFixedUpdate();
        Assert.Less(vc.SpeedMs, freeSpeed * 0.7f, "braking should shed speed");
        Assert.Greater(vc.Brakes.Temp, 0.1f, "brake temp should rise while braking");
        Downshift.VehicleInput.KeyboardBrake = false;

        Assert.AreEqual(0, vc.CurrentGear);
        vc.GearUp();
        Assert.AreEqual(1, vc.CurrentGear);
        vc.GearDown();
        Assert.AreEqual(0, vc.CurrentGear);

        Object.Destroy(slope);
        Object.Destroy(car);
    }
}
