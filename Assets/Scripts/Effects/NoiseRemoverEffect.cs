using NeonBlack.Entities.Player;
using UnityEngine;

namespace NeonBlack.Effects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Effects/NoiseRemover")]
    public class NoiseRemoverEffect : Effect
    {
        public override void Activate(PlayerController playerController)
        {
            playerController.IsInSilenceMode = true;
        }

        public override void Deactivate(PlayerController playerController)
        {
            playerController.IsInSilenceMode = false;
        }
    }
}
