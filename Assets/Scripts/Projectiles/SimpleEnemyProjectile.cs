using UnityEngine;

public class SimpleEnemyProjectile : Projectile
{
    #region Serialized Fields

    [SerializeField]
    private float speed = 10f;

    #endregion

    #region Event Functions

    private void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.Kill();
        }
    }

    #endregion
}
