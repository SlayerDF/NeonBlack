using UnityEngine;

public class BossBrain : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private LineOfSightByPathBehavior lineOfSightByPathBehavior;

    [SerializeField]
    private BossEye leftEye;

    [SerializeField]
    private BossEye rightEye;

    [SerializeField]
    private Transform playerTransform;

    #endregion

    #region Event Functions

    private void FixedUpdate()
    {
        // TODO: add events to the behavior to optimize it
        if (lineOfSightByPathBehavior.IsPlayerDetected)
        {
            transform.forward = (playerTransform.position - transform.position).normalized.With(y: 0);

            leftEye.SetTargetPosition(playerTransform.position, true);
            rightEye.SetTargetPosition(playerTransform.position, true);
        }
        else
        {
            transform.forward = (lineOfSightByPathBehavior.CurrentTargetPosition - transform.position).normalized.With(y: 0);

            leftEye.SetTargetPosition(lineOfSightByPathBehavior.CurrentTargetPosition);
            rightEye.SetTargetPosition(lineOfSightByPathBehavior.CurrentTargetPosition);
        }
    }

    #endregion
}
