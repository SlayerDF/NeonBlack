using System.Collections;
using NeonBlack.Projectiles;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Traps
{
    public class SimpleTrap : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private bool shootReady;

        [SerializeField]
        private float shootQuantity;

        [Header("References")]
        [SerializeField]
        private Transform projectileSpawnPoint;

        [SerializeField]
        private Projectile trapProjectilePrefab;

        #endregion

        public void Shoot()
        {
            if (shootReady)
            {
                StartCoroutine(ShootCoroutine());
            }
        }

        private IEnumerator ShootCoroutine()
        {
            shootReady = false;

            for (var i = 0; i < shootQuantity; i++)
            {
                ObjectPoolManager.Spawn(trapProjectilePrefab, out Projectile projectile, true);
                projectile.transform.position = projectileSpawnPoint.position;
                projectile.transform.rotation = projectileSpawnPoint.rotation;
                projectile.gameObject.SetActive(true);

                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(1f);

            shootReady = true;
        }
    }
}
