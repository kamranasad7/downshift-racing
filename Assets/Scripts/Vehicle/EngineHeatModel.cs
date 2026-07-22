using UnityEngine;

namespace Downshift
{
    public static class EngineHeatModel
    {
        public static float Step(float temp, float rpm, VehicleStats s, float dt)
        {
            float delta;
            if (rpm <= s.engineHeatStartRpm)
            {
                delta = -s.engineCoolPerSecond;
            }
            else
            {
                float n = (rpm - s.engineHeatStartRpm) / Mathf.Max(1f, s.redlineRpm - s.engineHeatStartRpm);
                delta = s.engineHeatAtRedline * n * n;
            }
            return Mathf.Clamp(temp + delta * dt, 0f, s.engineMaxTemp);
        }

        public static bool IsBlown(float temp, VehicleStats s)
        {
            return temp >= s.engineMaxTemp;
        }
    }
}
