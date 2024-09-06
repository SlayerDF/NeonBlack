using UnityEngine;
using UnityEngine.AI;

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
        waypoint = path.NextWaypoint();
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

    #endregion

    private bool ReachedDestination()
    {
        return !float.IsInfinity(navAgent.remainingDistance) && navAgent.remainingDistance < navAgent.stoppingDistance;
    }
}
