using NeonBlack.Projectiles;
using UnityEngine;

namespace NeonBlack.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private float shootInterval = 5f;

        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField]
        private Sprite icon;

        #endregion

        private float shootCooldownTimer;

        protected float ShootInterval => shootInterval;
        protected Projectile ProjectilePrefab => projectilePrefab;

        public Sprite Icon => icon;
        public bool ReadyToShoot => shootCooldownTimer <= 0f;

        #region Event Functions

        protected void Update()
        {
            if (shootCooldownTimer >= 0)
            {
                shootCooldownTimer -= Time.deltaTime;
            }
        }

        #endregion

        public virtual void Shoot(Vector3 direction)
        {
            shootCooldownTimer = shootInterval;
        }
    }
}
