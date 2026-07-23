using System.Collections.Generic;
using UnityEngine;

namespace Downshift
{
    public enum HazardKind { Rocks, Ridge, Washboard }

    public static class HazardPlacer
    {
        public static List<(float x, HazardKind kind)> PlacementsForChunk(int chunkIndex, TerrainConfig c)
        {
            float chunkLen = c.pointsPerChunk * c.pointSpacing;
            float start = chunkIndex * chunkLen;
            float end = start + chunkLen;
            var stations = StationsNear(chunkIndex, c);
            var result = new List<(float, HazardKind)>();

            float x = 0f;
            int i = 0;
            while (x < end)
            {
                float t = Mathf.Clamp01(x / c.hazardRampMeters);
                float spacing = Mathf.Lerp(c.hazardEveryMeters, c.hazardMinSpacingMeters, t);
                float jitter = (Mathf.PerlinNoise(c.seed * 1.31f + 7.1f, i * 0.577f) - 0.5f) * spacing * 0.4f;
                float next = x + spacing + jitter;

                if (next >= start && next < end && next >= c.hazardStartMeters && !NearStation(next, stations, c.stationClearMeters))
                    result.Add((next, KindAt(i, c)));

                x = next;
                i++;
            }
            return result;
        }

        public static List<float> SignsForChunk(int chunkIndex, TerrainConfig c)
        {
            float chunkLen = c.pointsPerChunk * c.pointSpacing;
            float start = chunkIndex * chunkLen;
            float end = start + chunkLen;
            var result = new List<float>();

            AddSigns(PlacementsForChunk(chunkIndex, c), start, end, c, result);
            AddSigns(PlacementsForChunk(chunkIndex + 1, c), start, end, c, result);

            result.Sort();
            return result;
        }

        static void AddSigns(List<(float x, HazardKind kind)> hazards, float start, float end, TerrainConfig c, List<float> result)
        {
            foreach (var h in hazards)
            {
                float signX = h.x - c.signLeadMeters;
                if (signX >= start && signX < end) result.Add(signX);
            }
        }

        static HazardKind KindAt(int i, TerrainConfig c)
        {
            float n = Mathf.PerlinNoise(c.seed * 2.63f + i * 0.421f, 3.7f);
            int idx = Mathf.Clamp(Mathf.FloorToInt(n * 3f), 0, 2);
            return (HazardKind)idx;
        }

        static bool NearStation(float x, List<float> stations, float clearMeters)
        {
            foreach (var s in stations)
                if (Mathf.Abs(x - s) < clearMeters) return true;
            return false;
        }

        static List<float> StationsNear(int chunkIndex, TerrainConfig c)
        {
            float chunkLen = c.pointsPerChunk * c.pointSpacing;
            int radius = Mathf.Max(1, Mathf.CeilToInt(c.stationClearMeters / chunkLen) + 1);
            var list = new List<float>();
            for (int k = chunkIndex - radius; k <= chunkIndex + radius; k++)
            {
                if (k < 0) continue;
                foreach (var p in PickupPlacer.PlacementsForChunk(k, c))
                    if (p.kind == PickupKind.Station) list.Add(p.x);
            }
            return list;
        }
    }
}
