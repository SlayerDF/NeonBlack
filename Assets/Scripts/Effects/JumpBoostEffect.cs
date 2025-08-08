using NeonBlack.Entities.Player;
using UnityEngine;

namespace NeonBlack.Effects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Effects/JumpBoost")]
    public class JumpBoostEffect : Effect
    {
        #region Serialized Fields

        [Header("Effect properties")]
        [SerializeField]
        private float jumpForceMultiplier;

        #endregion

        public override void Activate(PlayerController playerController)
        {
            playerController.Input.JumpForceMultiplier = jumpForceMultiplier;
        }

        public override void Deactivate(PlayerController playerController)
        {
            playerController.Input.JumpForceMultiplier = 1f;
        }
        
    }
}
