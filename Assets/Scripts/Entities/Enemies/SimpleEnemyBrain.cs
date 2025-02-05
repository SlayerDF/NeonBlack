﻿using System.ComponentModel;
using Cysharp.Threading.Tasks;
using NeonBlack.Entities.Enemies.Behaviors;
using NeonBlack.Extensions;
using NeonBlack.Interfaces;
using NeonBlack.Systems.AudioManagement;
using UnityEngine;

namespace NeonBlack.Entities.Enemies
{
    public class SimpleEnemyBrain : MonoBehaviour, IDistractible
    {
        #region Serialized Fields

        [SerializeField]
        private EnemyHealth enemyHealth;

        [SerializeField]
        private BossBrain bossBrain;

        [Header("Behaviors")]
        [SerializeField]
        private LineOfSightBehavior lineOfSightBehavior;

        [SerializeField]
        private CheckPlayerVisibilityBehavior checkPlayerVisibilityBehavior;

        [SerializeField]
        private PatrolBehavior patrolBehavior;

        [SerializeField]
        private PlayerDetectionBehavior playerDetectionBehavior;

        [SerializeField]
        private LookAtTargetBehavior lookAtTargetBehavior;

        [SerializeField]
        private ShootPlayerBehavior shootPlayerBehavior;

        [Header("Properties")]
        [SerializeField]
        private float thinkFrequency = 0.1f;

        [SerializeField]
        private float attackDelay = 1f;

        [SerializeField]
        private float notifyBossDelay = 3f;

        [SerializeField]
        private float distractedRotationSpeed = 5f;

        [Header("Visuals")]
        [SerializeField]
        private MeshRenderer lineOfSightVisuals;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color highAlertColor;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color lowAlertColor;

        [Header("Components")]
        [SerializeField]
        private EnemyAnimation enemyAnimation;

        #endregion

        private float actionTimer;
        private GameObject distractionGameObject;
        private float distractionTime;

        private Vector3 lastSeenPlayerPosition;

        private float lookAtOriginalRotationSpeed;

        private Transform lookAtOriginalTarget;

        private State state = State.Patrol;

        private float thinkTimer;

        #region Event Functions

        private void Start()
        {
            SwitchState(State.Patrol, true);
        }

        private void FixedUpdate()
        {
            if (enemyHealth.Dead)
            {
                return;
            }

            if ((thinkTimer += Time.fixedDeltaTime) < thinkFrequency)
            {
                return;
            }

            thinkTimer = 0;

            switch (state)
            {
                case State.Patrol:
                    HandlePatrolState(thinkFrequency);
                    break;
                case State.PrepareForAttack:
                    HandlePrepareForAttackState(thinkFrequency);
                    break;
                case State.Attack:
                    HandleAttackState(thinkFrequency);
                    break;
                case State.NotifyBoss:
                    HandleNotifyBossState(thinkFrequency);
                    break;
                case State.BeDistracted:
                    HandlePatrolState(thinkFrequency);
                    HandleBeDistractedState(thinkFrequency);
                    break;
                case State.Death:
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(state), (int)state, typeof(State));
            }
        }

        private void OnEnable()
        {
            enemyHealth.Death += OnDeath;
            shootPlayerBehavior.Shoot += OnShoot;
        }


        private void OnDisable()
        {
            enemyHealth.Death -= OnDeath;
            shootPlayerBehavior.Shoot -= OnShoot;
        }

        #endregion

        #region IDistractible Members

        public void Distract(GameObject distractor, float maxTime)
        {
            if (state is not (State.Patrol or State.BeDistracted))
            {
                return;
            }

            distractionGameObject = distractor;
            distractionTime = maxTime;
            SwitchState(State.BeDistracted, true, state is State.BeDistracted);
        }

        #endregion

        private void OnShoot()
        {
            AudioManager.Play(AudioManager.ShotsPrefab, AudioManager.SimpleEnemyShootClip, transform.position);
        }

        private void HandlePatrolState(float _)
        {
            playerDetectionBehavior.CanSeePlayer =
                lineOfSightBehavior.HasTarget && checkPlayerVisibilityBehavior.IsPlayerVisible();
            playerDetectionBehavior.DistanceToPlayerNormalized = lineOfSightBehavior.DistanceToPlayerNormalized;

            var color = Color.Lerp(lowAlertColor, highAlertColor, playerDetectionBehavior.DetectionLevel);
            lineOfSightVisuals.material.SetEmissionColor(color);

            if (playerDetectionBehavior.PlayerIsDetected.CurrentValue)
            {
                SwitchState(State.PrepareForAttack);
            }
        }

        private void HandlePrepareForAttackState(float deltaTime)
        {
            playerDetectionBehavior.CanSeePlayer = checkPlayerVisibilityBehavior.IsPlayerVisible();

            if (!playerDetectionBehavior.PlayerIsDetected.CurrentValue)
            {
                SwitchState(State.NotifyBoss);
                return;
            }

            lastSeenPlayerPosition = lookAtTargetBehavior.Target!.position;

            if ((actionTimer += deltaTime) >= attackDelay)
            {
                SwitchState(State.Attack);
            }
        }

        private void HandleAttackState(float _)
        {
            playerDetectionBehavior.CanSeePlayer = checkPlayerVisibilityBehavior.IsPlayerVisible();

            if (!playerDetectionBehavior.PlayerIsDetected.CurrentValue)
            {
                SwitchState(State.NotifyBoss);
                return;
            }

            lastSeenPlayerPosition = lookAtTargetBehavior.Target!.position;
        }

