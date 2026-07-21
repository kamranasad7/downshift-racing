using UnityEngine;

namespace Downshift
{
    public static class EngineHeatModel
    {
        public static float Step(float temp, float rpm, VehicleStats s, float dt)
        {
            float over = rpm - s.redlineRpm;
            float delta = over > 0f
                ? over * s.engineHeatPerRpmOverRedline
                : -s.engineCoolPerSecond;
            return Mathf.Clamp(temp + delta * dt, 0f, s.engineMaxTemp);
        }

        public static bool IsBlown(float temp, VehicleStats s)
        {
            return temp >= s.engineMaxTemp;
        }
    }
}
