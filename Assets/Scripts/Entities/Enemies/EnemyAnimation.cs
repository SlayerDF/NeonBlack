using JetBrains.Annotations;
using NeonBlack.Systems.AudioManagement;
using UnityEngine;

namespace NeonBlack.Entities.Enemies
{
    public class EnemyAnimation : EntityAnimation
    {
        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        private static readonly int IsNotifyingBoss = Animator.StringToHash("IsNotifyingBoss");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
        public static readonly int DeathAnimation = Animator.StringToHash("Death");
        public static readonly int WakeUpAnimation = Animator.StringToHash("WakeUp");
        public static readonly int CrouchAnimation = Animator.StringToHash("Crouch");
        public static readonly int StandAnimation = Animator.StringToHash("Stand");

        private Vector3 lastPosition;

        #region Event Functions

        private void Update()
        {
            if (Time.deltaTime <= 0)
            {
                return;
            }

            var currentPosition = transform.position;
            currentPosition.y = 0f;

            var velocity = Vector3.SqrMagnitude(currentPosition - lastPosition) / Time.deltaTime;

            lastPosition = currentPosition;

            animator.SetFloat(Velocity, velocity);
        }

        #endregion

        public void SetIsAttacking(bool value)
        {
            animator.SetBool(IsAttacking, value);
        }

        public void SetIsNotifyingBoss(bool value)
        {
            animator.SetBool(IsNotifyingBoss, value);
        }

        public void SetIsDead(bool value)
        {
            animator.SetBool(IsDead, value);
        }

        public void SetIsCrouching(bool value)
        {
            animator.SetBool(IsCrouching, value);
        }

        [UsedImplicitly]
        private void OnFootsteps()
        {
            AudioManager.Play(AudioManager.FootstepsPrefab, AudioManager.EnemyFootstepsClip, transform.position);
        }
    }
}
