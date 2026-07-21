using NUnit.Framework;
using UnityEngine;
using Downshift;

public class PickupPlacerTests
{
    TerrainConfig C()
    {
        var c = ScriptableObject.CreateInstance<TerrainConfig>();
        c.seed = 7;
        c.pointSpacing = 2f;
        c.pointsPerChunk = 64;
        c.coinEveryMeters = 25f;
        c.coolantEveryMeters = 180f;
        c.stationEveryMeters = 900f;
        c.pickupSparsityRampMeters = 4000f;
        c.pickupSparsityMaxMultiplier = 2.5f;
        return c;
    }

    [Test]
    public void Deterministic()
    {
        var c = C();
        var a = PickupPlacer.PlacementsForChunk(5, c);
        var b = PickupPlacer.PlacementsForChunk(5, c);
        Assert.AreEqual(a.Count, b.Count);
        for (int i = 0; i < a.Count; i++) Assert.AreEqual(a[i], b[i]);
    }

    [Test]
    public void EarlyChunksContainCoinsAndOccasionalCoolant()
    {
        var c = C();
        int coins = 0, coolant = 0;
        for (int i = 0; i < 10; i++)
            foreach (var p in PickupPlacer.PlacementsForChunk(i, c))
            {
                if (p.kind == PickupKind.Coin) coins++;
                if (p.kind == PickupKind.Coolant) coolant++;
            }
        Assert.Greater(coins, 20);
        Assert.Greater(coolant, 2);
    }

    [Test]
    public void PickupsGetSparserWithDistance()
    {
        var c = C();
        int near = 0, far = 0;
        for (int i = 0; i < 10; i++) near += PickupPlacer.PlacementsForChunk(i, c).Count;
        for (int i = 60; i < 70; i++) far += PickupPlacer.PlacementsForChunk(i, c).Count;
        Assert.Less(far, near);
    }

    [Test]
    public void PlacementsSortedAndInsideChunk()
    {
        var c = C();
        float len = c.pointsPerChunk * c.pointSpacing;
        var pts = PickupPlacer.PlacementsForChunk(4, c);
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
