using UnityEngine;

namespace Downshift
{
    public static class Drivetrain
    {
        public static float EngineRpm(float wheelAngularVelDeg, float gearRatio, float finalDrive)
        {
            float wheelRpm = Mathf.Abs(wheelAngularVelDeg) / 360f * 60f;
            return wheelRpm * gearRatio * finalDrive;
        }

        public static float EngineBrakeWheelTorque(float engineRpm, float redlineRpm, float gearRatio, float finalDrive, float engineBrakeTorque)
        {
            float rpmFactor = Mathf.Clamp01(engineRpm / redlineRpm);
            return engineBrakeTorque * rpmFactor * gearRatio * finalDrive;
        }

        public static int ClampGear(int gear, int gearCount)
        {
            return Mathf.Clamp(gear, 0, gearCount - 1);
        }
    }
}
