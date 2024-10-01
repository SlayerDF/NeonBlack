using NeonBlack.Utilities;
using UnityEngine;
using static NeonBlack.Utilities.Path;

namespace NeonBlack.Entities.Enemies.Behaviors
{
    /// <summary>
    /// Contains the logic to smoothly transition between path points and check if a player
    /// is inside a target sphere. Where path point is a radius of the target sphere.
    /// </summary>
    public class LineOfSightByPathBehavior : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private Transform targetPoint;

        [SerializeField]
        private Transform playerTransform;

        [SerializeField]
        private Path path;

        [Header("Properties")]
        [SerializeField]
        private float changePointTimeSec = 3.0f;

        [SerializeField]
        private float walkingSpeed = 5f;

        [SerializeField]
        private float sphereRadius = 1.0f;

        #endregion

        private Waypoint currentWaypoint;

        private float radiusSqr;

        private bool waiting;
        private float waitTimer;

        /// <summary>
        /// True if player is detected in the current moment.
        /// </summary>
        public bool IsPlayerDetected { get; private set; }

        public Transform TargetPoint => targetPoint;

        #region Event Functions

        private void Awake()
        {
            radiusSqr = sphereRadius * sphereRadius;
        }

        private void Start()
        {
            currentWaypoint = path.NextWaypoint();
            targetPoint.position = currentWaypoint.Position;
        }

        private void FixedUpdate()
        {
            // Go the next path point smoothly.
            targetPoint.position = Vector3.MoveTowards(targetPoint.position, currentWaypoint.Position,
                walkingSpeed * Time.fixedDeltaTime);

            var d =
                Mathf.Pow(playerTransform.position.x - targetPoint.position.x, 2) +
                Mathf.Pow(playerTransform.position.y - targetPoint.position.y, 2) +
                Mathf.Pow(playerTransform.position.z - targetPoint.position.z, 2);

            IsPlayerDetected = d < radiusSqr;

            // Wait on the path point for some time.
            if (waiting)
            {
                if (waitTimer < changePointTimeSec)
                {
                    waitTimer += Time.fixedDeltaTime;
                    return;
                }

                waiting = false;
                waitTimer = 0.0f;

                // Go to the next point.
                currentWaypoint = path.NextWaypoint(currentWaypoint);
            }
            // Check if we arrived to the next path point.
            else if (Vector3.SqrMagnitude(targetPoint.position - currentWaypoint.Position) < 0.1f)
            {
                waiting = true;
            }
        }

        private void OnEnable()
        {
            targetPoint.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            targetPoint.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!targetPoint)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPoint.position, sphereRadius);
        }
#endif

        #endregion
    }
}
