using System.Collections;
using NeonBlack.Projectiles;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Traps
{
    public class SimpleTrap : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Properties")]
        [SerializeField]
        private float shootQuantity;

        [SerializeField]
        private bool overrideInitialVelocity;

        [SerializeField]
        private float initialVelocity;

        [Header("References")]
        [SerializeField]
        private Transform projectileSpawnPoint;

        [SerializeField]
        private Projectile trapProjectilePrefab;

        #endregion

        private bool shootCooldown;

        public void Shoot()
        {
            if (!shootCooldown)
            {
                StartCoroutine(ShootCoroutine());
            }
        }

        private IEnumerator ShootCoroutine()
        {
            shootCooldown = true;

            for (var i = 0; i < shootQuantity; i++)
            {
                ObjectPoolManager.Spawn(trapProjectilePrefab, out TrapProjectile projectile, true);
                projectile.transform.position = projectileSpawnPoint.position;
                projectile.transform.rotation = projectileSpawnPoint.rotation;
                projectile.InitialVelocity = initialVelocity;
                projectile.gameObject.SetActive(true);

                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(1f);

            shootCooldown = false;
        }
    }
}
