using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class Attack : State<Blackboard>
    {
        internal override void OnExit()
        {
            Bb.SimpleEnemyAnimation.SetIsAttacking(false);
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

            Bb.SimpleEnemyAnimation.SetIsAttacking(true);
            Bb.SimpleEnemyHealth.Invincible = true;

            // Player is already detected so the detection rate must be maxed 
            Bb.PlayerDetectionBehavior.DistanceToPlayerNormalized = 0f;
        }

        internal override void OnUpdate(float deltaTime)
        {
            var playerIsVisible = Bb.CheckVisibilityBehavior.IsTargetVisible(Bb.PlayerController);
            Bb.PlayerDetectionBehavior.CanSeePlayer = playerIsVisible;

            if (playerIsVisible)
            {
                Bb.LastSeenPlayerPosition = Bb.PlayerController.transform.position;
            }

            if (!Bb.PlayerDetectionBehavior.PlayerIsDetected.CurrentValue)
            {
                SwitchState<NotifyBoss>();
            }
        }
    }
}
