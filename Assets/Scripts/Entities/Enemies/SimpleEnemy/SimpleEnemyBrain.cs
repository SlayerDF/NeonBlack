using Cysharp.Threading.Tasks;
using NeonBlack.Entities.Enemies.SimpleEnemy.States;
using NeonBlack.Enums;
using NeonBlack.Interfaces;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Systems.StateMachine;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy
{
    public class SimpleEnemyBrain : MonoBehaviour, IDistractible, ILosBehaviorTarget, ICheckVisibilityBehaviorTarget
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private Blackboard blackboard;

        [SerializeField]
        private Transform visibilityChecker;

        [Header("Properties")]
        [SerializeField]
        private float thinkFrequency = 0.1f;

        #endregion

        private StateMachine<Blackboard> stateMachine;

        private float thinkTimer;

        private Blackboard Blackboard => blackboard;
        private Blackboard Bb => blackboard;
        public bool IsInShadowZone { get; set; }

        public bool CouldBeResurrected => Bb.SimpleEnemyHealth.CouldBeResurrected;

        #region Event Functions

        private void Start()
        {
            stateMachine = new StateMachine<Blackboard>(blackboard);
            stateMachine.SwitchState<Patrol>();
        }

        private void FixedUpdate()
        {
            if ((thinkTimer += Time.fixedDeltaTime) < thinkFrequency)
            {
                return;
            }

            thinkTimer = 0;

            stateMachine.Update(thinkFrequency);
        }

        private void OnEnable()
        {
            Bb.SimpleEnemyHealth.Death += OnDeath;
            Bb.ShootPlayerBehavior.Shoot += OnShoot;
        }


        private void OnDisable()
        {
            Bb.SimpleEnemyHealth.Death -= OnDeath;
            Bb.ShootPlayerBehavior.Shoot -= OnShoot;
        }

        #endregion

        #region ICheckVisibilityBehaviorTarget Members

        public Transform VisibilityChecker => visibilityChecker;
        public bool IsVisible => !IsInShadowZone;
        public Layer VisibilityLayer => Layer.Enemies;

        #endregion

        #region IDistractible Members

        public void Distract(GameObject distractor, float maxTime)
        {
            if (stateMachine.CurrentState is not Patrol && stateMachine.CurrentState is not BeDistracted)
            {
                return;
            }

            Bb.DistractionGameObject = distractor;
            Bb.DistractionTime = maxTime;
            stateMachine.SwitchState<BeDistracted>(true, stateMachine.CurrentState is BeDistracted);
        }

        #endregion

        #region ILosBehaviorTarget Members

        public bool Destroyed => !this;

        #endregion

        public bool Resurrect()
        {
            if (!Bb.SimpleEnemyHealth.CouldBeResurrected)
            {
                return false;
            }

            Bb.SimpleEnemyAnimation.SetIsDead(false);
            Bb.SimpleEnemyAnimation.WaitAnimationEnd(SimpleEnemyAnimation.WakeUpAnimation, 0).ContinueWith(() =>
            {
                Bb.SimpleEnemyHealth.Resurrect();
                stateMachine.SwitchState<Patrol>();
            }).Forget();

            return true;
        }

        private void OnShoot()
        {
            AudioManager.Play(AudioManager.ShotsPrefab, AudioManager.SimpleEnemyShootClip, transform.position);
        }

        private void OnDeath()
        {
            stateMachine.SwitchState<Death>();
        }
    }
}
