using NeonBlack.Enums;
using NeonBlack.Interfaces;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack
{
    [RequireComponent(typeof(SphereCollider))]
    public class EnemyDistractor : PoolObject
    {
        #region Serialized Fields

        [SerializeField]
        private SphereCollider coll;

        #endregion

        private float lifetime;

        public float DistractionTime { get; set; }

        public float DistractionRadius
        {
            get => coll.radius;
            set => coll.radius = value;
        }

        #region Event Functions

        private void FixedUpdate()
        {
            if ((lifetime -= Time.fixedDeltaTime) < 0)
            {
                SceneObjectPool.Despawn(this);
            }
        }

        private void OnEnable()
        {
            lifetime = DistractionTime;
        }

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
