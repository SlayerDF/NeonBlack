using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class Attack : State<Blackboard>
    {
        internal override void OnExit()
        {
            Bb.EnemyAnimation.SetIsAttacking(false);
            Bb.SimpleEnemyHealth.Invincible = false;
        }

        internal override void OnEnter()
        {
            Bb.LineOfSightBehavior.enabled = false;
            Bb.PatrolBehavior.enabled = false;
            Bb.GoToBehavior.enabled = false;

            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LookAtTargetBehavior.enabled = true;
            Bb.PlayerDetectionBehavior.enabled = true;
            Bb.ShootPlayerBehavior.enabled = true;

            Bb.EnemyAnimation.SetIsAttacking(true);
            Bb.SimpleEnemyHealth.Invincible = true;

            // Player is already detected so the detection rate must be maxed 
            Bb.PlayerDetectionBehavior.DistanceToPlayerNormalized = 0f;
        }

        internal override void OnUpdate(float deltaTime)
        {
            Bb.PlayerDetectionBehavior.CanSeePlayer = Bb.CheckVisibilityBehavior.IsTargetVisible(Bb.PlayerController);

            if (!Bb.PlayerDetectionBehavior.PlayerIsDetected.CurrentValue)
            {
                SwitchState<NotifyBoss>();
                return;
            }

            if (Bb.LookAtTargetBehavior.Target != null)
            {
                Bb.LastSeenPlayerPosition = Bb.LookAtTargetBehavior.Target.position;
            }
        }
    }
}
