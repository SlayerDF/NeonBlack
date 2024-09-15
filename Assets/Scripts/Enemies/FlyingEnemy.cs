using Enemies.Behaviors;
using UnityEngine;

namespace Enemies
{
    public class FlyingEnemy : MonoBehaviour
    {
        [SerializeField]
        private LineOfSightByPathBehavior lineOfSightByPatternBehavior;

        [SerializeField]
        private Path path;

        [SerializeField]
        private Transform losMeshTransform;

        [SerializeField]
        private float alertValue = 0.001f;

        private void Start()
        {
            lineOfSightByPatternBehavior.Path = path;
            lineOfSightByPatternBehavior.StartPlayerWatching();
        }

        private void FixedUpdate()
        {
            if (lineOfSightByPatternBehavior.CurrentTargetPosition.HasValue)
            {
                losMeshTransform.LookAt(lineOfSightByPatternBehavior.CurrentTargetPosition.Value);
            }

            if (lineOfSightByPatternBehavior.IsPlayerDetected)
            {
                LevelState.UpdateAlert(alertValue);
            }
        }
    }
}