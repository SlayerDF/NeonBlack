using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.Boss.States
{
    public class ObserveLevel : State<Blackboard, Helpers>
    {
        internal override void OnExit()
        {
        }

        internal override void OnEnter()
        {
            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LineOfSightByPathBehavior.enabled = true;
            Bb.LookAtTargetBehavior.enabled = true;

            H.UpdateTarget(Bb.LineOfSightByPathBehavior.TargetPoint);
            H.Unfocus();
        }

        internal override void OnUpdate(float deltaTime)
        {
            if (Bb.LineOfSightByPathBehavior.IsPlayerDetected &&
                Bb.CheckVisibilityBehavior.IsTargetVisible(Bb.PlayerController))
            {
                SwitchState<FollowPlayer>();
            }
        }
    }
}
