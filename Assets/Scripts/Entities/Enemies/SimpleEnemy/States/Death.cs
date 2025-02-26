using System;
using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class Death : State<Blackboard>
    {
        private Stage? lastStage;
        private Stage stage;
        private bool FirstStageCall => stage != lastStage;

        internal override void OnExit()
        {
            Bb.EnemyCollider.enabled = true;
            Bb.NavAgent.enabled = true;

            Bb.EnemyAnimation.SetIsDead(false);
        }

        internal override void OnEnter()
        {
            Bb.EnemyCollider.enabled = false;
            Bb.NavAgent.enabled = false;

            Bb.CheckVisibilityBehavior.enabled = false;
            Bb.LookAtTargetBehavior.enabled = false;
            Bb.PlayerDetectionBehavior.enabled = false;
            Bb.ShootPlayerBehavior.enabled = false;
            Bb.LineOfSightBehavior.enabled = false;
            Bb.PatrolBehavior.enabled = false;
            Bb.GoToBehavior.enabled = false;

            stage = Stage.WaitAnimationStart;
        }

        internal override void OnUpdate(float deltaTime)
        {
            switch (stage)
            {
                case Stage.WaitAnimationStart:
                    if (FirstStageCall)
                    {
                        Bb.EnemyAnimation.SetIsDead(true);
                    }

                    switch (Bb.EnemyAnimation.AnimationStarted(EnemyAnimation.DeathAnimation, 0))
                    {
                        case true:
                            stage = Stage.WaitAnimationEnd;
                            break;
                        case null:
                            stage = Stage.Complete;
                            break;
                    }

                    break;
                case Stage.WaitAnimationEnd:
                    switch (Bb.EnemyAnimation.AnimationEnded(EnemyAnimation.DeathAnimation, 0))
                    {
                        case true:
                            Bb.EnemyHealth.Kill();
                            stage = Stage.Complete;
                            break;
                        case null:
                            stage = Stage.Complete;
                            break;
                    }

                    break;
                case Stage.Complete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum Stage
        {
            WaitAnimationStart,
            WaitAnimationEnd,
            Complete
        }
    }
}
