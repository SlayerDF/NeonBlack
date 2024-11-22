using System;
using NeonBlack.Entities.Enemies;
using NeonBlack.Interfaces;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeonBlack.Entities.Player
{
    public partial class PlayerInput
    {
        #region Serialized Fields

        [Header("Attack")]
        [SerializeField]
        private float attackCooldown = 0.5f;

        [SerializeField]
        private float shootCooldown = 2f;

        [SerializeField]
        private float attackFrame = 0.5f;

        [SerializeField]
        private SubscribableCollider attackCollider;

        [SerializeField]
        private Transform shootProjectileOrigin;

        #endregion


        private float attackTimer;
        private bool isAiming;

        private bool isAttacking;
        private float shootTimer;

        public static event Action<bool> AimStateChanged;

        private void OnEnableAttack()
        {
            attackCollider.TriggerEnter += OnAttackTriggerEnter;
        }

        private void OnDisableAttack()
        {
            attackCollider.TriggerEnter -= OnAttackTriggerEnter;
        }

        private void OnAttackStarted(InputAction.CallbackContext context)
        {
            isAttacking = true;
        }

        private void OnAttackCanceled(InputAction.CallbackContext context)
        {
            isAttacking = false;
        }

        private void OnAimStarted(InputAction.CallbackContext context)
        {
            isAiming = true;
            AimStateChanged?.Invoke(isAiming);
        }

        private void OnAimCancelled(InputAction.CallbackContext context)
        {
            isAiming = false;
            AimStateChanged?.Invoke(isAiming);
        }

        private void OnWeaponChange(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>() > 0)
            {
                inventory.NextWeapon();
            }
            else
            {
                inventory.PrevWeapon();
            }
        }

        private void OnShoot(InputAction.CallbackContext context)
        {
            if (!isAiming || shootTimer > 0f)
            {
                return;
            }

            var weapon = inventory.CurrentWeapon;
            if (weapon == null || !inventory.CurrentWeaponHasAmmo)
            {
                return;
            }

            weapon.Shoot(shootProjectileOrigin.position,
                Quaternion.Euler(cameraOrbit.x, cameraOrbit.y, 0) * Vector3.forward);

            inventory.DecreaseWeaponAmmo(weapon);

            shootTimer = shootCooldown;
        }

        private void UpdateShoot()
        {
            if (shootTimer > 0f)
            {
                shootTimer -= Time.deltaTime;
            }
        }

        private static void OnAttackTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out EnemyHealth enemyHealth))
            {
                return;
            }

            if (!enemyHealth.Dead)
            {
                AudioManager.Play(AudioManager.HitsPrefab, AudioManager.PlayerHitResultClip, other.transform.position);
            }

            enemyHealth.TakeDamage(DamageSource.Normal, 1f);
        }

        private void UpdateAttack()
        {
            if (attackTimer > attackFrame)
            {
                attackCollider.enabled = false;
            }

            if (attackTimer < attackCooldown)
            {
                attackTimer += Time.deltaTime;
                return;
            }

            if (!IsGrounded || !isAttacking || isAiming)
            {
                return;
            }

            attackTimer = 0f;

            playerAnimation.OnAttack();
            attackCollider.enabled = true;
        }
    }
}
