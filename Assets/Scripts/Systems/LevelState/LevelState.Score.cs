namespace NeonBlack.Systems.LevelState
{
    public partial class LevelState
    {
        public float Score { get; private set; }

        public void IncrementScore(float value)
        {
            Score += value;
        }
    }
}
