using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Cinemachine;
using Downshift;

public class SpeedCameraPlayTests : PlayModeCleanup
{
    [UnityTest]
    public IEnumerator CameraZoomsOutWithSpeed()
    {
        var camGo = new GameObject("MainCam");
        camGo.tag = "MainCamera";
        camGo.AddComponent<Camera>().orthographic = true;
        camGo.AddComponent<CinemachineBrain>();

        var ground = new GameObject("Ground");
        var col = ground.AddComponent<EdgeCollider2D>();
        var pts = new Vector2[80];
        for (int i = 0; i < pts.Length; i++)
            pts[i] = new Vector2(i * 4f, -i * 4f * 0.25f);
        col.points = pts;

#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
#else
        GameObject prefab = null;
#endif
        var car = Object.Instantiate(prefab, new Vector3(2f, 1.5f, 0f), Quaternion.identity);
        var vc = car.GetComponent<VehicleController>();

        var cineGo = new GameObject("CM Follow");
        var cine = cineGo.AddComponent<CinemachineCamera>();
        cine.Follow = car.transform;
        cineGo.AddComponent<CinemachinePositionComposer>();
        var lens = cine.Lens;
        lens.OrthographicSize = 6f;
        cine.Lens = lens;
        var speedCam = cineGo.AddComponent<SpeedCamera>();
        speedCam.vehicle = vc;

        yield return null;
        float initialSize = cine.Lens.OrthographicSize;

        var rb = car.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(15f, -3f);
        for (int i = 0; i < 180; i++) yield return new WaitForFixedUpdate();

        Assert.Greater(vc.SpeedMs, 2f, "car should be moving for the zoom test");
        Assert.Greater(cine.Lens.OrthographicSize, initialSize + 0.3f, "camera should zoom out with speed");

        Object.Destroy(camGo); Object.Destroy(ground); Object.Destroy(car); Object.Destroy(cineGo);
    }
}
