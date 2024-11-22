using System.Globalization;
using NeonBlack.Systems.LevelState;
using TMPro;
using UnityEngine;

namespace NeonBlack
{
    public class Score : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private TMP_Text scoreText;

        #endregion

        #region Event Functions

        private void OnEnable()
        {
            LevelState.LevelStarted += OnLevelStarted;
            LevelState.ScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            LevelState.LevelStarted -= OnLevelStarted;
            LevelState.ScoreChanged -= OnScoreChanged;
        }

        #endregion

        private void OnLevelStarted()
        {
            OnScoreChanged(0);
        }

        private void OnScoreChanged(float value)
        {
            scoreText.text = value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
