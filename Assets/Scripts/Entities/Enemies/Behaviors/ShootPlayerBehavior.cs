using System;
using NeonBlack.Extensions;
using NeonBlack.Projectiles;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Behaviors
{
    public class ShootPlayerBehavior : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField]
        private Transform projectileSpawnPoint;

        [SerializeField]
        private Transform playerTransform;

        [SerializeField]
        private float shootInterval = 0.5f;

        #endregion

        private float shootTimer;

        private Transform spawnPoint;
        private Vector3 targetDirection;

        #region Event Functions

        private void Awake()
        {
            spawnPoint = projectileSpawnPoint ? projectileSpawnPoint : transform;
        }

        private void Update()
        {
            targetDirection = (playerTransform.position - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, targetDirection.With(y: 0f), Time.deltaTime * 5f);
        }

        private void FixedUpdate()
        {
            if ((shootTimer += Time.fixedDeltaTime) < shootInterval)
            {
                return;
            }

            shootTimer = 0f;

            SpawnProjectile();
            Shoot?.Invoke();
        }

        #endregion

        public event Action Shoot;

        private void SpawnProjectile()
        {
            SceneObjectPool.Spawn(projectilePrefab, out Projectile projectile);

            projectile.transform.position = spawnPoint.position;
            projectile.transform.forward = targetDirection;
        }
    }
}
