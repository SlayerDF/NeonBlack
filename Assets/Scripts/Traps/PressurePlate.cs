using UnityEngine;

namespace NeonBlack.Traps
{
    public class PressurePlate : MonoBehaviour
    {
        private const float UpdateCollidersInterval = 0.5f;

        #region Serialized Fields

        [Header("Collisions")]
        [SerializeField]
        private Vector3 collisionCenter;

        [SerializeField]
        private Vector3 collisionSize;

        [SerializeField]
        private LayerMask activatedBy;

        [Header("Components")]
        [SerializeField]
        private SimpleTrap trap;

        [SerializeField]
        private Transform visuals;

        [Header("Configuration")]
        [SerializeField]
        private Vector3 visualsPressedPosition;

        [SerializeField]
        private float pressTime = 0.5f;

        #endregion

        private readonly Collider[] colliders = new Collider[1];
        private float activationCooldownTime;

        private int collisions;
        private float pressCurrentTime;
        private float updateCollidersTime;

        private Vector3 visualsOriginalPosition;

        #region Event Functions

        private void Update()
        {
            UpdateColliders(Time.deltaTime);

            var progress = AnimateVisuals(Time.deltaTime);

            if (activationCooldownTime > 0f)
            {
                activationCooldownTime -= Time.deltaTime;
            }

            if (progress < 1f || activationCooldownTime > 0f)
            {
                return;
            }

            trap.Shoot();
            activationCooldownTime = pressTime * 2f;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + Vector3.Scale(transform.localScale, collisionCenter),
                Vector3.Scale(transform.localScale, collisionSize));
        }
#endif

        #endregion

        private void UpdateColliders(float deltaTime)
        {
            if ((updateCollidersTime -= deltaTime) > 0f)
            {
                return;
            }

            updateCollidersTime = UpdateCollidersInterval;
            collisions = Physics.OverlapBoxNonAlloc(
                transform.position + Vector3.Scale(transform.localScale, collisionCenter),
                Vector3.Scale(transform.localScale, collisionSize) * 0.5f,
                colliders, transform.rotation, activatedBy);
        }

        private float AnimateVisuals(float deltaTime)
        {
            pressCurrentTime += collisions > 0 ? deltaTime : -deltaTime;
            pressCurrentTime = Mathf.Clamp(pressCurrentTime, 0f, pressTime);

            var progress = pressCurrentTime / pressTime;
            visuals.localPosition = Vector3.Lerp(visualsOriginalPosition, visualsPressedPosition, progress);

            return progress;
        }
    }
}
