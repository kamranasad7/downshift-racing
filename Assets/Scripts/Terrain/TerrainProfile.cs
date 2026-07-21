using UnityEngine;

namespace Downshift
{
    public static class TerrainProfile
    {
        public static float Height(float x, TerrainConfig c)
        {
            float grade = Mathf.Min(c.baseGrade + c.gradePerMeter * x, c.maxGrade);
            float trend = -(c.baseGrade * x + 0.5f * c.gradePerMeter * x * x);
            if (grade >= c.maxGrade)
            {
                float xCap = (c.maxGrade - c.baseGrade) / c.gradePerMeter;
                trend = -(c.baseGrade * xCap + 0.5f * c.gradePerMeter * xCap * xCap)
                        - c.maxGrade * (x - xCap);
            }
            float amp = Mathf.Lerp(c.bumpAmplitudeMin, c.bumpAmplitudeMax,
                Mathf.Clamp01(x / c.bumpAmplitudeRampMeters));
            float noise = Mathf.PerlinNoise(c.seed * 0.7331f, x * c.bumpFrequency) - 0.5f;
            return trend + amp * noise;
        }

        public static float[] ChunkHeights(int chunkIndex, TerrainConfig c)
        {
            var h = new float[c.pointsPerChunk + 1];
            float startX = chunkIndex * c.pointsPerChunk * c.pointSpacing;
            for (int i = 0; i <= c.pointsPerChunk; i++)
                h[i] = Height(startX + i * c.pointSpacing, c);
            return h;
        }
    }
}
