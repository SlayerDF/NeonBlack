using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Systems.AudioManagement
{
    public partial class AudioManager
    {
        #region Serialized Fields

        [Header("Spatial Audio")]
        [SerializeField]
        private SpatialAudio enemiesNotificationsPrefab;

        [SerializeField]
        private SpatialAudio shotsPrefab;

        [SerializeField]
        private SpatialAudio hitsPrefab;

        [SerializeField]
        private SpatialAudio environmentsPrefab;

        [SerializeField]
        private SpatialAudio footstepsPrefab;

        [SerializeField]
        private SpatialAudio interactionsPrefab;

        [SerializeField]
        private SpatialAudio explosionsPrefab;

        #endregion

        public static SpatialAudio EnemiesNotificationsPrefab => Instance.enemiesNotificationsPrefab;
        public static SpatialAudio ShotsPrefab => Instance.shotsPrefab;
        public static SpatialAudio HitsPrefab => Instance.hitsPrefab;
        public static SpatialAudio EnvironmentsPrefab => Instance.environmentsPrefab;
        public static SpatialAudio FootstepsPrefab => Instance.footstepsPrefab;
        public static SpatialAudio InteractionsPrefab => Instance.interactionsPrefab;
        public static SpatialAudio ExplosionsPrefab => Instance.explosionsPrefab;

        private static SpatialAudio PlaySpatial(SpatialAudio audioPrefab, AudioClip clip, Vector3 position)
        {
            ObjectPoolManager.Spawn<SpatialAudio>(audioPrefab, out var audio);

            audio.transform.position = position;
            audio.Play(clip);

            return audio;
        }
    }
}
