using UnityEngine;

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
