using System;
using NeonBlack.Entities.Enemies.Boss.States;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Systems.StateMachine;
using R3;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Boss
{
    public class BossBrain : MonoBehaviour
    {
        private const float ToggleFollowSoundDebounceInterval = 0.5f;

        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private Blackboard blackboard;

        [SerializeField]
        private BossHealth bossHealth;

        #endregion

        private StateMachine<Blackboard> stateMachine;

        private Blackboard Bb => blackboard;

        #region Event Functions

        private void Awake()
        {
            Bb.LeftEye.FocusSpeed = Bb.FocusSpeed;
            Bb.RightEye.FocusSpeed = Bb.FocusSpeed;
        }

        private void Start()
        {
            stateMachine = new StateMachine<Blackboard>(blackboard);
            stateMachine.SwitchState<ObserveLevel>();

            stateMachine.CurrentStateReactive.Debounce(TimeSpan.FromSeconds(ToggleFollowSoundDebounceInterval))
                .Select(it => it is FollowPlayer)
                .Subscribe(ToggleFollowSound)
                .AddTo(this);
        }

        private void FixedUpdate()
        {
            stateMachine.Update(Time.fixedDeltaTime);
        }

        private void OnEnable()
        {
            bossHealth.Death += OnDeath;
        }

        private void OnDisable()
        {
            bossHealth.Death -= OnDeath;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Bb)
            {
                return;
            }

            Bb.LeftEye.FocusSpeed = Bb.FocusSpeed;
            Bb.RightEye.FocusSpeed = Bb.FocusSpeed;
        }
#endif

        #endregion

        private void OnDeath()
        {
            stateMachine.SwitchState<BeDead>();
        }

        public void Notify(Vector3 position)
        {
            Bb.NotifiedPosition = position;

            stateMachine.SwitchState<BeNotified>();
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
    }
}
