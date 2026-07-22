using NUnit.Framework;
using UnityEngine;
using Downshift;

public abstract class PlayModeCleanup
{
    static readonly string[] SpawnNames =
        {
            "Ground", "Slope", "RunManager", "HUD", "EventSystem", "Terrain", "MainCam", "CM Follow", "Push",
            "Canvas", "Main Camera", "Input", "Hatchback", "Sky", "FarHills", "NearHills"
        };

    [TearDown]
    public void SweepScene()
    {
        VehicleInput.KeyboardBrake = false;
        VehicleInput.UiBrake = false;
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (go == null || go.transform.parent != null) continue;
            bool match = go.name.EndsWith("(Clone)");
            foreach (var n in SpawnNames)
                if (go.name == n) match = true;
            if (match) Object.Destroy(go);
        }
    }
}
