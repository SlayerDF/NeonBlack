using UnityEngine;

public class FlyingEnemyBrain : MonoBehaviour
{
    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private BossBrain bossBrain;

    [Header("Behaviors")]
    [SerializeField]
    private LineOfSightByPathBehavior lineOfSightByPathBehavior;

    [SerializeField]
    private CheckPlayerVisibilityBehavior checkPlayerVisibilityBehavior;

    #endregion

    #region Event Functions

    private void FixedUpdate()
    {
        if (lineOfSightByPathBehavior.IsPlayerDetected && checkPlayerVisibilityBehavior.IsPlayerVisible())
        {
            NotifyBoss();
        }
    }

    #endregion

    private void NotifyBoss()
    {
        bossBrain.Notify(lineOfSightByPathBehavior.TargetPoint.position);
    }
}
