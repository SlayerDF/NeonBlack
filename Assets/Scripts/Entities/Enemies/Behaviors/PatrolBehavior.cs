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

        private Path.Waypoint waypoint;

        #region Event Functions

        private void Awake()
        {
            waypoint = path.InitialWaypoint(transform.position);
        }

        private void Start()
        {
            navAgent.SetDestination(waypoint.Position);
        }

        private void FixedUpdate()
        {
            if (!ReachedDestination())
            {
                return;
            }

            waypoint = path.NextWaypoint(waypoint);
            navAgent.SetDestination(waypoint.Position);
        }

        private void OnEnable()
        {
            if (navAgent.isOnNavMesh)
            {
                navAgent.isStopped = false;
            }
        }

        private void OnDisable()
        {
            if (navAgent.isOnNavMesh)
            {
                navAgent.isStopped = true;
            }
        }

        #endregion

        private bool ReachedDestination()
        {
            return navAgent.enabled && !navAgent.pathPending && navAgent.remainingDistance < navAgent.stoppingDistance;
        }
    }
}
