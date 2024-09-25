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
    }
}
