using System;

namespace NeonBlack.Systems.LevelState
{
    public partial class LevelState
    {
        private int shards;

        public static int Shards => Instance.shards;

        public static event Action<int> ShardsQuantityChanged;

        public static void NotifyShardCollected()
        {
            Instance.shards++;

            ShardsQuantityChanged?.Invoke(Instance.shards);
        }

        public static void NotifyShardUsed()
        {
            Instance.shards--;

            ShardsQuantityChanged?.Invoke(Instance.shards);
        }
    }
}
