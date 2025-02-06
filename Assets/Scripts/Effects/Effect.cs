using NeonBlack.Entities.Player;
using UnityEngine;

namespace NeonBlack.Effects
{
    public abstract class Effect : ScriptableObject
    {
        #region Serialized Fields

        [Header("Assets")]
        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private Sprite iconAlpha;

        [Header("Common properties")]
        [SerializeField]
        private int duration;

        #endregion

        public Sprite Icon => icon;
        public Sprite IconAlpha => iconAlpha;
        public int Duration => duration;

        public abstract void Activate(PlayerController playerController);

        public abstract void Deactivate(PlayerController playerController);
    }
}
