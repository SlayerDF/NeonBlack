﻿using System;
using JetBrains.Annotations;
using NeonBlack.Systems.AudioManagement;
using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public class PlayerAnimation : EntityAnimation
    {
        private const int AttackTypesCount = 4;

        private static readonly int XAxisMovement = Animator.StringToHash("XAxis");
        private static readonly int ZAxisMovement = Animator.StringToHash("ZAxis");
        private static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");
        private static readonly int VelocityMultiplier = Animator.StringToHash("VelocityMultiplier");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private static readonly int Falling = Animator.StringToHash("Falling");
        private static readonly int Jumping = Animator.StringToHash("Jumping");
        private static readonly int Dashing = Animator.StringToHash("Dashing");
        private static readonly int Attacking = Animator.StringToHash("Attacking");
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Dash = Animator.StringToHash("Dash");
        private static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int DeathAnimation = Animator.StringToHash("Death");

        #region Serialized Fields

        [SerializeField]
        private CharacterController characterController;

        [SerializeField]
        private float blendingLerpSpeed = 5f;

        [Header("Parameters")]
        [SerializeField]
        private float normalVelocity = 5f;

        [SerializeField]
        private float velocityLerpSpeed = 5f;

        #endregion

        private int attackIndex;

        private Vector3 lastPosition;

        private float velocityMultiplier;
        private float xAxisMovement;
        private float zAxisMovement;

        #region Event Functions

        private void Update()
        {
            if (Time.deltaTime <= 0)
            {
                return;
            }

            var currentPosition = transform.position;
            currentPosition.y = 0f;

            var offset = currentPosition - lastPosition;
            var moveDir = transform.InverseTransformDirection(offset.normalized);
            var currentVelocityMultiplier =
                Mathf.Clamp(offset.magnitude / Time.deltaTime / normalVelocity, 1f, 10f);

            lastPosition = currentPosition;

            velocityMultiplier =
                Mathf.MoveTowards(velocityMultiplier, currentVelocityMultiplier, velocityLerpSpeed * Time.deltaTime);
            xAxisMovement = Mathf.MoveTowards(xAxisMovement, moveDir.x, blendingLerpSpeed * Time.deltaTime);
            zAxisMovement = Mathf.MoveTowards(zAxisMovement, moveDir.z, blendingLerpSpeed * Time.deltaTime);

            animator.SetFloat(VelocityMultiplier, velocityMultiplier);
            animator.SetFloat(XAxisMovement, xAxisMovement);
            animator.SetFloat(ZAxisMovement, zAxisMovement);
            animator.SetBool(Falling, !characterController.isGrounded);

            if (!characterController.isGrounded)
            {
                animator.SetBool(Attacking, false);
                return;
            }

            animator.SetBool(Jumping, false);
            animator.SetBool(Dashing, false);
        }

        #endregion

        public event Action FootstepClipPlayed;

        public void SetInputMagnitude(float value)
        {
            animator.SetFloat(InputMagnitude, value);
        }

        public void OnJump()
        {
            animator.SetBool(Jumping, true);
            animator.SetTrigger(Jump);
        }

        public void OnDash()
        {
            animator.SetBool(Dashing, true);
            animator.SetTrigger(Dash);
        }

        public void OnDeath()
        {
            animator.SetBool(Dead, true);
        }

        public void OnAttack()
        {
            attackIndex = (attackIndex + 1) % AttackTypesCount;

            animator.SetTrigger(Attack);
            animator.SetInteger(AttackIndex, attackIndex);
        }

        [UsedImplicitly]
        private void OnFootsteps(AnimationEvent animationEvent)
        {
            var clipsInfo = animator.GetCurrentAnimatorClipInfo(0);

            var highestWeight = float.MinValue;
            AnimatorClipInfo highestWeightClipInfo = default;
            for (var i = 0; i < clipsInfo.Length; i++)
            {
                if (clipsInfo[i].weight < highestWeight)
                {
                    continue;
                }

                highestWeight = clipsInfo[i].weight;
                highestWeightClipInfo = clipsInfo[i];
            }

            if (animationEvent.animatorClipInfo.clip == highestWeightClipInfo.clip)
            {
                FootstepClipPlayed?.Invoke();
            }
        }

        [UsedImplicitly]
        private void OnHit()
        {
            AudioManager.Play(AudioManager.HitsPrefab, AudioManager.PlayerHitClip, transform.position);
        }
    }
}
