using NUnit.Framework;
using UnityEditor;
using Downshift;

public class VehicleStatsTests
{
    [Test]
    public void HatchbackAssetExistsWithSaneValues()
    {
        var s = AssetDatabase.LoadAssetAtPath<VehicleStats>("Assets/Data/Hatchback.asset");
        Assert.IsNotNull(s);
        Assert.Greater(s.gearRatios.Length, 2);
        for (int i = 1; i < s.gearRatios.Length; i++)
            Assert.Less(s.gearRatios[i], s.gearRatios[i - 1]);
        Assert.Greater(s.redlineRpm, 0f);
        Assert.Greater(s.brakeMaxTemp, s.brakeFadeStartTemp);
        Assert.Less(s.brakeReengageTemp, s.brakeFadeStartTemp);
        Assert.Greater(s.creepForce, 0f);
    }
}
