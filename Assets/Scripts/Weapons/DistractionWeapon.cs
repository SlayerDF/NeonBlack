using NeonBlack.Projectiles;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Weapons
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Weapons/DistractionWeapon")]
    public class DistractionWeapon : Weapon
    {
        #region Serialized Fields

        [Header("Weapon specific properties")]
        [SerializeField]
        private float distractionRadius = 10f;

        [SerializeField]
        private float distractionTime = 5f;

        #endregion

        public override void Shoot(Vector3 origin, Vector3 direction)
        {
            ObjectPoolManager.Spawn<DistractionWeaponProjectile>(ProjectilePrefab, out var projectile, true);
            projectile.transform.position = origin;
            projectile.transform.forward = direction;
            projectile.DistractionRadius = distractionRadius;
            projectile.DistractionTime = distractionTime;
            projectile.gameObject.SetActive(true);
        }
    }
}
