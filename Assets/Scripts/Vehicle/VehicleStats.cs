using UnityEngine;

namespace Downshift
{
    [CreateAssetMenu(menuName = "Downshift/VehicleStats")]
    public class VehicleStats : ScriptableObject
    {
        [Header("Body")]
        [Tooltip("Vehicle weight. Heavier = more momentum, harder to stop, more stable in air.")]
        public float chassisMass = 1200f;
        [Tooltip("Wheel size in world units. Also sets how fast wheels spin per km/h (affects RPM).")]
        public float wheelRadius = 0.35f;
        [Tooltip("Tire grip on the road. Higher = less sliding, more traction. Driven by the TIRES upgrade.")]
        public float wheelFriction = 1.2f;
        [Tooltip("Suspension stiffness. Higher = firmer, snappier; lower = softer, bouncier.")]
        public float suspensionFrequency = 4f;
        [Tooltip("Suspension bounciness. Higher (toward 1) = less bounce; lower = more oscillation on landings.")]
        public float suspensionDamping = 0.7f;

        [Header("Drivetrain")]
        [Tooltip("Gear ratios, high to low. First = strongest engine braking/highest RPM; last = fastest cruise. Descending order.")]
        public float[] gearRatios = { 3.2f, 2.1f, 1.4f, 1.0f, 0.8f };
        [Tooltip("Overall gearing multiplier. Higher = higher RPM at any speed (raise this to make the RPM needle realistic).")]
        public float finalDrive = 3.7f;
        [Tooltip("Minimum displayed RPM at a standstill. Purely the idle floor for the gauge/sound.")]
        public float idleRpm = 900f;
        [Tooltip("Max safe RPM. Engine heats hard past this. Lower it to make over-revving a real threat sooner.")]
        public float redlineRpm = 6500f;
        [Tooltip("Engine-braking strength. Higher = engine slows the car more (and self-limits downhill speed lower). Driven by GEARBOX upgrade.")]
        public float engineBrakeTorque = 260f;

        [Header("Engine heat")]
        [Tooltip("RPM where engine heat starts. Below this the engine cools. Raise for a longer safe band.")]
        public float engineHeatStartRpm = 3000f;
        [Tooltip("Heat added per second at redline (quadratic ramp from start RPM). Higher = engine cooks faster near redline.")]
        public float engineHeatAtRedline = 12f;
        [Tooltip("How fast the engine cools when below the heat-start RPM. Driven by RADIATOR upgrade.")]
        public float engineCoolPerSecond = 6f;
        [Tooltip("Engine temperature that causes a blow-up (instant run over). Gauge is scaled to this.")]
        public float engineMaxTemp = 100f;

        [Header("Brakes")]
        [Tooltip("Braking force when brakes are cool. Higher = shorter stops.")]
        public float brakeTorque = 900f;
        [Tooltip("Brake heat added per second while braking, measured at the reference speed below. Higher = brakes fade sooner.")]
        public float brakeHeatPerSecondAtRef = 14f;
        [Tooltip("Speed at which brakeHeatPerSecondAtRef applies. Brake heat scales with actual speed / this value.")]
        public float brakeHeatRefSpeed = 20f;
        [Tooltip("How fast brakes cool when released. Higher = recover faster. Driven by BRAKES upgrade.")]
        public float brakeCoolPerSecond = 4f;
        [Tooltip("Brake temp where fade begins; braking force weakens above this. Driven by BRAKES upgrade.")]
        public float brakeFadeStartTemp = 60f;
        [Tooltip("Brake temp at full fade (near-zero force / runaway). Gauge is scaled to this.")]
        public float brakeMaxTemp = 100f;
        [Tooltip("After full fade, brakes stay dead until they cool back below this temp (hysteresis).")]
        public float brakeReengageTemp = 45f;

        [Header("Misc")]
        [Tooltip("Tiny constant forward force so the car never gets stuck. Imperceptible next to gravity.")]
        public float creepForce = 150f;
        [Tooltip("Torque applied when tapping gear buttons in mid-air (nose up/down). Higher = faster flip control.")]
        public float airTiltTorque = 3000f;
        [Tooltip("Gravity multiplier for this vehicle. Higher = heavier pull, faster descents.")]
        public float gravityScale = 1f;
    }
}
