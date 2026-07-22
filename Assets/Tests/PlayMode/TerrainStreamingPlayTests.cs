using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Downshift;

public class TerrainStreamingPlayTests : PlayModeCleanup
{
    [UnityTest]
    public IEnumerator CarDescendsStreamedTerrainWithoutSticking()
    {
        var config = ScriptableObject.CreateInstance<TerrainConfig>();
        config.pointsPerChunk = 16;

#if UNITY_EDITOR
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
        var shape = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.U2D.SpriteShape>("Assets/Art/TerrainShapeProfile.asset");
#else
        GameObject prefab = null; UnityEngine.U2D.SpriteShape shape = null;
#endif
        Assert.IsNotNull(prefab);
        Assert.IsNotNull(shape);

        var car = Object.Instantiate(prefab, new Vector3(4f, TerrainProfile.Height(4f, config) + 2f, 0f), Quaternion.identity);
        var terrainGo = new GameObject("Terrain");
        var streamer = terrainGo.AddComponent<TerrainStreamer>();
        streamer.config = config;
        streamer.target = car.transform;
        streamer.shapeProfile = shape;

        float lastX = car.transform.position.x;
        float minProgressWindow = float.MaxValue;
        for (int window = 0; window < 12; window++)
        {
            for (int i = 0; i < 250; i++) yield return new WaitForFixedUpdate();
            float x = car.transform.position.x;
            minProgressWindow = Mathf.Min(minProgressWindow, x - lastX);
            lastX = x;
            Assert.IsTrue(float.IsFinite(x));
            Assert.IsTrue(float.IsFinite(car.transform.position.y));
        }

        Assert.Greater(car.transform.position.x, 25f, "car should cover ground over 60s");
        Assert.Greater(minProgressWindow, 0.5f, "car should never stall in any 5s window");
        Assert.Greater(TerrainStreamer.ChunkIndexAt(car.transform.position.x, config), 0, "car should cross chunk boundaries");

        int chunkChildren = 0;
        foreach (Transform child in terrainGo.transform)
            if (child.GetComponent<UnityEngine.U2D.SpriteShapeController>() != null) chunkChildren++;
        Assert.AreEqual(streamer.activeChunks, chunkChildren, "chunk pool should stay at fixed size");

        Object.Destroy(car);
        Object.Destroy(terrainGo);
    }
}
