namespace Downshift
{
    public static class VehicleInput
    {
        public static bool KeyboardBrake;
        public static bool UiBrake;
        public static bool BrakeHeld => KeyboardBrake || UiBrake;
    }
}
