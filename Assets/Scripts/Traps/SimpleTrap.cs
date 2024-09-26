using System.Collections;
using UnityEngine;

public class SimpleTrap : MonoBehaviour
{
    [SerializeField]
    bool shootReady;

    [SerializeField]
    float shootQuantity;

    [Header("References")]
    [SerializeField]
    Transform projectileSpawnPoint;

    [SerializeField]
    Projectile trapProjectilePrefab;

    public void Shoot()
    {
        if (shootReady) StartCoroutine(ShootCoroutine());
    } 

    IEnumerator ShootCoroutine()
    {
        shootReady = false;

        for (int i = 0; i < shootQuantity; i++)
        {
            ObjectPoolManager.Spawn(trapProjectilePrefab, out Projectile projectile);
            projectile.transform.SetPositionAndRotation(projectileSpawnPoint.position, projectileSpawnPoint.rotation);

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        shootReady = true;
    }
}
