using NeonBlack.Entities.Player;
using NeonBlack.Extensions;
using NeonBlack.Systems.StateMachine;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy
{
    public class Helpers : StateMachineHelpers<Blackboard>
    {
        internal bool DetectPlayer()
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

            return Bb.PlayerDetectionBehavior.PlayerIsDetected.CurrentValue;
        }

        internal bool DetectAllyBody(out SimpleEnemyBrain brain)
        {
            brain = null;

            var allyInLos = Bb.LineOfSightBehavior.FirstTarget<SimpleEnemyBrain>();
            if (allyInLos && allyInLos.CouldBeResurrected && Bb.CheckVisibilityBehavior.IsTargetVisible(allyInLos))
            {
                brain = allyInLos;
                return true;
            }

            return false;
        }
    }
}
