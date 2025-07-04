using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Projectiles
{
    public abstract class Projectile : PoolObject
    {
        #region Serialized Fields

        [SerializeField]
        private float lifetime = 10f;

        #endregion

        private float time;

        #region Event Functions

        protected virtual void FixedUpdate()
        {
            if ((time += Time.fixedDeltaTime) >= lifetime && isActiveAndEnabled)
            {
                SceneObjectPool.Despawn(this);
            }
        }

        protected virtual void OnEnable()
        {
            time = 0f;
        }

        #endregion
    }
}
