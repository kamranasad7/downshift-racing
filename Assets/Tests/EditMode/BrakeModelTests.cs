using NUnit.Framework;
using UnityEngine;
using Downshift;

public class BrakeModelTests
{
    VehicleStats S()
    {
        var s = ScriptableObject.CreateInstance<VehicleStats>();
        s.brakeHeatPerSecondAtRef = 10f;
        s.brakeHeatRefSpeed = 20f;
        s.brakeCoolPerSecond = 5f;
        s.brakeFadeStartTemp = 60f;
        s.brakeMaxTemp = 100f;
        s.brakeReengageTemp = 45f;
        return s;
    }

    [Test]
    public void HeatsWhileBrakingScaledBySpeed()
    {
        var st = BrakeModel.Step(new BrakeState(), true, 40f, S(), 1f);
        Assert.AreEqual(20f, st.Temp, 0.001f);
    }

    [Test]
    public void CoolsWhenReleased()
    {
        var st = BrakeModel.Step(new BrakeState { Temp = 30f }, false, 40f, S(), 1f);
        Assert.AreEqual(25f, st.Temp, 0.001f);
    }

    [Test]
    public void FullEffectivenessBelowFadeStart()
    {
        Assert.AreEqual(1f, BrakeModel.Effectiveness(new BrakeState { Temp = 59f }, S()), 0.001f);
    }

    [Test]
    public void EffectivenessFadesLinearlyToZeroAtMax()
    {
        var s = S();
        Assert.AreEqual(0.5f, BrakeModel.Effectiveness(new BrakeState { Temp = 80f }, s), 0.001f);
        Assert.AreEqual(0f, BrakeModel.Effectiveness(new BrakeState { Temp = 100f }, s), 0.001f);
    }

    [Test]
    public void FadedLockoutUntilReengageTemp()
    {
        var s = S();
        var st = BrakeModel.Step(new BrakeState { Temp = 99.9f }, true, 40f, s, 1f);
        Assert.IsTrue(st.Faded);
        Assert.AreEqual(0f, BrakeModel.Effectiveness(st, s), 0.001f);
        st = BrakeModel.Step(st, false, 40f, s, 10f);
        Assert.IsTrue(st.Temp <= 50f && st.Temp > 45f ? st.Faded : true);
        while (st.Temp > 45f) st = BrakeModel.Step(st, false, 40f, s, 0.1f);
        Assert.IsFalse(st.Faded);
        Assert.Greater(BrakeModel.Effectiveness(st, s), 0.9f);
    }
}
