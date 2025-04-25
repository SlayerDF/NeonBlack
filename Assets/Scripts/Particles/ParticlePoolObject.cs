using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Particles
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlePoolObject : PoolObject
    {
        #region Serialized Fields

        [SerializeField]
        private ParticleSystem ps;

        #endregion

        #region Event Functions

        private void Reset()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void LateUpdate()
        {
            if (!ps.isStopped)
            {
                return;
            }

            SceneObjectPool.Despawn(this);
        }

        private void OnEnable()
        {
            ps.Play();
        }

        #endregion
    }
}
