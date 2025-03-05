using NeonBlack.Systems.StateMachine;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Boss
{
    public class Helpers : StateMachineHelpers<Blackboard>
    {
        internal void UpdateTarget(Transform target)
        {
            Bb.LookAtTargetBehavior.Target = target;
            Bb.LeftEye.Target = target;
            Bb.RightEye.Target = target;
        }

        internal bool CanSeePlayer()
        {
            return Bb.LeftEye.CanSeePoint(Bb.PlayerController.transform.position) ||
                   Bb.RightEye.CanSeePoint(Bb.PlayerController.transform.position);
        }

        internal void Focus()
        {
            Bb.LeftEye.IsFocused = true;
            Bb.RightEye.IsFocused = true;
        }

        internal void Unfocus()
        {
            Bb.LeftEye.IsFocused = false;
            Bb.RightEye.IsFocused = false;
        }
    }
}
