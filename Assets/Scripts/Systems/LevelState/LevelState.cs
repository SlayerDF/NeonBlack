using System;
using NeonBlack.Systems.AudioManagement;

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

        private void Start()
        {
            // TODO: implement playing own music for each level
            AudioManager.Play(AudioManager.Music, AudioManager.DemoLevelMusicClip, true);
        }

        private void FixedUpdate()
        {
            FixedUpdateAlert();
        }

        #endregion

        public static event Action LevelStarted;
    }
}
