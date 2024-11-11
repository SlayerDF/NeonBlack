using System;
using System.ComponentModel;
using NeonBlack.Entities.Enemies.Behaviors;
using NeonBlack.Systems;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Systems.LevelState;
using R3;
using UnityEngine;

namespace NeonBlack.Entities.Enemies
{
    public class BossBrain : MonoBehaviour
    {
        private const float ToggleFollowSoundDebounceInterval = 0.5f;

        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private BossEye leftEye;

        [SerializeField]
        private BossEye rightEye;

        [SerializeField]
        private Transform playerTransform;

        [SerializeField]
        private Transform tempTarget;

        [Header("Behaviors")]
        [SerializeField]
        private LineOfSightByPathBehavior lineOfSightByPathBehavior;

        [SerializeField]
        private CheckPlayerVisibilityBehavior checkPlayerVisibilityBehavior;

        [SerializeField]
        private LookAtTargetBehavior lookAtTargetBehavior;

        [Header("Properties")]
        [SerializeField]
        private float focusSpeed = 5f;

        [SerializeField]
        private float alertAccumulation = 0.01f;

        [SerializeField]
        private float waitTime = 3f;

        #endregion

        private readonly ReactiveProperty<State> state = new();

        private Vector3 notifiedPosition;
        private float waitTimer;

        #region Event Functions

        private void Awake()
        {
            leftEye.FocusSpeed = focusSpeed;
            rightEye.FocusSpeed = focusSpeed;

            state.Debounce(TimeSpan.FromSeconds(ToggleFollowSoundDebounceInterval))
                .Select(it => it == State.FollowPlayer)
                .Subscribe(ToggleFollowSound)
                .AddTo(this);
        }

        private void Start()
        {
            SwitchState(State.ObserveLevel, true);
        }

        private void FixedUpdate()
        {
            switch (state.Value)
            {
                case State.ObserveLevel:
                    HandleObserveLevelState(Time.fixedDeltaTime);
                    break;
                case State.FollowPlayer:
                    HandleFollowPlayerState(Time.fixedDeltaTime);
                    break;
                case State.LostPlayer:
                    HandleLostPlayerState(Time.fixedDeltaTime);
                    break;
                case State.Notified:
                    HandleNotifiedState(Time.fixedDeltaTime);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(state), (int)state.Value, typeof(State));
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            leftEye.FocusSpeed = focusSpeed;
            rightEye.FocusSpeed = focusSpeed;
        }
#endif

        #endregion

        public void Notify(Vector3 position)
        {
            notifiedPosition = position;

            SwitchState(State.Notified);
        }

        private void HandleObserveLevelState(float _)
        {
            if (lineOfSightByPathBehavior.IsPlayerDetected && checkPlayerVisibilityBehavior.IsPlayerVisible())
            {
                SwitchState(State.FollowPlayer);
            }
        }

        private void HandleFollowPlayerState(float deltaTime)
        {
            if (!checkPlayerVisibilityBehavior.IsPlayerVisible())
            {
                SwitchState(State.LostPlayer);
                return;
            }

            LevelState.UpdateAlert(alertAccumulation * deltaTime);
        }

        private void HandleLostPlayerState(float deltaTime)
        {
            if (CanSeePlayer() && checkPlayerVisibilityBehavior.IsPlayerVisible())
            {
                SwitchState(State.FollowPlayer);
                return;
            }

            if ((waitTimer += deltaTime) < waitTime)
            {
                return;
            }

            SwitchState(State.ObserveLevel);
        }

        private void HandleNotifiedState(float deltaTime)
        {
            if (CanSeePlayer() && checkPlayerVisibilityBehavior.IsPlayerVisible())
            {
                SwitchState(State.FollowPlayer);
                return;
            }

            if ((waitTimer += deltaTime) < waitTime)
            {
                return;
            }

            SwitchState(State.ObserveLevel);
        }

        private void SwitchState(State newState, bool force = false)
        {
            if (state.Value == newState && !force)
            {
                return;
            }

            // On Exit State
            switch (state.Value)
            {
                case State.FollowPlayer:
                    if (newState == State.Notified)
                    {
                        return;
                    }

                    break;
                case State.ObserveLevel:
                case State.LostPlayer:
                case State.Notified:
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(state), (int)state.Value, typeof(State));
            }

            // On Enter State
            switch (newState)
            {
                case State.ObserveLevel:
                    lineOfSightByPathBehavior.enabled = true;
                    lookAtTargetBehavior.enabled = true;

                    UpdateTarget(lineOfSightByPathBehavior.TargetPoint);
                    Unfocus();

                    break;
                case State.FollowPlayer:
                    lineOfSightByPathBehavior.enabled = false;
                    lookAtTargetBehavior.enabled = true;

                    UpdateTarget(playerTransform);
                    Focus();

                    break;
                case State.LostPlayer:
                    lineOfSightByPathBehavior.enabled = false;
                    lookAtTargetBehavior.enabled = false;

                    tempTarget.position = playerTransform.position;

                    UpdateTarget(tempTarget);
                    Focus();

                    waitTimer = 0f;

                    break;
                case State.Notified:
                    lineOfSightByPathBehavior.enabled = false;

                    tempTarget.position = notifiedPosition;

                    UpdateTarget(tempTarget);
                    Focus();

                    waitTimer = 0f;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(newState), (int)newState, typeof(State));
            }

            state.Value = newState;
        }

        private void Focus()
        {
            leftEye.IsFocused = true;
            rightEye.IsFocused = true;
        }

        private void Unfocus()
        {
            leftEye.IsFocused = false;
            rightEye.IsFocused = false;
        }

        private void UpdateTarget(Transform target)
        {
            lookAtTargetBehavior.Target = target;
            leftEye.Target = target;
            rightEye.Target = target;
        }

        private bool CanSeePlayer()
        {
            return leftEye.CanSeePoint(playerTransform.position) || rightEye.CanSeePoint(playerTransform.position);
        }

        private static void ToggleFollowSound(bool value)
        {
            if (value)
            {
                AudioManager.Play(AudioManager.BossNotifications, AudioManager.DangerClip, true);
            }
            else
            {
                AudioManager.Stop(AudioManager.BossNotifications, AudioManager.DangerClip);
            }
        }

        #region Nested type: ${0}

        private enum State
        {
            ObserveLevel,
            FollowPlayer,
            LostPlayer,
            Notified
        }

        #endregion
    }
}
