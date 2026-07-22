using UnityEngine;

namespace Downshift
{
    public enum UpgradeTrack
    {
        Brakes = 0,
        Radiator = 1,
        Gearbox = 2,
        Tires = 3
    }

    public static class Upgrades
    {
        public static int CostFor(int currentTier, EconomyConfig c)
        {
            if (currentTier >= c.maxTier) return -1;
            return Mathf.RoundToInt(c.baseUpgradeCost * Mathf.Pow(c.costGrowth, currentTier));
        }

        public static bool CanBuy(SaveModel s, UpgradeTrack t, EconomyConfig c)
        {
            var tier = s.upgradeTiers[(int)t];
            var cost = CostFor(tier, c);
            return cost >= 0 && s.coins >= cost;
        }

        public static bool Buy(SaveModel s, UpgradeTrack t, EconomyConfig c)
        {
            if (!CanBuy(s, t, c)) return false;
            var tier = s.upgradeTiers[(int)t];
            var cost = CostFor(tier, c);
            s.coins -= cost;
            s.upgradeTiers[(int)t] = tier + 1;
            return true;
        }

        public static VehicleStats ApplyTo(VehicleStats baseStats, SaveModel s, EconomyConfig c)
        {
            var stats = Object.Instantiate(baseStats);

            var brakesTier = s.upgradeTiers[(int)UpgradeTrack.Brakes];
            var brakesFactor = 1f + brakesTier * c.brakesCapacityPerTier;
            stats.brakeMaxTemp *= brakesFactor;
            stats.brakeFadeStartTemp *= brakesFactor;
            stats.brakeReengageTemp *= brakesFactor;

            var radiatorTier = s.upgradeTiers[(int)UpgradeTrack.Radiator];
            var radiatorFactor = 1f + radiatorTier * c.radiatorCoolPerTier;
            stats.engineCoolPerSecond *= radiatorFactor;
            stats.engineHeatAtRedline /= radiatorFactor;

            var gearboxTier = s.upgradeTiers[(int)UpgradeTrack.Gearbox];
            stats.engineBrakeTorque *= 1f + gearboxTier * c.gearboxEngineBrakePerTier;

            var tiresTier = s.upgradeTiers[(int)UpgradeTrack.Tires];
            var tiresFactor = 1f + tiresTier * c.tiresGripPerTier;
            stats.wheelFriction *= tiresFactor;
            stats.suspensionDamping = Mathf.Min(stats.suspensionDamping * tiresFactor, 1f);

            return stats;
        }
    }
}
