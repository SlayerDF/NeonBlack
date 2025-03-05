using NeonBlack.Systems.LevelState;
using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.Boss.States
{
    public class FollowPlayer : State<Blackboard, Helpers>
    {
        internal override void OnExit()
        {
        }

        internal override void OnEnter()
        {
            Bb.LineOfSightByPathBehavior.enabled = false;

            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LookAtTargetBehavior.enabled = true;

            H.UpdateTarget(Bb.PlayerController.transform);
            H.Focus();
        }

        internal override void OnUpdate(float deltaTime)
        {
            if (!Bb.CheckVisibilityBehavior.IsTargetVisible(Bb.PlayerController))
            {
                SwitchState<LoseSightOfPlayer>();
                return;
            }

            LevelState.UpdateAlert(Bb.AlertAccumulation * deltaTime);
        }
    }
}
