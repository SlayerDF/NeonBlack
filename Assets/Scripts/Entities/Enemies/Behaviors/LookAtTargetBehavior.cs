using JetBrains.Annotations;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Behaviors
{
    public class LookAtTargetBehavior : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        [CanBeNull]
        private Transform target;

        [SerializeField]
        private float rotationSpeed = 5f;

        [SerializeField]
        private bool lockPitch = true;

        #endregion

        [CanBeNull]
        public Transform Target
        {
            get => target;
            set => target = value;
        }

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = value;
        }

        #region Event Functions

        private void Update()
        {
            if (!target)
            {
                return;
            }

            var direction = LookDirection(target);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction),
                rotationSpeed * Time.deltaTime);
        }

        #endregion

        private Vector3 LookDirection(Transform lookTarget)
        {
            var direction = (lookTarget.position - transform.position).normalized;

            if (lockPitch)
            {
                direction.y = transform.forward.y;
            }

            return direction;
        }
    }
}
