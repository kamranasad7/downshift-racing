using NUnit.Framework;
using UnityEngine;
using Downshift;

public class ProceduralClipsTests
{
    const int SampleRate = 44100;

    static void AssertFormat(AudioClip clip, float expectedDurationS)
    {
        Assert.IsNotNull(clip);
        Assert.AreEqual(1, clip.channels);
        Assert.AreEqual(SampleRate, clip.frequency);
        int expectedSamples = Mathf.RoundToInt(SampleRate * expectedDurationS);
        Assert.AreEqual(expectedSamples, clip.samples, 1);
    }

    static float PeakAmplitude(AudioClip clip)
    {
        var data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);
        float peak = 0f;
        foreach (var s in data)
            peak = Mathf.Max(peak, Mathf.Abs(s));
        return peak;
    }

    [Test]
    public void EngineLoop_HasExpectedFormatAndAmplitude()
    {
        var clip = ProceduralClips.EngineLoop();
        AssertFormat(clip, 0.5f);
        float peak = PeakAmplitude(clip);
        Assert.Greater(peak, 0.05f);
        Assert.LessOrEqual(peak, 1.0f);
    }

    [Test]
    public void EngineLoop_LoopSeamIsContinuous()
    {
        var clip = ProceduralClips.EngineLoop();
        var data = new float[clip.samples];
        clip.GetData(data, 0);

        int n = data.Length;
        float wrapJump = Mathf.Abs(data[0] - data[n - 1]);
        float slopeEnd = data[n - 1] - data[n - 2];
        float slopeWrap = data[0] - data[n - 1];
        Assert.Less(wrapJump, 0.005f, "sample discontinuity at loop wrap would click");
        Assert.Less(Mathf.Abs(slopeWrap - slopeEnd), 0.01f, "slope discontinuity at loop wrap would click");
    }

    [Test]
    public void BrakeSqueal_HasExpectedFormatAndAmplitude()
    {
        var clip = ProceduralClips.BrakeSqueal();
        AssertFormat(clip, 0.5f);
        float peak = PeakAmplitude(clip);
        Assert.Greater(peak, 0.05f);
        Assert.LessOrEqual(peak, 1.0f);
    }

    [Test]
    public void CoinBlip_HasExpectedFormatAndAmplitude()
    {
        var clip = ProceduralClips.CoinBlip();
        AssertFormat(clip, 0.12f);
        float peak = PeakAmplitude(clip);
        Assert.Greater(peak, 0.05f);
        Assert.LessOrEqual(peak, 1.0f);
    }

    [Test]
    public void CrashThud_HasExpectedFormatAndAmplitude()
    {
        var clip = ProceduralClips.CrashThud();
        AssertFormat(clip, 0.4f);
        float peak = PeakAmplitude(clip);
        Assert.Greater(peak, 0.05f);
        Assert.LessOrEqual(peak, 1.0f);
    }

    [Test]
    public void BlowupBoom_HasExpectedFormatAndAmplitude()
    {
        var clip = ProceduralClips.BlowupBoom();
        AssertFormat(clip, 0.8f);
        float peak = PeakAmplitude(clip);
        Assert.Greater(peak, 0.05f);
        Assert.LessOrEqual(peak, 1.0f);
    }

    [Test]
    public void UiClick_HasExpectedFormatAndAmplitude()
    {
        var clip = ProceduralClips.UiClick();
        AssertFormat(clip, 0.05f);
        float peak = PeakAmplitude(clip);
        Assert.Greater(peak, 0.05f);
        Assert.LessOrEqual(peak, 1.0f);
    }
}
