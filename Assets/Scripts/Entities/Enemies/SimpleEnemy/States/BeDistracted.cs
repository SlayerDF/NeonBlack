using NeonBlack.Extensions;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class BeDistracted : Patrol
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
            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LookAtTargetBehavior.enabled = true;
            Bb.PlayerDetectionBehavior.enabled = true;
            Bb.LineOfSightBehavior.enabled = true;

            Bb.ShootPlayerBehavior.enabled = false;
            Bb.PatrolBehavior.enabled = false;

            lookAtOriginalTarget = Bb.LookAtTargetBehavior.Target;
            lookAtOriginalRotationSpeed = Bb.LookAtTargetBehavior.RotationSpeed;
            Bb.LookAtTargetBehavior.Target = Bb.DistractionGameObject?.transform;
            Bb.LookAtTargetBehavior.RotationSpeed = Bb.DistractedRotationSpeed;

            timer = 0f;
        }

        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (CurrentState != this)
            {
                return;
            }

            if (Bb.DistractionGameObject.IsValidAndEnabled() && (timer += deltaTime) < Bb.DistractionTime)
            {
                return;
            }

            SwitchState<Patrol>();
        }
    }
}
