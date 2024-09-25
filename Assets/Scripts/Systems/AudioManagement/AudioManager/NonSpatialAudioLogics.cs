using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Systems.AudioManagement
{
    public partial class AudioManager
    {
        #region Serialized Fields

        [Header("Non Spatial Audio")]
        [SerializeField]
        private NonSpatialAudio bossNotificationsPrefab;

        [SerializeField]
        private NonSpatialAudio enemiesNotificationsPrefab;

        #endregion

        private NonSpatialAudio[] nonSpatialAudio;

        public static NonSpatialAudio BossNotifications { get; private set; }
        public static NonSpatialAudio EnemiesNotifications { get; private set; }

        private void ConfigureSources()
        {
            nonSpatialAudio = new[]
            {
                BossNotifications = Instantiate(bossNotificationsPrefab, transform),
                EnemiesNotifications = Instantiate(enemiesNotificationsPrefab, transform)
            };
        }

        private async UniTask PlayNonSpatialAsync(NonSpatialAudio cont, AudioClip clip, bool loop = false)
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

        private async UniTask StopNonSpatialAsync(NonSpatialAudio cont)
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
    }
}
