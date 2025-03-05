using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.Boss.States
{
    public class BeNotified : State<Blackboard, Helpers>
    {
        private float timer;

        internal override void OnExit()
        {
        }

        internal override void OnEnter()
        {
            Bb.LineOfSightByPathBehavior.enabled = false;

            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LookAtTargetBehavior.enabled = true;

            Bb.TempTarget.position = Bb.NotifiedPosition;

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
