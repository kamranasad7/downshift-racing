using NUnit.Framework;
using Downshift;

public class DrivetrainTests
{
    [Test]
    public void EngineRpmScalesWithGearRatioAndWheelSpeed()
    {
        float wheelDeg = 360f * 10f;
        float rpm = Drivetrain.EngineRpm(wheelDeg, 2f, 3f);
        Assert.AreEqual(10f * 60f * 2f * 3f, rpm, 0.01f);
    }

    [Test]
    public void EngineRpmZeroAtStandstill()
    {
        Assert.AreEqual(0f, Drivetrain.EngineRpm(0f, 3.2f, 3.7f), 0.001f);
    }

    [Test]
    public void EngineBrakeTorqueGrowsWithRpmAndGear()
    {
        float low = Drivetrain.EngineBrakeWheelTorque(2000f, 6500f, 1f, 3.7f, 260f);
        float high = Drivetrain.EngineBrakeWheelTorque(6000f, 6500f, 1f, 3.7f, 260f);
        float lowGear = Drivetrain.EngineBrakeWheelTorque(6000f, 6500f, 3.2f, 3.7f, 260f);
        Assert.Greater(high, low);
        Assert.Greater(lowGear, high);
    }

    [Test]
    public void EngineBrakeTorqueZeroAtZeroRpm()
    {
        Assert.AreEqual(0f, Drivetrain.EngineBrakeWheelTorque(0f, 6500f, 3.2f, 3.7f, 260f), 0.001f);
    }

    [Test]
    public void ClampGearStaysInRange()
    {
        Assert.AreEqual(0, Drivetrain.ClampGear(-1, 5));
        Assert.AreEqual(4, Drivetrain.ClampGear(7, 5));
        Assert.AreEqual(2, Drivetrain.ClampGear(2, 5));
    }
}
