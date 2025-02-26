using UnityEngine;
using UnityEngine.AI;

namespace NeonBlack.Entities.Enemies.Behaviors
{
    public class GoToBehavior : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private NavMeshAgent navAgent;

        #endregion

        private Vector3? destination;

        public void SetDestination(Vector3 dest)
        {
            destination = dest;
            navAgent.SetDestination(destination.Value);
        }

        public bool ReachedDestination(float? customStoppingDistance = null)
        {
            return navAgent.enabled && !navAgent.pathPending &&
                   navAgent.remainingDistance < (customStoppingDistance ?? navAgent.stoppingDistance);
        }
    }
}
