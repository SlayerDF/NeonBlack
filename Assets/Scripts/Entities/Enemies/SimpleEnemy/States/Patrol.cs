using NeonBlack.Entities.Player;
using NeonBlack.Extensions;
using NeonBlack.Systems.StateMachine;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class Patrol : State<Blackboard>
    {
        internal override void OnExit()
        {
        }

        internal override void OnEnter()
        {
            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LineOfSightBehavior.enabled = true;
            Bb.PatrolBehavior.enabled = true;
            Bb.PlayerDetectionBehavior.enabled = true;

            Bb.LookAtTargetBehavior.enabled = false;
            Bb.ShootPlayerBehavior.enabled = false;
        }

        internal override void OnUpdate(float deltaTime)
        {
            var playerInLos = Bb.LineOfSightBehavior.FirstTarget<PlayerController>();
            var playerInLosNormalizedDistance = Bb.LineOfSightBehavior.NormalizedDistanceToTarget(playerInLos);

            Bb.PlayerController ??= playerInLos;

            if (playerInLosNormalizedDistance.HasValue && Bb.CheckVisibilityBehavior.IsTargetVisible(playerInLos))
            {
                Bb.PlayerDetectionBehavior.CanSeePlayer = true;
                Bb.PlayerDetectionBehavior.DistanceToPlayerNormalized = playerInLosNormalizedDistance.Value;
            }
            else
            {
                Bb.PlayerDetectionBehavior.CanSeePlayer = false;
            }

            var color = Color.Lerp(Bb.LowAlertColor, Bb.HighAlertColor, Bb.PlayerDetectionBehavior.DetectionLevel);
            Bb.LineOfSightVisuals.material.SetEmissionColor(color);

            if (Bb.PlayerDetectionBehavior.PlayerIsDetected.CurrentValue)
            {
                SwitchState<PrepareForAttack>();
            }
        }
    }
}
