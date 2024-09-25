using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Systems.AudioManagement
{
    public class AudioManager : SceneSingleton<AudioManager>
    {
        #region Serialized Fields

        [Header("Properties")]
        [SerializeField]
        private float fadeSpeed = 2f;

        [Header("Sources")]
        [SerializeField]
        private AudioSource bossNotificationsSource;

        [SerializeField]
        private AudioSource enemiesNotificationsSource;

        [Header("Clips")]
        [SerializeField]
        private AudioClip dangerClip;

        [SerializeField]
        private AudioClip enemyAlertedClip;

        #endregion

        private AudioSourceContainer[] audioSourceContainers;

        // Sources
        public static AudioSourceContainer BossNotificationsSource { get; private set; }
        public static AudioSourceContainer EnemiesNotificationsSource { get; private set; }

        // Clips
        public static AudioClip DangerClip => Instance.dangerClip;
        public static AudioClip EnemyAlertedClip => Instance.enemyAlertedClip;

        #region Event Functions

        protected override void Awake()
        {
            base.Awake();

            audioSourceContainers = new[]
            {
                BossNotificationsSource = new AudioSourceContainer(Instance.bossNotificationsSource),
                EnemiesNotificationsSource = new AudioSourceContainer(Instance.enemiesNotificationsSource)
            };
        }

        protected override void OnDestroy()
        {
            for (var i = 0; i < Instance.audioSourceContainers.Length; i++)
            {
                Instance.audioSourceContainers[i].Dispose();
            }

            base.OnDestroy();
        }

        #endregion

        public static void Play(AudioSourceContainer cont, AudioClip clip, bool loop = false)
        {
            if (cont.Source.clip == clip && (cont.State == AudioSourceContainer.PrepareState.Starting ||
                                             (cont.State == AudioSourceContainer.PrepareState.Finished &&
                                              cont.Source.isPlaying)))
            {
                return;
            }

            Instance.PlayAsync(cont, clip, loop).Forget();
        }

        public static void Stop(AudioSourceContainer cont, AudioClip clip)
        {
            if (cont.Source.clip != clip ||
                cont.State == AudioSourceContainer.PrepareState.Stopping ||
                (cont.State == AudioSourceContainer.PrepareState.Finished && !cont.Source.isPlaying))
            {
                return;
            }

            Instance.StopAsync(cont).Forget();
        }

        public static void PauseAll()
        {
            if (Instance == null)
            {
                return;
            }

            for (var i = 0; i < Instance.audioSourceContainers.Length; i++)
            {
                Instance.audioSourceContainers[i].Source.Pause();
            }
        }

        public static void UnPauseAll()
        {
            if (Instance == null)
            {
                return;
            }

            for (var i = 0; i < Instance.audioSourceContainers.Length; i++)
            {
                Instance.audioSourceContainers[i].Source.UnPause();
            }
        }

        public static void StopAll()
        {
            if (Instance == null)
            {
                return;
            }

            for (var i = 0; i < Instance.audioSourceContainers.Length; i++)
            {
                Instance.audioSourceContainers[i].CancelTask();
                Instance.audioSourceContainers[i].Source.Stop();
            }
        }

        private async UniTask PlayAsync(AudioSourceContainer cont, AudioClip clip, bool loop = false)
        {
            var cts = cont.StartTask();
            cont.State = AudioSourceContainer.PrepareState.Starting;

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

            cont.State = AudioSourceContainer.PrepareState.Finished;
        }

        private async UniTask StopAsync(AudioSourceContainer cont)
        {
            var cts = cont.StartTask();
            cont.State = AudioSourceContainer.PrepareState.Stopping;

            await FadeVolumeAsync(cont.Source, 0f, fadeSpeed, cts.Token);

            if (cts.IsCancellationRequested)
            {
                return;
            }

            cont.Source.Stop();
            cont.State = AudioSourceContainer.PrepareState.Finished;
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
