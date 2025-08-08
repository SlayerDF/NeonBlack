using NeonBlack.Entities.Player;
using UnityEngine;

namespace NeonBlack.Effects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Effects/SpeedBoost")]
    public class SpeedBoostEffect : Effect
    {
        #region Serialized Fields

        [Header("Effect properties")]
        [SerializeField]
        private float moveSpeedModifier;

        #endregion

        public override void Activate(PlayerController playerController)
        {
            playerController.Input.MoveSpeedModifier = moveSpeedModifier;
        }

        public override void Deactivate(PlayerController playerController)
        {
            playerController.Input.MoveSpeedModifier = 1f;
        }
    }
}
