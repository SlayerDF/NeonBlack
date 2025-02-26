using System.Threading;
using Cysharp.Threading.Tasks;
using NeonBlack.Utilities;
using UnityEngine;
using UnityEngine.Audio;

namespace NeonBlack.Systems.AudioManagement
{
    public partial class AudioManager : SceneSingleton<AudioManager>
    {
        private const string AudioMixerMasterVolumeParam = "MasterVolume";
        private const string AudioMixerMusicVolumeParam = "MusicVolume";
        private const string AudioMixerSfxVolumeParam = "SfxVolume";

        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private AudioMixer audioMixer;

        [Header("Properties")]
        [SerializeField]
        private float fadeSpeed = 2f;

        #endregion

        #region Event Functions

        protected override void Awake()
        {
            base.Awake();

            ConfigureSources();
        }

        private void Start()
        {
            OnMasterVolumeChanged(Settings.MasterVolume);
            OnMusicVolumeChanged(Settings.MusicVolume);
            OnSfxVolumeChanged(Settings.SfxVolume);
        }

        private void OnEnable()
        {
            Settings.MasterVolumeChanged += OnMasterVolumeChanged;
            Settings.MusicVolumeChanged += OnMusicVolumeChanged;
            Settings.SfxVolumeChanged += OnSfxVolumeChanged;
        }

        private void OnDisable()
        {
            Settings.MasterVolumeChanged -= OnMasterVolumeChanged;
            Settings.MusicVolumeChanged -= OnMusicVolumeChanged;
            Settings.SfxVolumeChanged -= OnSfxVolumeChanged;
        }

        #endregion

        private void OnMasterVolumeChanged(float volume)
        {
            UpdateMixerVolumeParam(AudioMixerMasterVolumeParam, volume);
        }

        private void OnMusicVolumeChanged(float volume)
        {
            UpdateMixerVolumeParam(AudioMixerMusicVolumeParam, volume);
        }

        private void OnSfxVolumeChanged(float volume)
        {
            UpdateMixerVolumeParam(AudioMixerSfxVolumeParam, volume);
        }

        private void UpdateMixerVolumeParam(string mixerVolumeParam, float volume)
        {
            volume = Mathf.Clamp(volume, 0.0001f, 1f);
            audioMixer.SetFloat(mixerVolumeParam, Mathf.Log10(volume) * 20);
        }

        public static void Play(NonSpatialAudio audio, AudioClip clip, bool loop = false)
        {
            if (audio.Source.clip != clip || audio.ReadyToStart)
            {
                Instance.PlayNonSpatialAsync(audio, clip, loop).Forget();
            }
        }

        public static void Stop(NonSpatialAudio audio, AudioClip clip)
        {
            if (audio.Source.clip == clip && audio.ReadyToStop)
            {
                Instance.StopNonSpatialAsync(audio).Forget();
            }
        }

        public static SpatialAudio Play(SpatialAudio audioPrefab, AudioClip clip, Vector3 position)
        {
            return PlaySpatial(audioPrefab, clip, position);
        }

        public static void StopAll()
        {
            if (Instance == null)
            {
                return;
            }

            for (var i = 0; i < Instance.nonSpatialAudio.Length; i++)
            {
                Instance.nonSpatialAudio[i].CancelTask();
                Instance.nonSpatialAudio[i].Source.Stop();
            }
        }

        private static async UniTask FadeVolumeAsync(AudioSource src, float targetVolume, float speed,
            CancellationToken ct)
        {
            if (!src)
            {
                return;
            }

            while (!Mathf.Approximately(src.volume, targetVolume) && !ct.IsCancellationRequested)
            {
                src.volume = Mathf.MoveTowards(src.volume, targetVolume, speed * Time.deltaTime);

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            src.volume = targetVolume;
        }
    }
}
