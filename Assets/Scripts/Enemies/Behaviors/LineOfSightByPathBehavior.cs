using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Enemies.Behaviors
{
    public class LineOfSightByPathBehavior : MonoBehaviour
    {
        [SerializeField]
        private float changePointTimeSec = 3.0f;

        [SerializeField]
        private float sphereRadius = 1.0f;

        [SerializeField]
        private PlayerController player;

        /// <summary>
        /// List of points to watch. Every point is a center of a sphere.
        /// </summary>
        public LinkedList<Vector3> PointsToWatch { get; set; }

        /// <summary>
        /// True if player is detected in the current moment.
        /// </summary>
        public bool IsPlayerDetected { get; private set; }

        /// <summary>
        /// Event is raised when all points on the path are passed (will not be raised if the path is looped).
        /// </summary>
        public event EventHandler WalkthroughCompleted;

        private float radiusSqr;

        private bool isActive;

        private LinkedListNode<Vector3> currentPoint;

        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Start player watching process. Will go through all the points and check if player is detected.
        /// </summary>
        /// <param name="loop">True to not stop after reaching the last point on the path.</param>
        public void StartPlayerWatching(bool loop)
        {
            if (isActive || PointsToWatch?.Count == 0)
            {
                return;
            }

            isActive = true;

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            WalkThroughPointsAsync(PointsToWatch.First, loop, cancellationTokenSource.Token).Forget();
        }

        public void StopPlayerWatching()
        {
            isActive = false;
            cancellationTokenSource?.Cancel();
            currentPoint = null;
        }

        private async UniTask WalkThroughPointsAsync(LinkedListNode<Vector3> startPoint, bool loop, CancellationToken cancellationToken)
        {
            currentPoint = startPoint;
            while (currentPoint != null)
            {
                await UniTask.WaitForSeconds(changePointTimeSec, cancellationToken: cancellationToken).SuppressCancellationThrow();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                currentPoint = currentPoint.Next;

                if (currentPoint == null && loop)
                {
                    currentPoint = PointsToWatch.First;
                }
                else
                {
                    WalkthroughCompleted?.Invoke(this, EventArgs.Empty);
                }
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
                Mathf.Pow(player.transform.position.x - currentPoint.Value.x, 2) +
                Mathf.Pow(player.transform.position.y - currentPoint.Value.y, 2) +
                Mathf.Pow(player.transform.position.z - currentPoint.Value.z, 2);

            if (d < radiusSqr)
            {
                IsPlayerDetected = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (currentPoint == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentPoint.Value, sphereRadius);
        }

        private void OnDestroy()
        {
            cancellationTokenSource?.Dispose();
        }
    }
}