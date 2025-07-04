using NeonBlack.Enums;
using NeonBlack.Interfaces;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Projectiles
{
    public class ThrowingKnifeProjectile : PhysicsProjectile
    {
        public float Damage { get; set; }

        #region Event Functions

        protected virtual void Update()
        {
            transform.forward = RigidBody.velocity;
        }

        private void OnTriggerEnter(Collider other)
        {
            var layer = (Layer)other.gameObject.layer;

            switch (layer)
            {
                case Layer.Enemies:
                    if (other.TryGetComponent(out IEntityHealth entityHealth))
                    {
                        entityHealth.TakeDamage(DamageSource.Normal, Damage);
                    }

                    break;
            }

            SceneObjectPool.Despawn(this);
        }

        #endregion
    }
}
