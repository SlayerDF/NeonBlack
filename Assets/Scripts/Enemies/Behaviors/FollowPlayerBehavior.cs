using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class FollowPlayerBehavior : EnemyBehavior
{
    [SerializeField]
    private NavMeshAgent navAgent;

    [field: SerializeField]
    public Transform PlayerTransform { get; set; }

    public override ExecutionKind ExecutionKind => ExecutionKind.FixedUpdate;

    public override void Update(float deltaTime)
    {
        if (!PlayerTransform) return;

        navAgent.SetDestination(PlayerTransform.position);

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

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Handles.color = Color.black;
        Handles.Label(Transform.position.With(y: Transform.position.y + 3f), navAgent.remainingDistance.ToString());
        Handles.Label(Transform.position.With(y: Transform.position.y + 2.6f), navAgent.stoppingDistance.ToString());
    }
#endif
}
