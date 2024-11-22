using NeonBlack.Systems.LevelState;
using UnityEngine;

namespace NeonBlack.UI
{
    public class ShardsCounter : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private CollectibleIcon collectibleIcon;

        #endregion

        #region Event Functions

        private void OnEnable()
        {
            LevelState.LevelStarted += OnLevelStarted;
            LevelState.ShardsQuantityChanged += OnShardsQuantityChanged;
        }

        private void OnDisable()
        {
            LevelState.LevelStarted -= OnLevelStarted;
            LevelState.ShardsQuantityChanged -= OnShardsQuantityChanged;
        }

        #endregion

        private void OnShardsQuantityChanged(int value)
        {
            collectibleIcon.Counter = value;
        }

        private void OnLevelStarted()
        {
            collectibleIcon.Counter = 0;
        }
    }
}
