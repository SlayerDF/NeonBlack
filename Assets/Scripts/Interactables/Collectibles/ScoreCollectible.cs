using NeonBlack.Entities.Player;
using NeonBlack.Systems.LevelState;
using UnityEngine;

namespace NeonBlack.Interactables
{
    public class ScoreCollectible : Collectible<PlayerController>
    {
        #region Serialized Fields

        [SerializeField]
        private float scoreForCollection;

        #endregion

        protected override void OnCollect(PlayerController _)
        {
            LevelState.IncrementScore(scoreForCollection);
            LevelState.NotifyShardCollected();
        }
    }
}
