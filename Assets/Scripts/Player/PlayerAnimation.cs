using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int XAxisMovement = Animator.StringToHash("XAxis");
        private static readonly int ZAxisMovement = Animator.StringToHash("ZAxis");
        private static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");
        private static readonly int VelocityMultiplier = Animator.StringToHash("VelocityMultiplier");
        private static readonly int Falling = Animator.StringToHash("Falling");
        private static readonly int Jumping = Animator.StringToHash("Jumping");
        private static readonly int Dashing = Animator.StringToHash("Dashing");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Dash = Animator.StringToHash("Dash");

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

        #region Event Functions

        private void FixedUpdate()
        {
            var currentPosition = transform.position;
            currentPosition.y = 0f;

            var offset = currentPosition - lastPosition;
            var currentVelocityMultiplier =
                Mathf.Clamp(offset.magnitude / Time.fixedDeltaTime / normalVelocity, 1f, 10f);
            var moveDir = transform.InverseTransformDirection(offset.normalized);

            lastPosition = currentPosition;

            velocityMultiplier =
                Mathf.Lerp(velocityMultiplier, currentVelocityMultiplier, velocityLerpSpeed * Time.fixedDeltaTime);
            xAxisMovement = Mathf.Lerp(xAxisMovement, moveDir.x, blendingLerpSpeed * Time.fixedDeltaTime);
            zAxisMovement = Mathf.Lerp(zAxisMovement, moveDir.z, blendingLerpSpeed * Time.fixedDeltaTime);

            animator.SetFloat(VelocityMultiplier, velocityMultiplier);
            animator.SetFloat(XAxisMovement, xAxisMovement);
            animator.SetFloat(ZAxisMovement, zAxisMovement);
            animator.SetBool(Falling, !characterController.isGrounded);

            if (!characterController.isGrounded)
            {
                return;
            }

            animator.SetBool(Jumping, false);
            animator.SetBool(Dashing, false);
        }

        #endregion

        public void SetInputMagnitude(float value)
        {
            animator.SetFloat(InputMagnitude, value);
        }

        public void OnJump()
        {
            animator.SetBool(Jumping, true);
            animator.SetTrigger(Jump);
        }

        public void OnDash()
        {
            animator.SetBool(Dashing, true);
            animator.SetTrigger(Dash);
        }
    }
}
