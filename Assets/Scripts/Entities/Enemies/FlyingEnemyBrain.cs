using System;
using System.ComponentModel;
using NeonBlack.Entities.Enemies.Behaviors;
using NeonBlack.Entities.Enemies.Boss;
using NeonBlack.Entities.Player;
using NeonBlack.Extensions;
using NeonBlack.Interfaces;
using NeonBlack.Particles;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Utilities;
using R3;
using UnityEngine;

namespace NeonBlack.Entities.Enemies
{
    public class FlyingEnemyBrain : MonoBehaviour, IDistractible
    {
        private const float CheckVisibilityInterval = 0.5f;

        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private BossBrain bossBrain;

        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private ParticlePoolObject onDeathParticles;

        [Header("Behaviors")]
        [SerializeField]
        private LineOfSightByPathBehavior lineOfSightByPathBehavior;

        [SerializeField]
        private CheckVisibilityBehavior checkVisibilityBehavior;

        [SerializeField]
        private PlayerDetectionBehavior playerDetectionBehavior;

        [SerializeField]
        private LookAtTargetBehavior lookAtTargetBehavior;

        [Header("Properties")]
        [SerializeField]
        private float thinkFrequency = 0.1f;

        [Header("Visuals")]
        [SerializeField]
        private MeshRenderer lineOfSightVisuals;

        [SerializeField]
        private Light targetPointLight;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color highAlertColor;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color lowAlertColor;

        #endregion

        private float actionTimer;

        private Color currentAlertColor;

        private GameObject distractionGameObject;
        private float distractionTime;
        private State state;

        private float thinkTimer;

        #region Event Functions

        private void Awake()
        {
            playerDetectionBehavior.PlayerIsDetected.Debounce(TimeSpan.FromSeconds(CheckVisibilityInterval))
                .Subscribe(CheckVisibility).AddTo(this);
        }

        private void FixedUpdate()
        {
            if ((thinkTimer += Time.fixedDeltaTime) < thinkFrequency)
            {
                return;
            }

            thinkTimer = 0;

            switch (state)
            {
                case State.Observe:
                    HandleObserveState(thinkFrequency);
                    break;
                case State.BeDistracted:
                    HandleObserveState(thinkFrequency);
                    HandleBeDistractedState(thinkFrequency);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(state), (int)state, typeof(State));
            }
        }

        private void OnEnable()
        {
            BossHealth.Death += OnBossDeath;
        }

        private void OnDisable()
        {
            BossHealth.Death -= OnBossDeath;
        }

        #endregion

        #region IDistractible Members

        public void Distract(GameObject distractor, float maxTime)
        {
            distractionGameObject = distractor;
            distractionTime = maxTime;

            SwitchState(State.BeDistracted, true);
        }

        #endregion

        private void OnBossDeath()
        {
            SpawnDeathParticles();

            if (this)
            {
                Destroy(gameObject);
            }
        }

        private void HandleObserveState(float _)
        {
            playerDetectionBehavior.CanSeePlayer =
                lineOfSightByPathBehavior.IsPlayerDetected && checkVisibilityBehavior.IsTargetVisible(playerController);

            var color = Color.Lerp(lowAlertColor, highAlertColor, playerDetectionBehavior.DetectionLevel);
            lineOfSightVisuals.material.SetEmissionColor(color);

            targetPointLight.color = color;
        }

        private void HandleBeDistractedState(float deltaTime)
        {
            if (distractionGameObject.IsValidAndEnabled() && (actionTimer += deltaTime) < distractionTime)
            {
                return;
            }

            SwitchState(State.Observe);
        }

        private void CheckVisibility(bool visible)
        {
            if (!visible)
            {
                return;
            }

            bossBrain.Notify(lineOfSightByPathBehavior.TargetPoint.position);
            AudioManager.Play(AudioManager.EnemiesNotificationsPrefab, AudioManager.EnemyAlertedClip,
                transform.position);
        }

        private void SwitchState(State newState, bool force = false)
        {
            if (state == newState && !force)
            {
                return;
            }

            // On Enter State
            switch (newState)
            {
                case State.Observe:
                    lineOfSightByPathBehavior.CustomTarget = null;
                    break;
                case State.BeDistracted:
                    lineOfSightByPathBehavior.CustomTarget = distractionGameObject.transform;
                    actionTimer = 0f;

                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(newState), (int)newState, typeof(State));
            }

            state = newState;
        }

        private void SpawnDeathParticles()
        {
            ObjectPoolManager.Spawn<ParticlePoolObject>(onDeathParticles, out var ps, true);
            ps.transform.position = transform.position;
            ps.gameObject.SetActive(true);
        }

        #region Nested type: ${0}

        private enum State
        {
            Observe,
            BeDistracted
        }

        #endregion
    }
}
