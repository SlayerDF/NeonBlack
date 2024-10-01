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

        #endregion

        private readonly int raycastLayerMask = Layer.Player.ToMask() | Layer.Terrain.ToMask();

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
            // TODO: add invisible zone check here
            return RaycastToPlayer();
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
