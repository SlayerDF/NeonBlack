using NeonBlack.Projectiles;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Weapons
{
    public class ThrowingKnife : Weapon
    {
        #region Serialized Fields

        [SerializeField]
        private float damage = 1f;

        [SerializeField]
        private Transform projectileSpawnPoint;

        #endregion

        public override void Shoot(Vector3 direction)
        {
            base.Shoot(direction);

            ObjectPoolManager.Spawn<ThrowingKnifeProjectile>(ProjectilePrefab, out var projectile, true);
            projectile.transform.position = projectileSpawnPoint.position;
            projectile.transform.forward = direction;
            projectile.Damage = damage;
            projectile.gameObject.SetActive(true);
        }
    }
}
