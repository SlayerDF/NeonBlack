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

        private Vector3? currentTargetPosition;

        public Vector3? CurrentTargetPosition => currentTargetPosition;

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
            currentTargetPosition = null;
        }

        private async UniTask WalkThroughPointsAsync(Waypoint startPoint, CancellationToken cancellationToken)
        {
            var currentWayPoint = startPoint;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var nextPoint = Path.NextWaypoint(currentWayPoint);

                await WalkToNextPoint(currentWayPoint.Position, nextPoint.Position, 0.01f, cancellationToken);
                await UniTask.WaitForSeconds(changePointTimeSec, cancellationToken: cancellationToken).SuppressCancellationThrow();

                currentWayPoint = nextPoint;
            }
        }

        private async UniTask WalkToNextPoint(Vector3 startPoint, Vector3 endPoint, float speed, CancellationToken cancellationToken)
        {
            var batchesCount = Mathf.RoundToInt(1 / speed);
            for (int i = 0; i < batchesCount; i++)
            {
                currentTargetPosition = Vector3.Lerp(startPoint, endPoint, i * speed);
                targetPointVisuals.transform.position = currentTargetPosition.Value;
                await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken).SuppressCancellationThrow();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private void Awake()
        {
            radiusSqr = sphereRadius * sphereRadius;
        }

        private void FixedUpdate()
        {
            if (!isActive || !currentTargetPosition.HasValue)
            {
                return;
            }

            var d =
                Mathf.Pow(player.transform.position.x - currentTargetPosition.Value.x, 2) +
                Mathf.Pow(player.transform.position.y - currentTargetPosition.Value.y, 2) +
                Mathf.Pow(player.transform.position.z - currentTargetPosition.Value.z, 2);

            IsPlayerDetected = d < radiusSqr;
        }

        private void OnDrawGizmos()
        {
            if (!currentTargetPosition.HasValue)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTargetPosition.Value, sphereRadius);
        }

        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }
    }
}