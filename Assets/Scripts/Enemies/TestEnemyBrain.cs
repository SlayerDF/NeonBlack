using UnityEngine;

public class TestEnemyBrain : MonoBehaviour
{
    #region Serialized Fields

    [Header("Behaviors")]
    [SerializeField]
    private FollowPlayerBehavior followPlayerBehavior;

    [SerializeField]
    private LineOfSightBehavior lineOfSightBehavior;

    [SerializeField]
    private PatrolBehavior patrolBehavior;

    [SerializeField]
    private PlayerDetectionBehavior playerDetectionBehavior;

    [Header("Properties")]
    [SerializeField]
    private float thinkFrequency = 0.1f;

    #endregion

    private float thinkTimer;

    #region Event Functions

    private void Awake()
    {
        followPlayerBehavior.enabled = false;
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
            followPlayerBehavior.enabled = true;
            patrolBehavior.enabled = false;
        }
        else
        {
            followPlayerBehavior.enabled = false;
            patrolBehavior.enabled = true;
        }
    }

    #endregion
}
