using NeonBlack.Enums;
using NeonBlack.Interfaces;
using UnityEngine;

namespace NeonBlack.Projectiles
{
    [RequireComponent(typeof(SphereCollider))]
    public class DistractionWeaponProjectile : PhysicsProjectile
    {
        #region Serialized Fields

        [SerializeField]
        private SphereCollider coll;

        #endregion

        public float DistractionTime { get; set; }

        public float DistractionRadius
        {
            get => coll.radius;
            set => coll.radius = value;
        }

        #region Event Functions

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, DistractionRadius);
        }
#endif

        private void OnTriggerEnter(Collider other)
        {
            var layer = (Layer)other.gameObject.layer;

            switch (layer)
            {
                case Layer.Enemies:
                    if (other.TryGetComponent(out IDistractible distractible))
                    {
                        distractible.Distract(gameObject, DistractionTime);
                    }

                    break;
            }
        }

        #endregion
    }
}
