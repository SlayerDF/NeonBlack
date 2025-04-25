using Cysharp.Threading.Tasks;
using NeonBlack.Particles;
using NeonBlack.Systems.StateMachine;
using NeonBlack.Utilities;

namespace NeonBlack.Entities.Enemies.Boss.States
{
    public class BeDead : State<Blackboard>
    {
        internal override void OnExit()
        {
            Bb.gameObject.SetActive(true);

            Bb.BossAnimation.SetIsDead(false);
        }

        internal override void OnEnter()
        {
            Bb.CheckVisibilityBehavior.enabled = false;
            Bb.LineOfSightByPathBehavior.enabled = false;
            Bb.LookAtTargetBehavior.enabled = false;

            SpawnDeathParticles();

            Bb.BossAnimation.SetIsDead(true);

            Bb.BossAnimation.WaitAnimationEnd(BossAnimation.DeathAnimation, 0).ContinueWith(() =>
            {
                Bb.gameObject.SetActive(false);
            });
        }

        internal override void OnUpdate(float deltaTime)
        {
        }

        private void SpawnDeathParticles()
        {
            SceneObjectPool.Spawn<ParticlePoolObject>(Bb.OnDeathParticles, out var ps, true);
            ps.transform.position = Bb.transform.position;
            ps.gameObject.SetActive(true);
        }
    }
}
