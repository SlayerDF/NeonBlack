using NeonBlack.Enums;
using NeonBlack.Interfaces;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Projectiles
{
    public class TrapProjectile : PhysicsProjectile
    {
        #region Serialized Fields

        [SerializeField]
        private float damage = 1f;

        [SerializeField]
        private TrailRenderer trailRenderer;

        #endregion

        #region Event Functions

        private void Update()
        {
            transform.forward = RigidBody.velocity;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            trailRenderer.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            var layer = (Layer)other.gameObject.layer;

            switch (layer)
            {
                case Layer.Enemies:
                case Layer.Player:
                    if (other.TryGetComponent(out IEntityHealth entityHealth))
                    {
                        entityHealth.TakeDamage(DamageSource.Normal, damage);
                    }

                    break;
            }

            ObjectPoolManager.Despawn(this);
        }

        #endregion
    }
}
