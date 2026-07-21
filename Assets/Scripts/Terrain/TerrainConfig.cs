using UnityEngine;

namespace Downshift
{
    [CreateAssetMenu(menuName = "Downshift/TerrainConfig")]
    public class TerrainConfig : ScriptableObject
    {
        public int seed = 42;
        public float baseGrade = 0.12f;
        public float gradePerMeter = 0.00004f;
        public float maxGrade = 0.45f;
        public float bumpAmplitudeMin = 0.4f;
        public float bumpAmplitudeMax = 2.5f;
        public float bumpAmplitudeRampMeters = 2000f;
        public float bumpFrequency = 0.035f;
        public float pointSpacing = 2f;
        public int pointsPerChunk = 64;
        public float maxLocalRise = 0.25f;
    }
}
