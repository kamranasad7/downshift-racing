using UnityEngine;

namespace Downshift
{
    public static class ProceduralClips
    {
        const int SampleRate = 44100;

        static int SamplesFor(float durationS) => Mathf.RoundToInt(SampleRate * durationS);

        static AudioClip Build(string name, float[] data)
        {
            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public static AudioClip EngineLoop()
        {
            const float duration = 0.5f;
            int samples = SamplesFor(duration);
            var data = new float[samples];
            var rng = new System.Random(1);

            const float baseFreq = 55f;
            var harmonicFreq = new float[4];
            for (int h = 0; h < 4; h++)
                harmonicFreq[h] = Mathf.Round(baseFreq * (h + 1) * duration) / duration;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float value = 0f;
                for (int h = 0; h < 4; h++)
                    value += Mathf.Cos(2f * Mathf.PI * harmonicFreq[h] * t) / (h + 1);
                value *= 0.6f;
                value += ((float)rng.NextDouble() * 2f - 1f) * 0.03f;
                data[i] = Mathf.Clamp(value * 0.4f, -1f, 1f);
            }
            return Build("EngineLoop", data);
        }

        public static AudioClip BrakeSqueal()
        {
            const float duration = 0.5f;
            int samples = SamplesFor(duration);
            var data = new float[samples];
            var rng = new System.Random(2);

            const float a = 0.95f;
            float prevX = 0f, prevY = 0f;
            for (int i = 0; i < samples; i++)
            {
                float x = (float)(rng.NextDouble() * 2.0 - 1.0);
                float y = a * (prevY + x - prevX);
                prevX = x;
                prevY = y;
                data[i] = Mathf.Clamp(y * 0.3f, -1f, 1f);
            }
            return Build("BrakeSqueal", data);
        }

        public static AudioClip CoinBlip()
        {
            const float duration = 0.12f;
            const float freq = 1568f;
            const float decay = 18f;
            int samples = SamplesFor(duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * Mathf.Exp(-decay * t) * 0.7f;
            }
            return Build("CoinBlip", data);
        }

        public static AudioClip CrashThud()
        {
            const float duration = 0.4f;
            int samples = SamplesFor(duration);
            var data = new float[samples];
            var rng = new System.Random(3);

            const float lpCoeff = 0.15f;
            float prevY = 0f;
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float x = (float)(rng.NextDouble() * 2.0 - 1.0);
                prevY += lpCoeff * (x - prevY);
                float decay = Mathf.Exp(-6f * t);
                data[i] = Mathf.Clamp(prevY * decay * 3f, -1f, 1f);
            }
            return Build("CrashThud", data);
        }

        public static AudioClip BlowupBoom()
        {
            const float duration = 0.8f;
            int samples = SamplesFor(duration);
            var data = new float[samples];
            var rng = new System.Random(4);

            float accum = 0f;
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float x = (float)(rng.NextDouble() * 2.0 - 1.0);
                accum = Mathf.Clamp(accum + x * 0.05f, -1f, 1f);
                float decay = Mathf.Exp(-3f * t);
                data[i] = Mathf.Clamp(accum * decay * 1.3f, -1f, 1f);
            }
            return Build("BlowupBoom", data);
        }

        public static AudioClip UiClick()
        {
            const float duration = 0.05f;
            const float freq = 2000f;
            const float decay = 60f;
            int samples = SamplesFor(duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * Mathf.Exp(-decay * t) * 0.8f;
            }
            return Build("UiClick", data);
        }
    }
}