        private void HandleNotifyBossState(float deltaTime)
        {
            if ((actionTimer += deltaTime) < notifyBossDelay)
            {
                return;
            }

            bossBrain.Notify(lastSeenPlayerPosition);
            SwitchState(State.Patrol);
        }

        private void HandleBeDistractedState(float deltaTime)
        {
            if (distractionGameObject.IsValidAndEnabled() && (actionTimer += deltaTime) < distractionTime)
            {
                return;
            }

            if (state == State.BeDistracted)
            {
                SwitchState(State.Patrol);
            }
        }

        private void SwitchState(State newState, bool force = false, bool skipExit = false)
        {
            if (state == newState && !force)
            {
                return;
            }

            if (!skipExit)
            {
                // On Exit state
                switch (state)
                {
                    case State.Patrol:
                        break;
                    case State.PrepareForAttack:
                        break;
                    case State.Attack:
                        enemyAnimation.SetIsAttacking(false);
                        enemyHealth.Invincible = false;
                        break;
                    case State.NotifyBoss:
                        enemyAnimation.SetIsNotifyingBoss(false);
                        break;
                    case State.Death:
                        break;
                    case State.BeDistracted:
                        lookAtTargetBehavior.Target = lookAtOriginalTarget;
                        lookAtTargetBehavior.RotationSpeed = lookAtOriginalRotationSpeed;
                        distractionGameObject = null;
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(state), (int)state, typeof(State));
                }
            }

            // On Enter State
            switch (newState)
            {
                case State.Patrol:
                    checkPlayerVisibilityBehavior.enabled = true;
                    lineOfSightBehavior.enabled = true;
                    patrolBehavior.enabled = true;
                    playerDetectionBehavior.enabled = true;

                    lookAtTargetBehavior.enabled = false;
                    shootPlayerBehavior.enabled = false;
                    break;
                case State.PrepareForAttack:
                    checkPlayerVisibilityBehavior.enabled = true;
                    lookAtTargetBehavior.enabled = true;
                    playerDetectionBehavior.enabled = true;

                    lineOfSightBehavior.enabled = false;
                    patrolBehavior.enabled = false;
                    shootPlayerBehavior.enabled = false;

                    actionTimer = 0f;

                    // Player is already detected so the detection rate must be maxed 
                    playerDetectionBehavior.DistanceToPlayerNormalized = 0f;

                    AudioManager.Play(AudioManager.EnemiesNotificationsPrefab, AudioManager.EnemyAlertedClip,
                        transform.position);

                    break;
                case State.Attack:
                    checkPlayerVisibilityBehavior.enabled = true;
                    lookAtTargetBehavior.enabled = true;
                    playerDetectionBehavior.enabled = true;
                    shootPlayerBehavior.enabled = true;

                    lineOfSightBehavior.enabled = false;
                    patrolBehavior.enabled = false;

                    enemyAnimation.SetIsAttacking(true);
                    enemyHealth.Invincible = true;

                    // Player is already detected so the detection rate must be maxed 
                    playerDetectionBehavior.DistanceToPlayerNormalized = 0f;

                    break;
                case State.NotifyBoss:
                    checkPlayerVisibilityBehavior.enabled = false;
                    lookAtTargetBehavior.enabled = false;
                    playerDetectionBehavior.enabled = false;
                    shootPlayerBehavior.enabled = false;
                    lineOfSightBehavior.enabled = false;
                    patrolBehavior.enabled = false;

                    actionTimer = 0f;
                    enemyAnimation.SetIsNotifyingBoss(true);

                    break;
                case State.BeDistracted:
                    checkPlayerVisibilityBehavior.enabled = true;
                    lookAtTargetBehavior.enabled = true;
                    playerDetectionBehavior.enabled = true;
                    lineOfSightBehavior.enabled = true;

                    shootPlayerBehavior.enabled = false;
                    patrolBehavior.enabled = false;

                    lookAtOriginalTarget = lookAtTargetBehavior.Target;
                    lookAtOriginalRotationSpeed = lookAtTargetBehavior.RotationSpeed;
                    lookAtTargetBehavior.Target = distractionGameObject?.transform;
                    lookAtTargetBehavior.RotationSpeed = distractedRotationSpeed;
                    actionTimer = 0f;

                    break;
                case State.Death:
                    checkPlayerVisibilityBehavior.enabled = false;
                    lookAtTargetBehavior.enabled = false;
                    playerDetectionBehavior.enabled = false;
                    shootPlayerBehavior.enabled = false;
                    lineOfSightBehavior.enabled = false;
                    patrolBehavior.enabled = false;

                    PlayAnimationAndKill().Forget();

                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(newState), (int)newState, typeof(State));
            }

            state = newState;
        }

        private void OnDeath()
        {
            SwitchState(State.Death);
        }

        private async UniTaskVoid PlayAnimationAndKill()
        {
            enemyAnimation.OnDeath();

            await enemyAnimation.WaitAnimationEnd(EnemyAnimation.Death, 0);

            enemyHealth.Kill();
        }

        #region Nested type: ${0}

        private enum State
        {
            Patrol,
            PrepareForAttack,
            Attack,
            NotifyBoss,
            BeDistracted,
            Death
        }

        #endregion
    }
}
