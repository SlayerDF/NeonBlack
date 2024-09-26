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
        private NonSpatialAudio musicPrefab;

        #endregion

        private NonSpatialAudio[] nonSpatialAudio;

        public static NonSpatialAudio BossNotifications { get; private set; }

        public static NonSpatialAudio Music { get; private set; }

        private void ConfigureSources()
        {
            nonSpatialAudio = new[]
            {
                BossNotifications = Instantiate(bossNotificationsPrefab, transform),
                Music = Instantiate(musicPrefab, transform)
            };
        }

        private async UniTask PlayNonSpatialAsync(NonSpatialAudio audio, AudioClip clip, bool loop = false)
        {
            var cts = audio.StartTask();
            audio.State = PlayState.Starting;

            if (audio.Source.isPlaying)
            {
                await FadeVolumeAsync(audio.Source, 0f, fadeSpeed, cts.Token);

                if (cts.IsCancellationRequested)
                {
                    return;
                }

                audio.Source.Stop();
            }

            audio.Source.volume = 0f;
            audio.Source.loop = loop;
            audio.Source.clip = clip;
            audio.Source.Play();

            await FadeVolumeAsync(audio.Source, 1f, fadeSpeed, cts.Token);

            if (cts.IsCancellationRequested)
            {
                return;
            }

            audio.State = PlayState.Finished;
        }

        private async UniTask StopNonSpatialAsync(NonSpatialAudio audio)
        {
            var cts = audio.StartTask();
            audio.State = PlayState.Stopping;

            await FadeVolumeAsync(audio.Source, 0f, fadeSpeed, cts.Token);

            if (cts.IsCancellationRequested)
            {
                return;
            }

            audio.Source.Stop();
            audio.State = PlayState.Finished;
        }
    }
}
