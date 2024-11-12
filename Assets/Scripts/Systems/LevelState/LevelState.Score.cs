using System;

namespace NeonBlack.Systems.LevelState
{
    public partial class LevelState
    {
        private float score;

        public static float Score => Instance.score;

        public static event Action<float> ScoreChanged;

        public static void IncrementScore(float value)
        {
            Instance.score += value;

            ScoreChanged?.Invoke(Instance.score);
        }
    }
}
