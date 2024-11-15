using System;
using NeonBlack.Interfaces;
using NeonBlack.Systems.LevelState;
using UnityEngine;

namespace NeonBlack.Interactables
{
    public class ShardSlot : MonoBehaviour, IPlayerInteractable, IActivatable
    {
        #region Serialized Fields

        [SerializeField]
        private GameObject shardGameObject;

        #endregion

        #region IActivatable Members

        public bool IsActivated { get; private set; }
        public event Action Activated;

        #endregion

        #region IPlayerInteractable Members

        public bool CanBeInteracted => !IsActivated && LevelState.Shards > 0;

        public void Interact()
        {
            if (LevelState.Shards < 1)
            {
                return;
            }

            IsActivated = true;
            LevelState.NotifyShardUsed();
            shardGameObject.SetActive(true);

            Activated?.Invoke();
        }

        #endregion
    }
}
