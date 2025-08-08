using System;

namespace NeonBlack.Systems.LevelState
{
    public partial class LevelState : SceneSingleton<LevelState>
    {
        #region Event Functions

        protected override void Awake()
        {
            base.Awake();

            AwakeAlert();

            LevelStarted?.Invoke();
        }

        private void FixedUpdate()
        {
            FixedUpdateAlert();
        }

        #endregion

        public static event Action LevelStarted;

        public static event Action<bool> LevelEnded;

        public static void EndLevel(bool success)
        {
            LevelEnded?.Invoke(success);
        }
    }
}
