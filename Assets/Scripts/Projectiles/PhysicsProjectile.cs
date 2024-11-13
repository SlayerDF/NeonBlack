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

        public float InitialVelocity
        {
            get => initialVelocity;
            set => initialVelocity = value;
        }

        protected Rigidbody RigidBody => rigidBody;

        #region Event Functions

        protected override void OnEnable()
        {
            base.OnEnable();

            rigidBody.velocity = transform.forward * InitialVelocity;
        }

        #endregion
    }
}
