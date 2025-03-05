using UnityEngine;

namespace NeonBlack.Entities.Enemies.Boss
{
    public class BossAnimation : EntityAnimation
    {
        private static readonly int DeathBool = Animator.StringToHash("IsDead");

        public static readonly int DeathAnimation = Animator.StringToHash("Death");

        public void SetIsDead(bool value)
        {
            animator.SetBool(DeathBool, value);
        }
    }
}
