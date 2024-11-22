using UnityEngine;

namespace NeonBlack.Interactables
{
    public abstract class Collectible : MonoBehaviour
    {
    }

    [RequireComponent(typeof(Collider))]
    public abstract class Collectible<T> : Collectible where T : MonoBehaviour
    {
        #region Event Functions

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out T mono))
            {
                return;
            }

            OnCollect(mono);
            Destroy(gameObject);
        }

        #endregion

        protected abstract void OnCollect(T mono);
    }
}
