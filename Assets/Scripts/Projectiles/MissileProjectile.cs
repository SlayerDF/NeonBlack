using NeonBlack.Interfaces;
using NeonBlack.Particles;
using NeonBlack.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeonBlack.Projectiles
{
    [RequireComponent(typeof(Collider))]
    public class MissileProjectile : Projectile
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private Transform target;

        [SerializeField]
        private ParticlePoolObject explosionParticlesPrefab;

        #endregion

        [Header("General settings")]
        [SerializeField]
        private readonly float damage = 1f;

        [SerializeField]
        private readonly float noiseAmplitude = 2f;

        [SerializeField]
        private readonly float noiseFrequency = 2f;

        [SerializeField]
        private readonly float rotationSpeed = 10f;

        [Header("Flight settings")]
        [SerializeField]
        private readonly float speed = 35f;

        [SerializeField]
        private readonly float spiralAmplitude = 1f;

        [SerializeField]
        private readonly float spiralFrequency = 2f;

        private Vector3 randomOffset;

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        #region Event Functions

        private void Start()
        {
            randomOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ) * noiseAmplitude;
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            var toTarget = (target.position - transform.position).normalized;
            var perpendicular = Vector3.Cross(toTarget, Vector3.up).normalized;
            var spiralOffset = perpendicular * (Mathf.Sin(Time.time * spiralFrequency) * spiralAmplitude);

            var noiseOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * noiseFrequency, randomOffset.x) - 0.5f,
                Mathf.PerlinNoise(Time.time * noiseFrequency, randomOffset.y) - 0.5f,
                Mathf.PerlinNoise(Time.time * noiseFrequency, randomOffset.z) - 0.5f
            ) * noiseAmplitude;

            var direction = (toTarget + spiralOffset + noiseOffset).normalized;

            transform.forward = Vector3.Slerp(transform.forward, direction, rotationSpeed * Time.deltaTime);
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IEntityHealth>(out var health))
            {
                return;
            }

            health.TakeDamage(DamageSource.Missile, damage);

            SceneObjectPool.Spawn<ParticlePoolObject>(explosionParticlesPrefab, out var ps);
            ps.transform.position = transform.position;

            if (this)
            {
                SceneObjectPool.Despawn(this);
            }
        }

        #endregion
    }
}
