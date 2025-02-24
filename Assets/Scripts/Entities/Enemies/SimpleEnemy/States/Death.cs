using Cysharp.Threading.Tasks;
using NeonBlack.Systems.StateMachine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy.States
{
    public class Death : State<Blackboard>
    {
        internal override void OnExit()
        {
        }

        internal override void OnEnter()
        {
            Bb.CheckVisibilityBehavior.enabled = false;
            Bb.LookAtTargetBehavior.enabled = false;
            Bb.PlayerDetectionBehavior.enabled = false;
            Bb.ShootPlayerBehavior.enabled = false;
            Bb.LineOfSightBehavior.enabled = false;
            Bb.PatrolBehavior.enabled = false;

            PlayAnimationAndKill().Forget();
        }

        internal override void OnUpdate(float deltaTime)
        {
        }

        private async UniTaskVoid PlayAnimationAndKill()
        {
            Bb.EnemyAnimation.OnDeath();

            await Bb.EnemyAnimation.WaitAnimationEnd(EnemyAnimation.Death, 0);

            Bb.EnemyHealth.Kill();
        }
    }
}
