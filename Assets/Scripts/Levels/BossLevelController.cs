using JetBrains.Annotations;
using NeonBlack.Entities.Enemies.Boss;
using NeonBlack.Systems.AudioManagement;
using UnityEngine;

namespace NeonBlack.Levels
{
    public class BossLevelController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Music")]
        [SerializeField]
        private AudioClip bossFightMusic;

        [Header("Exit Island logic")]
        [SerializeField]
        private GameObject exitIslandRoot;

        [SerializeField]
        private Vector3 exitIslandStartPosition;

        [SerializeField]
        private Vector3 exitIslandEndPosition;

        [SerializeField]
        private AnimationCurve exitIslandMovementCurve;

        #endregion

        private bool bossFightMusicPlaying;

        [CanBeNull]
        private AudioClip defaultClip;

        private bool moveIsland;
        private float moveIslandTimer;

        #region Event Functions

        private void Start()
        {
            exitIslandRoot.SetActive(false);
            exitIslandRoot.transform.position = exitIslandStartPosition;
        }

        private void Update()
        {
            if (!moveIsland)
            {
                return;
            }

            var progress = exitIslandMovementCurve.Evaluate(moveIslandTimer += Time.deltaTime);

            if (progress >= 1f)
            {
                moveIsland = false;
                return;
            }

            exitIslandRoot.transform.position = Vector3.Lerp(exitIslandStartPosition, exitIslandEndPosition, progress);
        }

        private void OnEnable()
        {
            BossHealth.HealthChanged += OnBossDamaged;
            BossHealth.Death += OnBossDeath;
        }

        private void OnDisable()
        {
            BossHealth.HealthChanged -= OnBossDamaged;
            BossHealth.Death -= OnBossDeath;
        }

        #endregion

        private void OnBossDamaged(float _, float __)
        {
            if (!bossFightMusicPlaying && bossFightMusic != null)
            {
                defaultClip = AudioManager.Music.AudioClip;
                AudioManager.Play(AudioManager.Music, bossFightMusic, true);
                bossFightMusicPlaying = true;
            }
        }

        private void OnBossDeath()
        {
            MoveIsland();

            if (bossFightMusicPlaying)
            {
                AudioManager.Play(AudioManager.Music, defaultClip, true);
                bossFightMusicPlaying = false;
            }
        }

        [ContextMenu("Move island")]
        private void MoveIsland()
        {
            moveIsland = true;
            moveIslandTimer = 0f;
            exitIslandRoot.SetActive(true);
        }

        [ContextMenu("Hide island")]
        private void HideIsland()
        {
            exitIslandRoot.SetActive(false);
        }
    }
}
