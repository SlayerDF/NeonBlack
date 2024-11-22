using System.Threading;
using Cysharp.Threading.Tasks;
using NeonBlack.Interfaces;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Systems.LevelState;
using NeonBlack.Systems.SceneManagement;
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

        [SerializeField]
        private PlayerAnimation playerAnimation;

        [Header("Visuals")]
        [SerializeField]
        private ParticleSystem bloodParticles;

        #endregion

        private bool collidedDeathZone;

        private bool killed;

        public Transform VisibilityChecker => visibilityChecker;

        public bool IsInShadowZone { get; set; }

        public bool IsVisible => !IsInShadowZone;

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

        public void TakeDamage(DamageSource source, float dmg)
        {
            if (source == DamageSource.DeathZone)
            {
                collidedDeathZone = true;
            }

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

        private void Kill()
        {
            if (killed)
            {
                return;
            }

            killed = true;

            KillAsync().Forget();
        }

        private async UniTaskVoid KillAsync()
        {
            playerInput.ToggleMovementActions(false);
            playerInput.ToggleAttackActions(false);
            playerInput.ToggleInteractionActions(false);
            playerAnimation.OnDeath();

            bloodParticles.Play();

            AudioManager.Play(AudioManager.Music, AudioManager.PlayerDeathMusicClip);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            try
            {
                var waitAnimation = playerAnimation.WaitAnimationEnd(PlayerAnimation.DeathAnimation, 2,
                    cts.Token);
                var waitDeathZoneCollision = UniTask.WaitUntil(() => collidedDeathZone, cancellationToken: cts.Token);

                await UniTask.WhenAny(UniTask.WhenAll(AudioManager.Music.WaitFinish(cts.Token), waitAnimation),
                    UniTask.WhenAll(waitDeathZoneCollision, AudioManager.Music.WaitFinish(cts.Token)));
                SceneLoader.RestartLevel().Forget();
            }
            finally
            {
                cts.Cancel();
            }
        }
    }
}
