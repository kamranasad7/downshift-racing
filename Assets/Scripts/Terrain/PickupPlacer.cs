using System.Collections.Generic;
using UnityEngine;

namespace Downshift
{
    public enum PickupKind { Coin, Coolant, Station }

    public static class PickupPlacer
    {
        public static List<(float x, PickupKind kind)> PlacementsForChunk(int chunkIndex, TerrainConfig c)
        {
            float chunkLen = c.pointsPerChunk * c.pointSpacing;
            float start = chunkIndex * chunkLen;
            float end = start + chunkLen;
            var result = new List<(float, PickupKind)>();
            AddKind(result, PickupKind.Station, c.stationEveryMeters, start, end, c);
            AddKind(result, PickupKind.Coolant, c.coolantEveryMeters, start, end, c);
            AddKind(result, PickupKind.Coin, c.coinEveryMeters, start, end, c);
            result.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            return result;
        }

        static void AddKind(List<(float, PickupKind)> list, PickupKind kind, float baseSpacing, float start, float end, TerrainConfig c)
        {
            float x = 0f;
            int i = 0;
            while (x < end)
            {
                float sparsity = Mathf.Lerp(1f, c.pickupSparsityMaxMultiplier,
                    Mathf.Clamp01(x / c.pickupSparsityRampMeters));
                float jitter = (Mathf.PerlinNoise(c.seed * 1.17f + (int)kind * 31.7f, i * 0.613f) - 0.5f) * baseSpacing * 0.4f;
                float next = x + baseSpacing * sparsity + jitter;
                if (next >= start && next < end) list.Add((next, kind));
                x = next;
                i++;
            }
        }
    }
}
