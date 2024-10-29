using Cysharp.Threading.Tasks;
using NeonBlack.Interfaces;
using NeonBlack.Systems;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Systems.SceneManagement;
using NeonBlack.Weapons;
using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public class PlayerController : MonoBehaviour, IEntityHealth
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private PlayerInput playerInput;

        [SerializeField]
        private Transform visibilityChecker;

        #endregion

        private bool killed;

        [SerializeField]
        private PlayerAnimation playerAnimation;

        [SerializeField]
        private Weapon[] weapons;

        public Transform VisibilityChecker => visibilityChecker;

        #region Event Functions

        private void OnEnable()
        {
            LevelState.AlertChanged += OnAlertChanged;
        }

        private void OnDisable()
        {
            LevelState.AlertChanged -= OnAlertChanged;
        }

        #endregion

        #region IEntityHealth Members

        public void TakeDamage(float dmg)
        {
            Kill();
        }

        #endregion

        private void OnAlertChanged(float value)
        {
            if (value >= 1)
            {
                Kill();
            }
        }

        public void Kill()
        {
            if (killed)
            {
                return;
            }

            killed = true;
            playerInput.ToggleMovementActions(false);
            playerInput.ToggleAttackActions(false);
            playerAnimation.OnDeath();

            AudioManager.Play(AudioManager.Music, AudioManager.PlayerDeathMusicClip);

            UniTask.WhenAll(
                AudioManager.Music.WaitFinish(),
                playerAnimation.WaitAnimationEnd(PlayerAnimation.Dead, 2)
            ).ContinueWith(SceneLoader.RestartLevel);
        }
    }
}
