using NeonBlack.Extensions;
using NeonBlack.Interfaces;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Behaviors
{
    public class CheckVisibilityBehavior : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private LayerMask obstacleLayers;

        #endregion

        private RaycastHit raycastHit;

        public bool IsTargetVisible(ICheckVisibilityBehaviorTarget target, bool ignoreInvisibility = false,
            int? customObstacleLayers = null)
        {
            return (target.IsVisible || ignoreInvisibility) &&
                   RaycastToTarget(target, customObstacleLayers ?? obstacleLayers);
        }

        private bool RaycastToTarget(ICheckVisibilityBehaviorTarget target, int obstacleMask = 0)
        {
            var direction = target.VisibilityChecker.position - transform.position;
            var raycast = Physics.Raycast(transform.position, direction.normalized, out raycastHit,
                direction.magnitude,
                target.VisibilityLayer.ToMask() | obstacleMask);

            return raycast && raycastHit.collider.gameObject.layer == (int)target.VisibilityLayer;
        }
    }
}
