using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        public static readonly int XAxisMovement = Animator.StringToHash("XAxis");
        public static readonly int ZAxisMovement = Animator.StringToHash("ZAxis");
        public static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static readonly int VelocityMultiplier = Animator.StringToHash("VelocityMultiplier");
        public static readonly int Falling = Animator.StringToHash("Falling");
        public static readonly int Jumping = Animator.StringToHash("Jumping");
        public static readonly int Dashing = Animator.StringToHash("Dashing");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int Dash = Animator.StringToHash("Dash");
        public static readonly int Death = Animator.StringToHash("Death");

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

        public void OnDeath()
        {
            animator.SetTrigger(Death);
        }

        public async UniTask WaitAnimationEnd(int hash, int layer)
        {
            if (!animator.HasState(layer, hash))
            {
                return;
            }

            await UniTask.WaitUntil(
                () => !animator || animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == hash);
            await UniTask.WaitUntil(() =>
                !animator || animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 1f);
        }
    }
}
