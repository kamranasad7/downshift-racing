using NUnit.Framework;
using UnityEngine;
using Downshift;

public class EngineHeatModelTests
{
    VehicleStats S()
    {
        var s = ScriptableObject.CreateInstance<VehicleStats>();
        s.redlineRpm = 6000f;
        s.engineHeatPerRpmOverRedline = 0.01f;
        s.engineCoolPerSecond = 5f;
        s.engineMaxTemp = 100f;
        return s;
    }

    [Test]
    public void HeatsAboveRedlineProportionally()
    {
        var s = S();
        float t = EngineHeatModel.Step(20f, 7000f, s, 1f);
        Assert.AreEqual(20f + 1000f * 0.01f, t, 0.001f);
    }

    [Test]
    public void CoolsBelowRedline()
    {
        var s = S();
        float t = EngineHeatModel.Step(20f, 3000f, s, 1f);
        Assert.AreEqual(15f, t, 0.001f);
    }

    [Test]
    public void ClampsToZeroAndMax()
    {
        var s = S();
        Assert.AreEqual(0f, EngineHeatModel.Step(1f, 0f, s, 10f), 0.001f);
        Assert.AreEqual(100f, EngineHeatModel.Step(99f, 20000f, s, 10f), 0.001f);
    }

    [Test]
    public void BlownExactlyAtMax()
    {
        var s = S();
        Assert.IsFalse(EngineHeatModel.IsBlown(99.9f, s));
        Assert.IsTrue(EngineHeatModel.IsBlown(100f, s));
    }
}
