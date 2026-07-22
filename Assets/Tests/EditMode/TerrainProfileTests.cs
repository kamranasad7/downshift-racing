using NUnit.Framework;
using UnityEngine;
using Downshift;

public class TerrainProfileTests
{
    TerrainConfig C()
    {
        var c = ScriptableObject.CreateInstance<TerrainConfig>();
        c.seed = 42;
        c.baseGrade = 0.12f;
        c.gradePerMeter = 0.00004f;
        c.maxGrade = 0.45f;
        c.bumpAmplitudeMin = 0.4f;
        c.bumpAmplitudeMax = 2.5f;
        c.bumpAmplitudeRampMeters = 2000f;
        c.bumpFrequency = 0.035f;
        c.pointSpacing = 2f;
        c.pointsPerChunk = 64;
        c.maxLocalRise = 0.25f;
        return c;
    }

    [Test]
    public void Deterministic()
    {
        var c = C();
        Assert.AreEqual(TerrainProfile.Height(137.5f, c), TerrainProfile.Height(137.5f, c), 0f);
    }

    [Test]
    public void ChunksAreContinuous()
    {
        var c = C();
        var a = TerrainProfile.ChunkHeights(3, c);
        var b = TerrainProfile.ChunkHeights(4, c);
        Assert.AreEqual(a[a.Length - 1], b[0], 0.0001f);
    }

    [Test]
    public void NetDescentOverEveryChunk()
    {
        var c = C();
        for (int chunk = 0; chunk < 40; chunk++)
        {
            var h = TerrainProfile.ChunkHeights(chunk, c);
            Assert.Less(h[h.Length - 1], h[0]);
        }
        for (int chunk = 64; chunk < 75; chunk++)
        {
            var h = TerrainProfile.ChunkHeights(chunk, c);
            Assert.Less(h[h.Length - 1], h[0]);
        }
    }

    [Test]
    public void LocalRisesBounded()
    {
        var c = C();
        for (int chunk = 0; chunk < 40; chunk++)
        {
            var h = TerrainProfile.ChunkHeights(chunk, c);
            for (int i = 1; i < h.Length; i++)
                Assert.LessOrEqual(h[i] - h[i - 1], c.maxLocalRise + 0.0001f);
        }
        for (int chunk = 64; chunk < 75; chunk++)
        {
            var h = TerrainProfile.ChunkHeights(chunk, c);
            for (int i = 1; i < h.Length; i++)
                Assert.LessOrEqual(h[i] - h[i - 1], c.maxLocalRise + 0.0001f);
        }
    }

    [Test]
    public void GradeSteepensWithDistanceUpToCap()
    {
        var c = C();
        float near = TerrainProfile.Height(0f, c) - TerrainProfile.Height(500f, c);
        float far = TerrainProfile.Height(8000f, c) - TerrainProfile.Height(8500f, c);
        Assert.Greater(far, near);
    }

    [Test]
    public void ZeroGradePerMeterIsFiniteAndDescending()
    {
        var c = C();
        c.gradePerMeter = 0f;
        c.baseGrade = c.maxGrade + 0.1f;

        foreach (var x in new[] { 0f, 100f, 1000f, 5000f, 20000f })
            Assert.IsTrue(float.IsFinite(TerrainProfile.Height(x, c)));

        for (int chunk = 0; chunk < 3; chunk++)
        {
            var h = TerrainProfile.ChunkHeights(chunk, c);
            Assert.Less(h[h.Length - 1], h[0]);
        }
    }
}
