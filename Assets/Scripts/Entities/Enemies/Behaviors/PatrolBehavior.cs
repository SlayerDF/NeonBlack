using NeonBlack.Utilities;
using UnityEngine;
using UnityEngine.AI;

namespace NeonBlack.Entities.Enemies.Behaviors
{
    public class PatrolBehavior : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private NavMeshAgent navAgent;

        [SerializeField]
        private Path path;

        #endregion

        private Path.Waypoint? waypoint;

        #region Event Functions

        private void Awake()
        {
            if (path)
            {
                waypoint = path.InitialWaypoint(transform.position);
            }
        }

        private void Start()
        {
            if (waypoint.HasValue)
            {
                navAgent.SetDestination(waypoint.Value.Position);
            }
        }

        private void FixedUpdate()
        {
            if (!waypoint.HasValue || !ReachedDestination())
            {
                return;
            }

            waypoint = path.NextWaypoint(waypoint.Value);
            navAgent.SetDestination(waypoint.Value.Position);
        }

        #endregion

        private bool ReachedDestination()
        {
            return navAgent.enabled && !navAgent.pathPending && navAgent.remainingDistance < navAgent.stoppingDistance;
        }
    }
}
