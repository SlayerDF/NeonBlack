using NeonBlack.Extensions;
using NeonBlack.Systems.StateMachine;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class BeDistracted : State<Blackboard, Helpers>
    {
        private float lookAtOriginalRotationSpeed;
        private Transform lookAtOriginalTarget;
        private float timer;

        internal override void OnExit()
        {
            Bb.LookAtTargetBehavior.Target = lookAtOriginalTarget;
            Bb.LookAtTargetBehavior.RotationSpeed = lookAtOriginalRotationSpeed;
            Bb.DistractionGameObject = null;
        }

        internal override void OnEnter()
        {
            Bb.ShootPlayerBehavior.enabled = false;
            Bb.PatrolBehavior.enabled = false;
            Bb.GoToBehavior.enabled = false;

            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LookAtTargetBehavior.enabled = true;
            Bb.PlayerDetectionBehavior.enabled = true;
            Bb.LineOfSightBehavior.enabled = true;

            lookAtOriginalTarget = Bb.LookAtTargetBehavior.Target;
            lookAtOriginalRotationSpeed = Bb.LookAtTargetBehavior.RotationSpeed;
            Bb.LookAtTargetBehavior.Target = Bb.DistractionGameObject?.transform;
            Bb.LookAtTargetBehavior.RotationSpeed = Bb.DistractedRotationSpeed;

            timer = 0f;
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

            if (Bb.DistractionGameObject.IsValidAndEnabled() && (timer += deltaTime) < Bb.DistractionTime)
            {
                return;
            }

            SwitchState<Patrol>();
        }
    }
}
