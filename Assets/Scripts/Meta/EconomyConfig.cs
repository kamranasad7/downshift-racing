using UnityEngine;

namespace Downshift
{
    [CreateAssetMenu(menuName = "Downshift/EconomyConfig")]
    public class EconomyConfig : ScriptableObject
    {
        public int baseUpgradeCost = 100;
        public float costGrowth = 1.6f;
        public int maxTier = 8;
        public float brakesCapacityPerTier = 0.10f;
        public float radiatorCoolPerTier = 0.10f;
        public float gearboxEngineBrakePerTier = 0.06f;
        public float tiresGripPerTier = 0.06f;
    }
}
