using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class Patrol : State<Blackboard, Helpers>
    {
        internal override void OnExit()
        {
            if (Bb.NavAgent.isOnNavMesh)
            {
                Bb.NavAgent.isStopped = true;
            }
        }

        internal override void OnEnter()
        {
            if (Bb.NavAgent.isOnNavMesh)
            {
                Bb.NavAgent.isStopped = false;
            }

            Bb.LookAtTargetBehavior.enabled = false;
            Bb.ShootPlayerBehavior.enabled = false;
            Bb.GoToBehavior.enabled = false;

            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LineOfSightBehavior.enabled = true;
            Bb.PatrolBehavior.enabled = true;
            Bb.PlayerDetectionBehavior.enabled = true;
        }

        internal override void OnUpdate(float deltaTime)
        {
            if (H.DetectPlayer())
            {
                SwitchState<PrepareForAttack>();
                return;
            }

            if (H.DetectAllyBody(out var body))
            {
                Bb.wakeUpAllyTarget = body;
                SwitchState<WakeUpAlly>();
            }
        }
    }
}
