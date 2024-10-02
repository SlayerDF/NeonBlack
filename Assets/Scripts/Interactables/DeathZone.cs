using NeonBlack.Interfaces;
using UnityEngine;

namespace NeonBlack.Interactables
{
    public class DeathZone : MonoBehaviour
    {
        #region Event Functions

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IEntityHealth entityHealth))
            {
                entityHealth.TakeDamage(float.MaxValue);
            }
        }

        #endregion
    }
}
