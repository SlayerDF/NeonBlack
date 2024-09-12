using UnityEngine;

public class TestEnemyBrain : MonoBehaviour
{
    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private EnemyAnimation enemyAnimation;

    [Header("Behaviors")]
    [SerializeField]
    private FollowPlayerBehavior followPlayerBehavior;

    [SerializeField]
    private LineOfSightBehavior lineOfSightBehavior;

    [SerializeField]
    private PatrolBehavior patrolBehavior;

    [SerializeField]
    private PlayerDetectionBehavior playerDetectionBehavior;

    [SerializeField]
    private ShootPlayerBehavior shootPlayerBehavior;

    [Header("Properties")]
    [SerializeField]
    private float thinkFrequency = 0.1f;

    #endregion

    private float thinkTimer;

    #region Event Functions

    private void Awake()
    {
        followPlayerBehavior.enabled = false;
        shootPlayerBehavior.enabled = false;
    }

    private void FixedUpdate()
    {
        if ((thinkTimer += Time.fixedDeltaTime) < thinkFrequency)
        {
            return;
        }

        thinkTimer = 0;

        playerDetectionBehavior.CanSeePlayer = lineOfSightBehavior.HasTarget;
        playerDetectionBehavior.DistanceToPlayerNormalized = lineOfSightBehavior.DistanceToPlayerNormalized;

        if (playerDetectionBehavior.PlayerIsDetected)
        {
            // followPlayerBehavior.enabled = true;
            shootPlayerBehavior.enabled = true;
            patrolBehavior.enabled = false;

            enemyAnimation.SetIsAttacking(true);
        }
        else
        {
            // followPlayerBehavior.enabled = false;
            shootPlayerBehavior.enabled = false;
            patrolBehavior.enabled = true;

            enemyAnimation.SetIsAttacking(false);
        }
    }

    #endregion
}
