using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrap : MonoBehaviour
{
    [SerializeField]
    bool shootReady;

    [SerializeField]
    float shootQuantity;

    [Header("References")]
    [SerializeField]
    Transform projectileSpawnPoint;

    [SerializeField]
    GameObject trapProjectilePrefab;


    public void Shoot()
    {
        if (shootReady) StartCoroutine(ShootCoroutine());
    } 

    IEnumerator ShootCoroutine()
    {
        shootReady = false;

        for (int i = 0; i < shootQuantity; i++)
        {
            Instantiate(trapProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation, this.transform);

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        shootReady = true;
    }
}
