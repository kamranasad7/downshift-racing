using UnityEngine;

namespace Downshift
{
    public struct BrakeState
    {
        public float Temp;
        public bool Faded;
    }

    public static class BrakeModel
    {
        public static BrakeState Step(BrakeState state, bool braking, float speedMs, VehicleStats s, float dt)
        {
            float delta = braking
                ? s.brakeHeatPerSecondAtRef * (speedMs / s.brakeHeatRefSpeed)
                : -s.brakeCoolPerSecond;
            state.Temp = Mathf.Clamp(state.Temp + delta * dt, 0f, s.brakeMaxTemp);
            if (state.Temp >= s.brakeMaxTemp) state.Faded = true;
            if (state.Faded && state.Temp <= s.brakeReengageTemp) state.Faded = false;
            return state;
        }

        public static float Effectiveness(BrakeState state, VehicleStats s)
        {
            if (state.Faded) return 0f;
            if (state.Temp <= s.brakeFadeStartTemp) return 1f;
            return Mathf.Clamp01(1f - (state.Temp - s.brakeFadeStartTemp) / (s.brakeMaxTemp - s.brakeFadeStartTemp));
        }
    }
}
