using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int XAxisMovement = Animator.StringToHash("xAxis");
        private static readonly int ZAxisMovement = Animator.StringToHash("zAxis");
        private static readonly int VelocityMultiplier = Animator.StringToHash("velocityMultiplier");

        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private Animator animator;

        [Header("Parameters")]
        [SerializeField]
        private float normalVelocity = 5f;

        [SerializeField]
        private float velocityLerpSpeed = 5f;

        [SerializeField]
        private float blendingLerpSpeed = 5f;

        #endregion

        private Vector3 lastPosition;

        private float velocityMultiplier;
        private float xAxisMovement;
        private float zAxisMovement;

        #region Event Functions

        private void FixedUpdate()
        {
            var offset = transform.position - lastPosition;
            var currentVelocityMultiplier = offset.magnitude / Time.fixedDeltaTime / normalVelocity;
            var moveDir = transform.InverseTransformDirection(offset.normalized);

            lastPosition = transform.position;

            velocityMultiplier =
                Mathf.Lerp(velocityMultiplier, currentVelocityMultiplier, velocityLerpSpeed * Time.fixedDeltaTime);
            xAxisMovement = Mathf.Lerp(xAxisMovement, moveDir.x, blendingLerpSpeed * Time.fixedDeltaTime);
            zAxisMovement = Mathf.Lerp(zAxisMovement, moveDir.z, blendingLerpSpeed * Time.fixedDeltaTime);

            animator.SetFloat(VelocityMultiplier, velocityMultiplier);
            animator.SetFloat(XAxisMovement, xAxisMovement);
            animator.SetFloat(ZAxisMovement, zAxisMovement);
        }

        #endregion
    }
}