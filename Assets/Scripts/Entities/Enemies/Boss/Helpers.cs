using NeonBlack.Systems.StateMachine;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Boss
{
    public class Helpers : StateMachineHelpers<Blackboard>
    {
        internal void UpdateTarget(Transform target)
        {
            Bb.LookAtTargetBehavior.Target = target;

            for (var i = 0; i < Bb.Eyes.Length; i++)
            {
                Bb.Eyes[i].Target = target;
            }
        }

        internal bool CanSeePlayer()
        {
            for (var i = 0; i < Bb.Eyes.Length; i++)
            {
                if (Bb.Eyes[i].CanSeePoint(Bb.PlayerController.transform.position))
                {
                    return true;
                }
            }

            return false;
        }

        internal void Focus()
        {
            for (var i = 0; i < Bb.Eyes.Length; i++)
            {
                Bb.Eyes[i].IsFocused = true;
            }
        }

        internal void Unfocus()
        {
            for (var i = 0; i < Bb.Eyes.Length; i++)
            {
                Bb.Eyes[i].IsFocused = false;
            }
        }
    }
}
