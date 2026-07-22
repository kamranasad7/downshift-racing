using NUnit.Framework;
using UnityEngine;
using Downshift;

public class UpgradesTests
{
    EconomyConfig C()
    {
        var c = ScriptableObject.CreateInstance<EconomyConfig>();
        c.baseUpgradeCost = 100;
        c.costGrowth = 2f;
        c.maxTier = 3;
        c.brakesCapacityPerTier = 0.5f;
        return c;
    }

    [Test]
    public void CostDoublesPerTier()
    {
        var c = C();
        Assert.AreEqual(100, Upgrades.CostFor(0, c));
        Assert.AreEqual(200, Upgrades.CostFor(1, c));
        Assert.AreEqual(400, Upgrades.CostFor(2, c));
        Assert.AreEqual(-1, Upgrades.CostFor(3, c));
    }

    [Test]
    public void BuyDeductsAndIncrements()
    {
        var c = C();
        var s = new SaveModel { coins = 250 };
        Assert.IsTrue(Upgrades.Buy(s, UpgradeTrack.Brakes, c));
        Assert.AreEqual(150, s.coins);
        Assert.AreEqual(1, s.upgradeTiers[0]);
        Assert.IsFalse(Upgrades.Buy(s, UpgradeTrack.Brakes, c));
        Assert.AreEqual(150, s.coins);
    }

    [Test]
    public void BuyRefusesAtMaxTier()
    {
        var c = C();
        var s = new SaveModel { coins = 999999 };
        s.upgradeTiers[0] = 3;
        Assert.IsFalse(Upgrades.Buy(s, UpgradeTrack.Brakes, c));
    }

    [Test]
    public void ApplyToScalesBrakesWithoutMutatingBase()
    {
        var c = C();
        var baseStats = ScriptableObject.CreateInstance<VehicleStats>();
        baseStats.brakeMaxTemp = 100f;
        var s = new SaveModel();
        s.upgradeTiers[0] = 2;
        var applied = Upgrades.ApplyTo(baseStats, s, c);
        Assert.AreEqual(200f, applied.brakeMaxTemp, 0.001f);
        Assert.AreEqual(100f, baseStats.brakeMaxTemp, 0.001f);
    }
}
