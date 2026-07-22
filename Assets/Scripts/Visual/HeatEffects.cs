using UnityEngine;

namespace Downshift
{
    public class HeatEffects : MonoBehaviour
    {
        public VehicleController vehicle;
        public ParticleSystem brakeEmbersFront;
        public ParticleSystem brakeEmbersRear;
        public ParticleSystem engineSmoke;
        public ParticleSystem blowupBurst;
        public float maxEmberRate = 40f;
        public float maxSmokeRate = 25f;

        void OnEnable()
        {
            if (vehicle != null) vehicle.Blown += HandleBlown;
            Begin(brakeEmbersFront);
            Begin(brakeEmbersRear);
            Begin(engineSmoke);
        }

        static void Begin(ParticleSystem ps)
        {
            if (ps != null && !ps.isPlaying) ps.Play();
        }

        void OnDisable()
        {
            if (vehicle != null) vehicle.Blown -= HandleBlown;
        }

        void Update()
        {
            if (vehicle == null || vehicle.stats == null) return;
            var stats = vehicle.stats;

            float emberT = Mathf.InverseLerp(stats.brakeFadeStartTemp, stats.brakeMaxTemp, vehicle.Brakes.Temp);
            SetRate(brakeEmbersFront, emberT * maxEmberRate);
            SetRate(brakeEmbersRear, emberT * maxEmberRate);

            float smokeT = Mathf.InverseLerp(stats.engineMaxTemp * 0.6f, stats.engineMaxTemp, vehicle.EngineTemp);
            SetRate(engineSmoke, smokeT * maxSmokeRate);
        }

        static void SetRate(ParticleSystem ps, float rate)
        {
            if (ps == null) return;
            var emission = ps.emission;
            emission.rateOverTime = rate;
        }

        void HandleBlown()
        {
            if (blowupBurst == null) return;
            blowupBurst.Emit(40);
        }
    }
}
