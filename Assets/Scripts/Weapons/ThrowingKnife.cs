using NeonBlack.Projectiles;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Weapons
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Weapons/ThrowingKnife")]
    public class ThrowingKnife : Weapon
    {
        #region Serialized Fields

        [Header("Weapon specific properties")]
        [SerializeField]
        private float damage = 1f;

        [SerializeField]
        private float initialVelocity = 15f;

        #endregion

        public override void Shoot(Vector3 origin, Vector3 direction)
        {
            ObjectPoolManager.Spawn<ThrowingKnifeProjectile>(ProjectilePrefab, out var projectile, true);
            projectile.transform.position = origin;
            projectile.transform.forward = direction;
            projectile.Damage = damage;
            projectile.InitialVelocity = initialVelocity;
            projectile.gameObject.SetActive(true);
        }
    }
}
