using NeonBlack.Systems;
using NeonBlack.Systems.AudioManagement;
using UnityEngine;

namespace NeonBlack.Particles
{
    public class ExplosionParticle : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private ParticleSystemEvents particleSystemEvents;

        #endregion

        #region Event Functions

        private void OnEnable()
        {
            particleSystemEvents.ParticleSpawned += OnParticleSpawned;
        }

        private void OnDisable()
        {
            particleSystemEvents.ParticleSpawned -= OnParticleSpawned;
        }

        #endregion

        private void OnParticleSpawned(ParticleSystem.Particle p)
        {
            AudioManager.Play(AudioManager.ExplosionsPrefab, AudioManager.ExplosionClip, p.position);
        }
    }
}
