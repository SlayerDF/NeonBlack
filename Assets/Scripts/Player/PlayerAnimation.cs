using JetBrains.Annotations;
using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int XAxisMovement = Animator.StringToHash("XAxis");
        private static readonly int ZAxisMovement = Animator.StringToHash("ZAxis");
        private static readonly int VelocityMultiplier = Animator.StringToHash("VelocityMultiplier");
        private static readonly int Falling = Animator.StringToHash("Falling");
        private static readonly int Flipping = Animator.StringToHash("Flipping");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Flip = Animator.StringToHash("Flip");

        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private CharacterController characterController;

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

        public bool ReadyToJump { get; private set; } = true;

        #region Event Functions

        private void FixedUpdate()
        {
            var offset = transform.position - lastPosition;
            var currentVelocityMultiplier = offset.magnitude / Time.fixedDeltaTime / normalVelocity;
            var moveDir = transform.InverseTransformDirection(offset.normalized);

            lastPosition = transform.position;

            if (!characterController.isGrounded) ReadyToJump = false;

            velocityMultiplier =
                Mathf.Lerp(velocityMultiplier, currentVelocityMultiplier, velocityLerpSpeed * Time.fixedDeltaTime);
            xAxisMovement = Mathf.Lerp(xAxisMovement, moveDir.x, blendingLerpSpeed * Time.fixedDeltaTime);
            zAxisMovement = Mathf.Lerp(zAxisMovement, moveDir.z, blendingLerpSpeed * Time.fixedDeltaTime);

            animator.SetFloat(VelocityMultiplier, velocityMultiplier);
            animator.SetFloat(XAxisMovement, xAxisMovement);
            animator.SetFloat(ZAxisMovement, zAxisMovement);
            animator.SetBool(Falling, !characterController.isGrounded);
        }

        #endregion

        public void OnJump()
        {
            ReadyToJump = false;
            animator.SetTrigger(Jump);
        }

        public void OnFlip()
        {
            ReadyToJump = false;
            animator.SetBool(Flipping, true);
            animator.SetTrigger(Flip);
        }

        [UsedImplicitly]
        private void OnLandedEvent()
        {
            ReadyToJump = true;
            animator.SetBool(Flipping, false);
        }
    }
}
