using UnityEngine;
using UnityEngine.AI;

namespace NeonBlack.Entities.Enemies.Behaviors
{
    public class GoToBehavior : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Vector3 destination;

        [SerializeField]
        private NavMeshAgent navAgent;

        #endregion

        #region Event Functions

        private void OnEnable()
        {
            navAgent.SetDestination(destination);
        }

        #endregion

        public void SetDestination(Vector3 dest)
        {
            destination = dest;
            navAgent.SetDestination(destination);
        }

        public bool ReachedDestination(float? customStoppingDistance = null)
        {
            return navAgent.enabled && !navAgent.pathPending &&
                   navAgent.remainingDistance < (customStoppingDistance ?? navAgent.stoppingDistance);
        }
    }
}
