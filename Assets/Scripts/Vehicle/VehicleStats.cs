using UnityEngine;

namespace Downshift
{
    [CreateAssetMenu(menuName = "Downshift/VehicleStats")]
    public class VehicleStats : ScriptableObject
    {
        [Header("Body")]
        public float chassisMass = 1200f;
        public float wheelRadius = 0.35f;
        public float wheelFriction = 1.2f;
        public float suspensionFrequency = 4f;
        public float suspensionDamping = 0.7f;

        [Header("Drivetrain")]
        public float[] gearRatios = { 3.2f, 2.1f, 1.4f, 1.0f, 0.8f };
        public float finalDrive = 3.7f;
        public float redlineRpm = 6500f;
        public float engineBrakeTorque = 260f;

        [Header("Engine heat")]
        public float engineHeatPerRpmOverRedline = 0.02f;
        public float engineCoolPerSecond = 6f;
        public float engineMaxTemp = 100f;

        [Header("Brakes")]
        public float brakeTorque = 900f;
        public float brakeHeatPerSecondAtRef = 14f;
        public float brakeHeatRefSpeed = 20f;
        public float brakeCoolPerSecond = 4f;
        public float brakeFadeStartTemp = 60f;
        public float brakeMaxTemp = 100f;
        public float brakeReengageTemp = 45f;

        [Header("Misc")]
        public float creepForce = 150f;
    }
}
