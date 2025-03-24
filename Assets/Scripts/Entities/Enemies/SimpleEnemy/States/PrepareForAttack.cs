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
            Bb.LineOfSightBehavior.enabled = false;
            Bb.PatrolBehavior.enabled = false;
            Bb.GoToBehavior.enabled = false;
            Bb.ShootPlayerBehavior.enabled = false;

            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LookAtTargetBehavior.enabled = true;
            Bb.PlayerDetectionBehavior.enabled = true;

            timer = 0f;

            // Player is already detected so the detection rate must be maxed 
            Bb.PlayerDetectionBehavior.DistanceToPlayerNormalized = 0f;

            AudioManager.Play(AudioManager.EnemiesNotificationsPrefab, AudioManager.EnemyAlertedClip,
                Bb.transform.position);
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
                return;
            }

            if ((timer += deltaTime) >= Bb.AttackDelay)
            {
                SwitchState<Attack>();
            }
        }
    }
}
