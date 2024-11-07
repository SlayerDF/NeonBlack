using System;

namespace NeonBlack.Systems.LevelState
{
    public partial class LevelState
    {
        public float Score { get; private set; }

        public static event Action<float> ScoreChanged;

        public void IncrementScore(float value)
        {
            Score += value;

            ScoreChanged?.Invoke(Score);
        }
    }
}
