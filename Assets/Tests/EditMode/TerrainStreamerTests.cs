using NUnit.Framework;
using UnityEngine;
using Downshift;

public class TerrainStreamerTests
{
    [Test]
    public void ChunkIndexAtComputesFromSpacing()
    {
        var c = ScriptableObject.CreateInstance<TerrainConfig>();
        c.pointSpacing = 2f;
        c.pointsPerChunk = 64;
        Assert.AreEqual(0, TerrainStreamer.ChunkIndexAt(0f, c));
        Assert.AreEqual(0, TerrainStreamer.ChunkIndexAt(127.9f, c));
        Assert.AreEqual(1, TerrainStreamer.ChunkIndexAt(128f, c));
        Assert.AreEqual(3, TerrainStreamer.ChunkIndexAt(500f, c));
    }
}
