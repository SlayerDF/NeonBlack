using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class NotifyBoss : State<Blackboard>
    {
        private float timer;

        internal override void OnExit()
        {
            Bb.SimpleEnemyAnimation.SetIsNotifyingBoss(false);
        }

        internal override void OnEnter()
        {
            Bb.CheckVisibilityBehavior.enabled = false;
            Bb.LookAtTargetBehavior.enabled = false;
            Bb.PlayerDetectionBehavior.enabled = false;
            Bb.ShootPlayerBehavior.enabled = false;
            Bb.LineOfSightBehavior.enabled = false;
            Bb.PatrolBehavior.enabled = false;
            Bb.GoToBehavior.enabled = false;

            timer = 0f;
            Bb.SimpleEnemyAnimation.SetIsNotifyingBoss(true);
        }

        internal override void OnUpdate(float deltaTime)
        {
            if ((timer += deltaTime) < Bb.NotifyBossDelay)
            {
                return;
            }

            Bb.BossBrain.Notify(Bb.LastSeenPlayerPosition);
            SwitchState<Patrol>();
        }
    }
}
