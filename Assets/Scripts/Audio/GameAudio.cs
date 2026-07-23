using UnityEngine;

namespace Downshift
{
    public class GameAudio : MonoBehaviour
    {
        public VehicleController vehicle;
        public RunManager runManager;

        public float enginePitchMin = 0.6f;
        public float enginePitchMax = 2.2f;
        public float engineVolume = 0.5f;
        public float squealVolume = 0.7f;
        public float oneShotVolume = 0.8f;
        public float musicVolume = 0.35f;
        public AudioClip musicClip;

        AudioSource _engineSource;
        AudioSource _squealSource;
        AudioSource _oneShotSource;
        AudioSource _musicSource;

        static GameAudio _instance;
        static AudioClip _coinClip;
        static AudioClip _crashClip;
        static AudioClip _blowupClip;
        static AudioClip _clickClip;

        void Awake()
        {
            _instance = this;

            _engineSource = gameObject.AddComponent<AudioSource>();
            _engineSource.playOnAwake = false;
            _engineSource.clip = ProceduralClips.EngineLoop();
            _engineSource.loop = true;
            _engineSource.volume = engineVolume;
            _engineSource.Play();

            _squealSource = gameObject.AddComponent<AudioSource>();
            _squealSource.playOnAwake = false;
            _squealSource.clip = ProceduralClips.BrakeSqueal();
            _squealSource.loop = true;
            _squealSource.volume = 0f;
            _squealSource.Play();

            _oneShotSource = gameObject.AddComponent<AudioSource>();
            _oneShotSource.playOnAwake = false;

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            _musicSource.volume = musicVolume;
            if (musicClip != null)
            {
                _musicSource.clip = musicClip;
                _musicSource.Play();
            }

            if (runManager != null)
                runManager.StateChanged += OnRunStateChanged;
        }

        void OnDestroy()
        {
            if (runManager != null)
                runManager.StateChanged -= OnRunStateChanged;
            if (_instance == this)
                _instance = null;
        }

        void OnRunStateChanged(RunState state)
        {
            if (state == RunState.Crashed) PlayCrash();
            else if (state == RunState.BlownUp) PlayBlowup();
        }

        void Update()
        {
            bool sfxMuted = GameSession.Save.sfxMuted;
            _musicSource.volume = GameSession.Save.musicMuted ? 0f : musicVolume;

            if (vehicle == null || vehicle.stats == null) return;

            float rpmT = Mathf.InverseLerp(vehicle.stats.idleRpm, vehicle.stats.redlineRpm, vehicle.EngineRpm);
            _engineSource.pitch = Mathf.Lerp(enginePitchMin, enginePitchMax, rpmT);
            float speedT = Mathf.Clamp01(vehicle.SpeedMs / 20f);
            _engineSource.volume = (sfxMuted || vehicle.IsShutdown) ? 0f : engineVolume * (0.4f + 0.6f * speedT);

            bool squealing = !sfxMuted && VehicleInput.BrakeHeld && !vehicle.IsShutdown && vehicle.SpeedMs > 0.5f;
            _squealSource.volume = squealing
                ? squealVolume * Mathf.Clamp01(vehicle.Brakes.Temp / vehicle.stats.brakeMaxTemp)
                : 0f;
        }

        public static void PlayCoin()
        {
            if (_coinClip == null) _coinClip = ProceduralClips.CoinBlip();
            PlayOneShot(_coinClip);
        }

        public static void PlayCrash()
        {
            if (_crashClip == null) _crashClip = ProceduralClips.CrashThud();
            PlayOneShot(_crashClip);
        }

        public static void PlayBlowup()
        {
            if (_blowupClip == null) _blowupClip = ProceduralClips.BlowupBoom();
            PlayOneShot(_blowupClip);
        }

        public static void PlayClick()
        {
            if (_clickClip == null) _clickClip = ProceduralClips.UiClick();
            PlayOneShot(_clickClip);
        }

        static void PlayOneShot(AudioClip clip)
        {
            if (_instance == null || _instance._oneShotSource == null) return;
            if (GameSession.Save.sfxMuted) return;
            _instance._oneShotSource.PlayOneShot(clip, _instance.oneShotVolume);
        }
    }
}
