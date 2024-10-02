namespace NeonBlack.Systems
{
    public partial class LevelState : SceneSingleton<LevelState>
    {
        #region Event Functions

        protected override void Awake()
        {
            base.Awake();

            AwakeAlert();
        }

        private void FixedUpdate()
        {
            FixedUpdateAlert();
        }

        #endregion
    }
}
