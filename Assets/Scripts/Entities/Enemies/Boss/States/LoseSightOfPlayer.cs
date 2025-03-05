using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.Boss.States
{
    public class LoseSightOfPlayer : State<Blackboard, Helpers>
    {
        private float timer;

        internal override void OnExit()
        {
        }

        internal override void OnEnter()
        {
            Bb.LineOfSightByPathBehavior.enabled = false;
            Bb.LookAtTargetBehavior.enabled = false;

            Bb.CheckVisibilityBehavior.enabled = true;

            Bb.TempTarget.position = Bb.PlayerController.transform.position;

            H.UpdateTarget(Bb.TempTarget);
            H.Focus();

            timer = 0f;
        }

        internal override void OnUpdate(float deltaTime)
        {
            if (H.CanSeePlayer() && Bb.CheckVisibilityBehavior.IsTargetVisible(Bb.PlayerController))
            {
                SwitchState<FollowPlayer>();
                return;
            }

            if ((timer += deltaTime) < Bb.WaitTime)
            {
                return;
            }

            SwitchState<ObserveLevel>();
        }
    }
}
