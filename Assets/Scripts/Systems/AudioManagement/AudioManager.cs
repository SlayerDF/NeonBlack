using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Systems.AudioManagement
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
                Instance.PlayAsync(audio, clip, loop).Forget();
            }
        }

        public static void Stop(NonSpatialAudio audio, AudioClip clip)
        {
            if (audio.Source.clip == clip && audio.ReadyToStop)
            {
                Instance.StopAsync(audio).Forget();
            }
        }

        public static void PauseAll()
        {
            if (Instance == null)
            {
                return;
            }

            for (var i = 0; i < Instance.nonSpatialAudio.Length; i++)
            {
                Instance.nonSpatialAudio[i].Source.Pause();
            }
        }

        public static void UnPauseAll()
        {
            if (Instance == null)
            {
                return;
            }

            for (var i = 0; i < Instance.nonSpatialAudio.Length; i++)
            {
                Instance.nonSpatialAudio[i].Source.UnPause();
            }
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

        private async UniTask PlayAsync(NonSpatialAudio cont, AudioClip clip, bool loop = false)
        {
            var cts = cont.StartTask();
            cont.State = PlayState.Starting;

            if (cont.Source.isPlaying)
            {
                await FadeVolumeAsync(cont.Source, 0f, fadeSpeed, cts.Token);

                if (cts.IsCancellationRequested)
                {
                    return;
                }

                cont.Source.Stop();
            }

            cont.Source.volume = 0f;
            cont.Source.loop = loop;
            cont.Source.clip = clip;
            cont.Source.Play();

            await FadeVolumeAsync(cont.Source, 1f, fadeSpeed, cts.Token);

            if (cts.IsCancellationRequested)
            {
                return;
            }

            cont.State = PlayState.Finished;
        }

        private async UniTask StopAsync(NonSpatialAudio cont)
        {
            var cts = cont.StartTask();
            cont.State = PlayState.Stopping;

            await FadeVolumeAsync(cont.Source, 0f, fadeSpeed, cts.Token);

            if (cts.IsCancellationRequested)
            {
                return;
            }

            cont.Source.Stop();
            cont.State = PlayState.Finished;
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
