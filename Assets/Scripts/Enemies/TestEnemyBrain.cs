using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestEnemyBrain : MonoBehaviour
{
    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private EnemyAnimation enemyAnimation;

    [SerializeField]
    private EnemyHealth enemyHealth;

    [Header("Behaviors")]
    [SerializeField]
    private LineOfSightBehavior lineOfSightBehavior;

    [SerializeField]
    private PatrolBehavior patrolBehavior;

    [SerializeField]
    private PlayerDetectionBehavior playerDetectionBehavior;

    [SerializeField]
    private LookAtPlayerBehavior lookAtPlayerBehavior;

    [SerializeField]
    private ShootPlayerBehavior shootPlayerBehavior;

    [Header("Properties")]
    [SerializeField]
    private float thinkFrequency = 0.1f;

    [SerializeField]
    private float attackDelay = 1f;

    #endregion

    private float attackDelayTimer;

    private DateTime date;

    private float thinkTimer;

    #region Event Functions

    private void Awake()
    {
        lookAtPlayerBehavior.enabled = false;
        shootPlayerBehavior.enabled = false;
    }

    private void FixedUpdate()
    {
        if (enemyHealth.Dead)
        {
            return;
        }

        if ((thinkTimer += Time.fixedDeltaTime) < thinkFrequency)
        {
            return;
        }

        thinkTimer = 0;

        playerDetectionBehavior.CanSeePlayer = lineOfSightBehavior.HasTarget;
        playerDetectionBehavior.DistanceToPlayerNormalized = lineOfSightBehavior.DistanceToPlayerNormalized;

        if (playerDetectionBehavior.PlayerIsDetected)
        {
            OnDetectedUpdate(thinkFrequency);
        }
        else
        {
            OnUndetectedUpdate(thinkFrequency);
        }
    }

    private void OnEnable()
    {
        enemyHealth.Death += OnDeath;
    }

    private void OnDisable()
    {
        enemyHealth.Death -= OnDeath;
    }

    #endregion

    private void OnDetectedUpdate(float deltaTime)
    {
        lookAtPlayerBehavior.enabled = true;
        patrolBehavior.enabled = false;

        if (attackDelayTimer < attackDelay)
        {
            attackDelayTimer += deltaTime;
        }
        else
        {
            lookAtPlayerBehavior.enabled = false;
            shootPlayerBehavior.enabled = true;

            enemyAnimation.SetIsAttacking(true);
            enemyHealth.Invincible = true;
        }
    }

    private void OnUndetectedUpdate(float _)
    {
        enemyHealth.Invincible = false;
        attackDelayTimer = 0f;

        lookAtPlayerBehavior.enabled = false;
        shootPlayerBehavior.enabled = false;
        patrolBehavior.enabled = true;

        enemyAnimation.SetIsAttacking(false);
    }

    private void OnDeath()
    {
        lineOfSightBehavior.enabled = false;
        patrolBehavior.enabled = false;
        playerDetectionBehavior.enabled = false;
        lookAtPlayerBehavior.enabled = false;
        shootPlayerBehavior.enabled = false;


        PlayAnimationAndKill().Forget();
    }

    private async UniTaskVoid PlayAnimationAndKill()
    {
        enemyAnimation.OnDeath();

        await enemyAnimation.WaitAnimationEnd(EnemyAnimation.Death, 0);

        enemyHealth.Kill();
    }
}
