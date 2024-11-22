using NeonBlack.Entities.Player;
using NeonBlack.Enums;
using NeonBlack.Extensions;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Behaviors
{
    public class CheckPlayerVisibilityBehavior : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private PlayerController player;

        [Header("Settings")]
        [SerializeField]
        private bool ignorePlayerInvisibility;

        #endregion

        private readonly int raycastLayerMask =
            Layer.Player.ToMask() | Layer.Terrain.ToMask() | Layer.Buildings.ToMask();

        private Transform playerTransform;

        private RaycastHit raycastHit;

        #region Event Functions

        private void Awake()
        {
            playerTransform = player.VisibilityChecker;
        }

        #endregion

        public bool IsPlayerVisible()
        {
            return (player.IsVisible || ignorePlayerInvisibility) && RaycastToPlayer();
        }

        private bool RaycastToPlayer()
        {
            var direction = playerTransform.position - transform.position;
            var raycast = Physics.Raycast(transform.position, direction.normalized, out raycastHit,
                direction.magnitude,
                raycastLayerMask);

            return raycast && raycastHit.collider.gameObject.layer == (int)Layer.Player;
        }
    }
}
