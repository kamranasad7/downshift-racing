using UnityEngine;

namespace Downshift
{
    [CreateAssetMenu(menuName = "Downshift/TerrainConfig")]
    public class TerrainConfig : ScriptableObject
    {
        [Tooltip("Random seed for the terrain shape. Same seed = same course every run.")]
        public int seed = 42;
        [Tooltip("Starting downhill steepness (rise/run). Higher = steeper from the very start.")]
        public float baseGrade = 0.12f;
        [Tooltip("How much steeper the slope gets per meter travelled. Higher = ramps up to the max grade faster.")]
        public float gradePerMeter = 0.00004f;
        [Tooltip("Steepest the slope can ever get. The difficulty ceiling for grade.")]
        public float maxGrade = 0.45f;
        [Tooltip("Bump height near the start (gentle). Higher = rougher early road.")]
        public float bumpAmplitudeMin = 0.4f;
        [Tooltip("Bump height once fully ramped up. Higher = bigger crests/dips deep in a run.")]
        public float bumpAmplitudeMax = 2.5f;
        [Tooltip("Distance over which bumps grow from min to max amplitude.")]
        public float bumpAmplitudeRampMeters = 2000f;
        [Tooltip("Bump frequency. Higher = more frequent, tighter bumps; lower = long rolling hills.")]
        public float bumpFrequency = 0.035f;
        [Tooltip("Distance between terrain mesh points. Smaller = smoother curves but more geometry.")]
        public float pointSpacing = 2f;
        [Tooltip("Mesh points per streamed chunk. Chunk length = this x pointSpacing.")]
        public int pointsPerChunk = 64;
        [Tooltip("Max upward step between adjacent points (stuck-proofing). Keeps any local rise gentle enough to clear.")]
        public float maxLocalRise = 0.25f;

        [Header("Pickups")]
        [Tooltip("Average meters between coins. Lower = more coins.")]
        public float coinEveryMeters = 25f;
        [Tooltip("Average meters between coolant pickups (partial cool). Lower = more frequent relief.")]
        public float coolantEveryMeters = 180f;
        [Tooltip("Average meters between service stations (full cool). Lower = more frequent full resets.")]
        public float stationEveryMeters = 900f;
        [Tooltip("Distance over which pickups get sparser (difficulty ramp).")]
        public float pickupSparsityRampMeters = 4000f;
        [Tooltip("How much rarer pickups get at max distance (e.g. 2.5 = 2.5x the base spacing).")]
        public float pickupSparsityMaxMultiplier = 2.5f;

        [Header("Hazards")]
        [Tooltip("Average meters between hazards near the start. Lower = more hazards.")]
        public float hazardEveryMeters = 350f;
        [Tooltip("Tightest hazard spacing deep in a run (they get denser with distance, down to this floor).")]
        public float hazardMinSpacingMeters = 150f;
        [Tooltip("Distance over which hazard spacing tightens from hazardEveryMeters to hazardMinSpacingMeters.")]
        public float hazardRampMeters = 3000f;
        [Tooltip("No hazards before this distance (tutorial grace period).")]
        public float hazardStartMeters = 250f;
        [Tooltip("How far ahead of each hazard its warning sign is placed.")]
        public float signLeadMeters = 25f;
        [Tooltip("Keep-clear radius around service stations so a hazard never spawns on top of one.")]
        public float stationClearMeters = 60f;
    }
}
