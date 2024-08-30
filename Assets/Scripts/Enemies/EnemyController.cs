using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent navAgent;

    [SerializeField]
    private Transform destination;

    private void FixedUpdate()
    {
        navAgent.SetDestination(destination.position);

        if (ReachedDestination())
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
        }
        else
        {
            navAgent.isStopped = false;
        }
    }

    private bool ReachedDestination()
    {
        return !float.IsInfinity(navAgent.remainingDistance) && navAgent.remainingDistance < navAgent.stoppingDistance;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        var percent = Mathf.Clamp01(1f - navAgent.remainingDistance / navAgent.stoppingDistance);

        Handles.color = Color.black;
        Handles.Label(transform.position.With(y: transform.position.y + 3f), navAgent.remainingDistance.ToString());
        Handles.Label(transform.position.With(y: transform.position.y + 2.6f), navAgent.stoppingDistance.ToString());
    }
}
