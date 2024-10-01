using Systems.AudioManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInput
{
    #region Serialized Fields

    [Header("Attack")]
    [SerializeField]
    private float attackCooldown = 0.5f;

    [SerializeField]
    private float attackFrame = 0.5f;

    [SerializeField]
    private SubscribableCollider attackCollider;

    #endregion

    private float attackTimer;

    private bool isAttacking;

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

        enemyHealth.TakeDamage(1f);
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

        if (!isAttacking || !IsGrounded)
        {
            return;
        }

        attackTimer = 0f;

        playerAnimation.OnAttack();
        attackCollider.enabled = true;
    }
}
