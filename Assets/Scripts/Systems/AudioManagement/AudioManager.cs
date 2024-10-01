using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NeonBlack.Systems.AudioManagement
{
    public partial class AudioManager : SceneSingleton<AudioManager>
    {
        #region Serialized Fields

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

        #endregion

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
            while (!Mathf.Approximately(src.volume, targetVolume) && !ct.IsCancellationRequested)
            {
                src.volume = Mathf.MoveTowards(src.volume, targetVolume, speed * Time.deltaTime);

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            src.volume = targetVolume;
        }
    }
}
