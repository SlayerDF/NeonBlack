using UnityEngine;

namespace NeonBlack.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class PhysicsProjectile : Projectile
    {
        #region Serialized Fields

        [SerializeField]
        private Rigidbody rigidBody;

        [SerializeField]
        private float initialVelocity = 10f;

        #endregion

        #region Event Functions

        protected virtual void Update()
        {
            transform.forward = rigidBody.velocity;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            rigidBody.velocity = transform.forward * initialVelocity;
        }

        #endregion
    }
}
