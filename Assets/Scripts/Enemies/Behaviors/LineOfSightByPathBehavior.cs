using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using static Path;

namespace Enemies.Behaviors
{
    public class LineOfSightByPathBehavior : MonoBehaviour
    {
        [SerializeField]
        private float changePointTimeSec = 3.0f;

        [SerializeField]
        private float sphereRadius = 1.0f;

        [SerializeField]
        private GameObject targetPointVisuals;

        [SerializeField]
        private PlayerController player;

        /// <summary>
        /// Path to go through.
        /// </summary>
        public Path Path { get; set; }

        /// <summary>
        /// True if player is detected in the current moment.
        /// </summary>
        public bool IsPlayerDetected { get; private set; }

        private float radiusSqr;

        private bool isActive;

        private Waypoint? currentPoint;

        public Waypoint? CurrentPoint => currentPoint;

        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Start player watching process. Will go through all the path points and check if player is detected.
        /// </summary>
        public void StartPlayerWatching()
        {
            if (isActive || Path == null)
            {
                return;
            }

            isActive = true;

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            targetPointVisuals.SetActive(true);
            WalkThroughPointsAsync(Path.NextWaypoint(), cancellationTokenSource.Token).Forget();
        }

        public void StopPlayerWatching()
        {
            isActive = false;
            targetPointVisuals.SetActive(false);
            cancellationTokenSource?.Cancel();
            currentPoint = null;
        }

        private async UniTask WalkThroughPointsAsync(Waypoint startPoint, CancellationToken cancellationToken)
        {
            currentPoint = startPoint;
            while (true)
            {
                targetPointVisuals.transform.position = currentPoint.Value.Position;

                await UniTask.WaitForSeconds(changePointTimeSec, cancellationToken: cancellationToken).SuppressCancellationThrow();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                currentPoint = Path.NextWaypoint(currentPoint);
            }
        }

        private void Awake()
        {
            radiusSqr = sphereRadius * sphereRadius;
        }

        private void FixedUpdate()
        {
            if (!isActive || currentPoint == null)
            {
                return;
            }

            var d =
                Mathf.Pow(player.transform.position.x - currentPoint.Value.Position.x, 2) +
                Mathf.Pow(player.transform.position.y - currentPoint.Value.Position.y, 2) +
                Mathf.Pow(player.transform.position.z - currentPoint.Value.Position.z, 2);

            IsPlayerDetected = d < radiusSqr;
        }

        private void OnDrawGizmos()
        {
            if (currentPoint == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentPoint.Value.Position, sphereRadius);
        }

        private void OnDestroy()
        {
            cancellationTokenSource?.Dispose();
        }
    }
}