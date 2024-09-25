using UnityEngine;

namespace Systems.AudioManagement
{
    public partial class AudioManager
    {
        #region Serialized Fields

        [Header("Spatial Audio")]
        [SerializeField]
        private SpatialAudio shotsPrefab;

        #endregion

        public static SpatialAudio ShotsPrefab => Instance.shotsPrefab;

        private static SpatialAudio PlaySpatial(SpatialAudio audioPrefab, AudioClip clip, Vector3 position)
        {
            ObjectPoolManager.Spawn<SpatialAudio>(audioPrefab, out var audio);

            audio.transform.position = position;
            audio.Play(clip);

            return audio;
        }
    }
}
