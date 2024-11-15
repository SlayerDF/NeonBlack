using NeonBlack.Interfaces;
using NeonBlack.Systems.LevelState;
using UnityEngine;

namespace NeonBlack.Interactables
{
    public class ShardSlot : MonoBehaviour, IPlayerInteractable
    {
        #region Serialized Fields

        [SerializeField]
        private GameObject shardGameObject;

        #endregion

        #region IPlayerInteractable Members

        public bool CanBeInteracted { get; private set; } = true;

        public void Activate()
        {
            if (LevelState.Shards < 1)
            {
                return;
            }

            CanBeInteracted = false;
            LevelState.NotifyShardUsed();
            shardGameObject.SetActive(true);
        }

        #endregion
    }
}
