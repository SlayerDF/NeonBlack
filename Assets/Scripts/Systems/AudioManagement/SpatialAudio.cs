using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Systems.AudioManagement
{
    public class SpatialAudio : PoolObject
    {
        #region Serialized Fields

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private Vector2 pitchRandomization = new(0.9f, 1.1f);

        #endregion

        #region Event Functions

        private void FixedUpdate()
        {
            if (audioSource.isPlaying)
            {
                return;
            }

            SceneObjectPool.Despawn(this);
        }

        #endregion

        internal void Play(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.pitch = Random.Range(pitchRandomization.x, pitchRandomization.y);
            audioSource.Play();
        }

        public void Stop()
        {
            if (this == null) // If already despawned
            {
                return;
            }

            audioSource.Stop();
            SceneObjectPool.Despawn(this);
        }
    }
}
