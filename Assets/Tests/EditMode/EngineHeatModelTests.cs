using NUnit.Framework;
using UnityEngine;
using Downshift;

public class EngineHeatModelTests
{
    VehicleStats S()
    {
        var s = ScriptableObject.CreateInstance<VehicleStats>();
        s.redlineRpm = 6000f;
        s.engineHeatStartRpm = 3000f;
        s.engineHeatAtRedline = 10f;
        s.engineCoolPerSecond = 5f;
        s.engineMaxTemp = 100f;
        return s;
    }

    [Test]
    public void CoolsBelowHeatStartRpm()
    {
        var s = S();
        Assert.AreEqual(15f, EngineHeatModel.Step(20f, 2000f, s, 1f), 0.001f);
        Assert.AreEqual(15f, EngineHeatModel.Step(20f, 3000f, s, 1f), 0.001f);
    }

    [Test]
    public void HeatsLightlyJustAboveStart()
    {
        var s = S();
        float t = EngineHeatModel.Step(20f, 3300f, s, 1f);
        Assert.AreEqual(20f + 10f * 0.01f, t, 0.001f);
    }

    [Test]
    public void HeatsQuadraticallyTowardRedline()
    {
        var s = S();
        float t = EngineHeatModel.Step(20f, 4500f, s, 1f);
        Assert.AreEqual(20f + 10f * 0.25f, t, 0.001f);
    }

    [Test]
    public void HeatAtRedlineEqualsConfiguredRate()
    {
        var s = S();
        float t = EngineHeatModel.Step(20f, 6000f, s, 1f);
        Assert.AreEqual(30f, t, 0.001f);
    }

    [Test]
    public void HeatingAcceleratesPastRedline()
    {
        var s = S();
        float atRedline = EngineHeatModel.Step(20f, 6000f, s, 1f) - 20f;
        float pastRedline = EngineHeatModel.Step(20f, 9000f, s, 1f) - 20f;
        Assert.AreEqual(atRedline * 4f, pastRedline, 0.001f);
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
