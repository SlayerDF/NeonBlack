using NeonBlack.Entities.Enemies.SimpleEnemy;
using NeonBlack.Entities.Player;
using NeonBlack.Extensions;
using UnityEngine;

namespace NeonBlack.Interactables
{
    [RequireComponent(typeof(BoxCollider))]
    public class ShadowZone : MonoBehaviour
    {
        private const float ColliderEdgeOffset = 0.5f;

        #region Serialized Fields

        [SerializeField]
        private BoxCollider zoneCollider;

        #endregion

        #region Event Functions

        private void Awake()
        {
            var colliderLossyScaleWithOffset = transform.localScale - ColliderEdgeOffset * 2 * Vector3.one;

            zoneCollider.size = Vector3.Max(Vector3.zero, colliderLossyScaleWithOffset)
                .Unscale(transform.localScale);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController playerController))
            {
                playerController.IsInShadowZone = true;
            }
            else if (other.TryGetComponent(out SimpleEnemyBrain enemyBrain))
            {
                enemyBrain.IsInShadowZone = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerController playerController))
            {
                playerController.IsInShadowZone = false;
            }
            else if (other.TryGetComponent(out SimpleEnemyBrain enemyBrain))
            {
                enemyBrain.IsInShadowZone = false;
            }
        }

        #endregion
    }
}
