using System;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Systems.LevelState;
using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class WakeUpAlly : State<Blackboard, Helpers>
    {
        private const float LookDuration = 2f;
        private const float WaitDuration = 2f;
        private const float WakeUpDistance = 2.5f;
        private const float WakeUpAlertIncrease = 0.2f;

        private Stage? lastStage;
        private Stage stage;
        private float timer;

        private bool FirstStageCall => stage != lastStage;

        internal override void OnExit()
        {
            if (Bb.NavAgent.isOnNavMesh)
            {
                Bb.NavAgent.isStopped = true;
            }
        }

        internal override void OnEnter()
        {
            Bb.LookAtTargetBehavior.enabled = false;
            Bb.ShootPlayerBehavior.enabled = false;
            Bb.PatrolBehavior.enabled = false;
            Bb.GoToBehavior.enabled = false;

            Bb.CheckVisibilityBehavior.enabled = true;
            Bb.LineOfSightBehavior.enabled = true;
            Bb.PlayerDetectionBehavior.enabled = true;

            stage = Stage.Look;
        }

        internal override void OnUpdate(float deltaTime)
        {
            if (!Bb.wakeUpAllyTarget || !Bb.wakeUpAllyTarget.CouldBeResurrected)
            {
                stage = Stage.Complete;
            }

            Stage? nextStage = null;
            switch (stage)
            {
                case Stage.Look:
                    if (FirstStageCall)
                    {
                        timer = 0f;
                        Bb.LookAtTargetBehavior.enabled = true;
                        Bb.LookAtTargetBehavior.Target = Bb.wakeUpAllyTarget.transform;
                    }

                    if (H.DetectPlayer())
                    {
                        SwitchState<PrepareForAttack>();
                        return;
                    }

                    if ((timer += deltaTime) > LookDuration)
                    {
                        Bb.LookAtTargetBehavior.enabled = false;
                        nextStage = Stage.ComeCloser;
                    }

                    break;
                case Stage.ComeCloser:
                    if (FirstStageCall)
                    {
                        Bb.GoToBehavior.enabled = true;

                        if (Bb.NavAgent.isOnNavMesh)
                        {
                            Bb.NavAgent.isStopped = false;
                        }

                        Bb.GoToBehavior.SetDestination(Bb.wakeUpAllyTarget.transform.position);
                    }

                    if (H.DetectPlayer())
                    {
                        SwitchState<PrepareForAttack>();
                        return;
                    }

                    if (Bb.GoToBehavior.ReachedDestination(WakeUpDistance))
                    {
                        Bb.GoToBehavior.enabled = false;

                        if (Bb.NavAgent.isOnNavMesh)
                        {
                            Bb.NavAgent.isStopped = true;
                        }

                        nextStage = Stage.WakeUpCrouch;
                    }

                    break;
                case Stage.WakeUpCrouch:
                    if (FirstStageCall)
                    {
                        Bb.CheckVisibilityBehavior.enabled = false;
                        Bb.LineOfSightBehavior.enabled = false;
                        Bb.PlayerDetectionBehavior.enabled = false;

                        Bb.SimpleEnemyAnimation.SetIsCrouching(true);
                    }

                    var animationStarted =
                        Bb.SimpleEnemyAnimation.AnimationStarted(SimpleEnemyAnimation.CrouchAnimation, 0);

                    if (animationStarted is null)
                    {
                        nextStage = Stage.Complete;
                        break;
                    }

                    if (animationStarted is false)
                    {
                        break;
                    }

                    switch (Bb.SimpleEnemyAnimation.AnimationEnded(SimpleEnemyAnimation.CrouchAnimation, 0))
                    {
                        case true:
                            if (Bb.wakeUpAllyTarget.Resurrect())
                            {
                                LevelState.UpdateAlert(WakeUpAlertIncrease, 0.99f);
                                AudioManager.Play(AudioManager.EnemiesNotificationsPrefab,
                                    AudioManager.SimpleEnemyWakeUpClip, Bb.wakeUpAllyTarget.transform.position);
                            }

                            nextStage = Stage.WakeUpStand;
                            break;
                        case null:
                            nextStage = Stage.Complete;
                            break;
                    }

                    break;
                case Stage.WakeUpStand:
                    if (FirstStageCall)
                    {
                        Bb.SimpleEnemyAnimation.SetIsCrouching(false);
                    }

                    animationStarted = Bb.SimpleEnemyAnimation.AnimationStarted(SimpleEnemyAnimation.StandAnimation, 0);

                    if (animationStarted is null)
                    {
                        nextStage = Stage.Complete;
                        break;
                    }

                    if (animationStarted is false)
                    {
                        break;
                    }

                    nextStage = Bb.SimpleEnemyAnimation.AnimationEnded(SimpleEnemyAnimation.StandAnimation, 0) switch
                    {
                        true => Stage.Wait,
                        null => Stage.Complete,
                        _ => nextStage
                    };

                    break;
                case Stage.Wait:
                    if (FirstStageCall)
                    {
                        timer = 0f;
                        Bb.CheckVisibilityBehavior.enabled = true;
                        Bb.LineOfSightBehavior.enabled = true;
                        Bb.PlayerDetectionBehavior.enabled = true;
                    }

                    if (H.DetectPlayer())
                    {
                        SwitchState<PrepareForAttack>();
                        return;
                    }

                    if ((timer += deltaTime) >= WaitDuration)
                    {
                        nextStage = Stage.Complete;
                    }

                    break;
                case Stage.Complete:
                    Bb.SimpleEnemyAnimation.SetIsCrouching(false);
                    SwitchState<Patrol>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            lastStage = stage;
            stage = nextStage ?? stage;
        }

        private enum Stage
        {
            Look,
            ComeCloser,
            WakeUpCrouch,
            WakeUpStand,
            Wait,
            Complete
        }
    }
}
