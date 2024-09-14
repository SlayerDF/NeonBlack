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

        private void Start()
        {
            lineOfSightByPatternBehavior.PointsToWatch = path.PathPoints;
            lineOfSightByPatternBehavior.StartPlayerWatching(true);
        }
    }
}