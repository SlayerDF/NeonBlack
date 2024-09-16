using UnityEngine;

public class FlyingEnemyBrain : MonoBehaviour
{
    [SerializeField]
    private LineOfSightByPathBehavior lineOfSightByPathBehavior;

    [SerializeField]
    private Transform losMeshTransform;

    [SerializeField]
    private float alertValue = 0.001f;

    private void FixedUpdate()
    {
        losMeshTransform.LookAt(lineOfSightByPathBehavior.CurrentTargetPosition);

        if (lineOfSightByPathBehavior.IsPlayerDetected)
        {
            LevelState.UpdateAlert(alertValue * Time.fixedDeltaTime);
        }
    }
}