using NeonBlack.Systems.AudioManagement;
using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class PrepareForAttack : State<Blackboard>
    {
        private float timer;

        internal override void OnExit()
        {
        }

        internal override void OnEnter()
        {
            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LookAtTargetBehavior.enabled = true;
            Bb.PlayerDetectionBehavior.enabled = true;

            Bb.LineOfSightBehavior.enabled = false;
            Bb.PatrolBehavior.enabled = false;
            Bb.ShootPlayerBehavior.enabled = false;

            timer = 0f;

            // Player is already detected so the detection rate must be maxed 
            Bb.PlayerDetectionBehavior.DistanceToPlayerNormalized = 0f;

            AudioManager.Play(AudioManager.EnemiesNotificationsPrefab, AudioManager.EnemyAlertedClip,
                Bb.transform.position);
        }

        internal override void OnUpdate(float deltaTime)
        {
            Bb.PlayerDetectionBehavior.CanSeePlayer = Bb.CheckVisibilityBehavior.IsTargetVisible(Bb.PlayerController);

            if (!Bb.PlayerDetectionBehavior.PlayerIsDetected.CurrentValue)
            {
                SwitchState<NotifyBoss>();
                return;
            }

            Bb.LastSeenPlayerPosition = Bb.LookAtTargetBehavior.Target!.position;

            if ((timer += deltaTime) >= Bb.AttackDelay)
            {
                SwitchState<Attack>();
            }
        }
    }
}
