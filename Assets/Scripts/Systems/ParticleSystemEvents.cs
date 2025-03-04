using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBlack.Systems
{
    public class ParticleSystemEvents : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private ParticleSystem ps;

        #endregion

        private readonly List<float> particlesRemainingLifetime = new();
        private ParticleSystem.Particle[] particles;

        private float shortestLifetime = float.MaxValue;

        #region Event Functions

        private void Awake()
        {
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
        }

        private void LateUpdate()
        {
            for (var i = particlesRemainingLifetime.Count - 1; i >= 0; i--)
            {
                if ((particlesRemainingLifetime[i] -= Time.deltaTime) > 0)
                {
                    continue;
                }

                particlesRemainingLifetime.RemoveAt(i);
                ParticleDespawned?.Invoke();
            }

            if (ps.isStopped || ps.particleCount == 0)
            {
                return;
            }

            var count = ps.GetParticles(particles);

            var newShortestLifetime = float.MaxValue;
            for (var i = 0; i < count; i++)
            {
                var lifetime = particles[i].startLifetime - particles[i].remainingLifetime;
                if (lifetime < newShortestLifetime)
                {
                    newShortestLifetime = lifetime;
                }
            }

            if (newShortestLifetime < shortestLifetime)
            {
                for (var i = 0; i < count; i++)
                {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (particles[i].startLifetime - particles[i].remainingLifetime != newShortestLifetime)
                    {
                        continue;
                    }

                    ParticleSpawned?.Invoke(particles[i]);
                    particlesRemainingLifetime.Add(particles[i].remainingLifetime);
                }
            }

            shortestLifetime = newShortestLifetime;
        }

        #endregion

        public event Action<ParticleSystem.Particle> ParticleSpawned;
        public event Action ParticleDespawned;
    }
}
