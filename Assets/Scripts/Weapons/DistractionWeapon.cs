using NeonBlack.Projectiles;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Weapons
{
    public class DistractionWeapon : Weapon
    {
        #region Serialized Fields

        [SerializeField]
        private float distractionRadius = 10f;

        [SerializeField]
        private float distractionTime = 5f;

        [SerializeField]
        private Transform projectileSpawnPoint;

        #endregion

        public override void Shoot(Vector3 direction)
        {
            base.Shoot(direction);

            ObjectPoolManager.Spawn<DistractionWeaponProjectile>(ProjectilePrefab, out var projectile, true);
            projectile.transform.position = projectileSpawnPoint.position;
            projectile.transform.forward = direction;
            projectile.DistractionTime = distractionTime;
            projectile.DistractionRadius = distractionRadius;
            projectile.gameObject.SetActive(true);
        }
    }
}
