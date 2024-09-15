using UnityEngine;

public class FlyingEnemyBrain : MonoBehaviour
{
    [SerializeField]
    private LineOfSightByPathBehavior lineOfSightByPatternBehavior;

    [SerializeField]
    private Transform losMeshTransform;

    [SerializeField]
    private float alertValue = 0.001f;

    private void FixedUpdate()
    {
        losMeshTransform.LookAt(lineOfSightByPatternBehavior.CurrentTargetPosition);

        if (lineOfSightByPatternBehavior.IsPlayerDetected)
        {
            LevelState.UpdateAlert(alertValue * Time.fixedDeltaTime);
        }
    }
}