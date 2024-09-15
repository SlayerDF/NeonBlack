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
        private float walkingSpeed = 0.01f;

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

        /// <summary>
        /// True if the behavior is active.
        /// </summary>
        public bool IsActive => isActive;

        private Vector3 currentTargetPosition;

        public Vector3 CurrentTargetPosition => currentTargetPosition;

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

                await WalkToNextPoint(currentWayPoint.Position, nextPoint.Position, walkingSpeed, cancellationToken);
                await UniTask.WaitForSeconds(changePointTimeSec, cancellationToken: cancellationToken).SuppressCancellationThrow();

                currentWayPoint = nextPoint;
            }
        }

        private async UniTask WalkToNextPoint(Vector3 startPoint, Vector3 endPoint, float speedMultiplier, CancellationToken cancellationToken)
        {
            var speed = speedMultiplier * Time.fixedDeltaTime;
            var batchesCount = Mathf.RoundToInt(1 / speed);
            for (int i = 0; i < batchesCount; i++)
            {
                currentTargetPosition = Vector3.Lerp(startPoint, endPoint, i * speed);
                targetPointVisuals.transform.position = currentTargetPosition;
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
            if (!isActive)
            {
                return;
            }

            var d =
                Mathf.Pow(player.transform.position.x - currentTargetPosition.x, 2) +
                Mathf.Pow(player.transform.position.y - currentTargetPosition.y, 2) +
                Mathf.Pow(player.transform.position.z - currentTargetPosition.z, 2);

            IsPlayerDetected = d < radiusSqr;
        }

        private void OnDrawGizmos()
        {
            if (!isActive)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTargetPosition, sphereRadius);
        }

        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }
    }
}