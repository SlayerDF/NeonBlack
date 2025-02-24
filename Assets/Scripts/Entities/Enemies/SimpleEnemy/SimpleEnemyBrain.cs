using NeonBlack.Entities.Enemies.SimpleEnemy.States;
using NeonBlack.Interfaces;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Systems.StateMachine;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy
{
    public class SimpleEnemyBrain : MonoBehaviour, IDistractible
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private Blackboard blackboard;

        [Header("Properties")]
        [SerializeField]
        private float thinkFrequency = 0.1f;

        #endregion

        private StateMachine<Blackboard> stateMachine;

        private float thinkTimer;

        private Blackboard Blackboard => blackboard;
        private Blackboard Bb => blackboard;

        #region Event Functions

        private void Start()
        {
            stateMachine = new StateMachine<Blackboard>(blackboard);
            stateMachine.SwitchState<Patrol>();
        }

        private void FixedUpdate()
        {
            if (Bb.EnemyHealth.Dead)
            {
                return;
            }

            if ((thinkTimer += Time.fixedDeltaTime) < thinkFrequency)
            {
                return;
            }

            thinkTimer = 0;

            stateMachine.Update(thinkFrequency);
        }

        private void OnEnable()
        {
            Bb.EnemyHealth.Death += OnDeath;
            Bb.ShootPlayerBehavior.Shoot += OnShoot;
        }


        private void OnDisable()
        {
            Bb.EnemyHealth.Death -= OnDeath;
            Bb.ShootPlayerBehavior.Shoot -= OnShoot;
        }

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
