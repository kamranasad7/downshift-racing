using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Downshift;

public class HazardPlacerTests
{
    TerrainConfig C()
    {
        var c = ScriptableObject.CreateInstance<TerrainConfig>();
        c.seed = 7;
        c.pointSpacing = 2f;
        c.pointsPerChunk = 64;
        c.stationEveryMeters = 900f;
        c.coolantEveryMeters = 180f;
        c.coinEveryMeters = 25f;
        c.pickupSparsityRampMeters = 4000f;
        c.pickupSparsityMaxMultiplier = 2.5f;
        c.hazardEveryMeters = 350f;
        c.hazardMinSpacingMeters = 150f;
        c.hazardRampMeters = 3000f;
        c.hazardStartMeters = 250f;
        c.signLeadMeters = 25f;
        c.stationClearMeters = 60f;
        return c;
    }

    [Test]
    public void Deterministic()
    {
        var c = C();
        var a = HazardPlacer.PlacementsForChunk(5, c);
        var b = HazardPlacer.PlacementsForChunk(5, c);
        Assert.AreEqual(a.Count, b.Count);
        for (int i = 0; i < a.Count; i++) Assert.AreEqual(a[i], b[i]);
    }

    [Test]
    public void SpacingShrinksWithDistance()
    {
        var c = C();
        int near = 0, far = 0;
        for (int i = 0; i < 10; i++) near += HazardPlacer.PlacementsForChunk(i, c).Count;
        for (int i = 60; i < 70; i++) far += HazardPlacer.PlacementsForChunk(i, c).Count;
        Assert.Greater(far, near);
    }

    [Test]
    public void NothingBeforeHazardStartMeters()
    {
        var c = C();
        for (int i = 0; i < 5; i++)
            foreach (var h in HazardPlacer.PlacementsForChunk(i, c))
                Assert.GreaterOrEqual(h.x, c.hazardStartMeters);
    }

    [Test]
    public void HazardsNearStationsAreDropped()
    {
        var c = C();
        c.stationClearMeters = 500f;
        int withClearance = 0;
        for (int i = 0; i < 10; i++) withClearance += HazardPlacer.PlacementsForChunk(i, c).Count;

        var c2 = C();
        c2.stationClearMeters = 0f;
        int withoutClearance = 0;
        for (int i = 0; i < 10; i++) withoutClearance += HazardPlacer.PlacementsForChunk(i, c2).Count;

        Assert.Less(withClearance, withoutClearance);

        var stations = new List<float>();
        for (int k = -1; k <= 10; k++)
        {
            if (k < 0) continue;
            foreach (var p in PickupPlacer.PlacementsForChunk(k, c))
                if (p.kind == PickupKind.Station) stations.Add(p.x);
        }
        for (int i = 0; i < 10; i++)
            foreach (var h in HazardPlacer.PlacementsForChunk(i, c))
                foreach (var s in stations)
                    Assert.GreaterOrEqual(Mathf.Abs(h.x - s), c.stationClearMeters);
    }

    [Test]
    public void SignLeadDistanceIsExact()
    {
        var c = C();
        int chunkIndex = 3;
        float len = c.pointsPerChunk * c.pointSpacing;
        float start = chunkIndex * len;
        float end = start + len;

        var expected = new List<float>();
        foreach (var h in HazardPlacer.PlacementsForChunk(chunkIndex, c))
        {
            float s = h.x - c.signLeadMeters;
            if (s >= start && s < end) expected.Add(s);
        }
        foreach (var h in HazardPlacer.PlacementsForChunk(chunkIndex + 1, c))
        {
            float s = h.x - c.signLeadMeters;
            if (s >= start && s < end) expected.Add(s);
        }
        expected.Sort();

        var actual = HazardPlacer.SignsForChunk(chunkIndex, c);
        Assert.AreEqual(expected.Count, actual.Count);
        for (int i = 0; i < expected.Count; i++) Assert.AreEqual(expected[i], actual[i], 0.0001f);
    }

    [Test]
    public void SignCanFallInPreviousChunkFromHazard()
    {
        var c = C();
        float len = c.pointsPerChunk * c.pointSpacing;
        bool found = false;
        for (int k = 0; k < 200 && !found; k++)
        {
            float start = k * len;
            foreach (var h in HazardPlacer.PlacementsForChunk(k, c))
            {
                float signX = h.x - c.signLeadMeters;
                if (signX < start)
                {
                    var signs = HazardPlacer.SignsForChunk(k - 1, c);
                    bool contains = false;
                    foreach (var s in signs)
                        if (Mathf.Abs(s - signX) < 0.0001f) contains = true;
                    Assert.IsTrue(contains, "boundary sign missing from previous chunk");
                    found = true;
                    break;
                }
            }
        }
        Assert.IsTrue(found, "no chunk-boundary sign case found within scanned range");
    }

    [Test]
    public void PlacementsSortedAndInsideChunk()
    {
        var c = C();
        float len = c.pointsPerChunk * c.pointSpacing;
        var pts = HazardPlacer.PlacementsForChunk(4, c);
        float prev = 4 * len;
        foreach (var p in pts)
        {
            Assert.GreaterOrEqual(p.x, 4 * len);
            Assert.Less(p.x, 5 * len);
            Assert.GreaterOrEqual(p.x, prev);
            prev = p.x;
        }
    }
}
