using System;

namespace NeonBlack.Systems.LevelState
{
    public partial class LevelState
    {
        public static event Action<float> NoiseChanged;

        public static void UpdateNoise(float newValue)
        {
            NoiseChanged?.Invoke(newValue);
        }
    }
}
