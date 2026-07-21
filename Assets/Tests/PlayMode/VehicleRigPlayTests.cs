using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VehicleRigPlayTests
{
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
        Assert.Greater(car.transform.position.x, startX + 5f, "car should roll downhill");
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
}
