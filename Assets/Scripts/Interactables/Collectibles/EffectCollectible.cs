using NeonBlack.Effects;
using NeonBlack.Entities.Player;
using UnityEngine;

namespace NeonBlack.Interactables
{
    public class EffectCollectible : Collectible<PlayerEffects>
    {
        #region Serialized Fields

        [SerializeField]
        private Effect effect;

        #endregion

        protected override void OnCollect(PlayerEffects playerEffects)
        {
            playerEffects.AddEffect(effect);
        }
    }
}
