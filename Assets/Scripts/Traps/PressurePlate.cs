using NeonBlack.Systems.AudioManagement;
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

        [SerializeField]
        private ParticleSystem activationParticles;

        #endregion

        private readonly Collider[] colliders = new Collider[10];
        private float activationCooldownTime;

        private int collisions;
        private float pressCurrentTime;
        private float pressProgress;
        private float updateCollidersTime;

        private Vector3 visualsOriginalPosition;

        #region Event Functions

        private void Update()
        {
            UpdateColliders(Time.deltaTime);

            var prevProgress = pressProgress;
            pressProgress = AnimateVisuals(Time.deltaTime);

            if (activationCooldownTime > 0f)
            {
                activationCooldownTime -= Time.deltaTime;
            }

            if (pressProgress < 1f || activationCooldownTime > 0f)
            {
                return;
            }

            if (pressProgress >= 1f && prevProgress < 1f)
            {
                AudioManager.Play(AudioManager.InteractionsPrefab, AudioManager.PressurePlateClip, transform.position);
                activationParticles.Play();
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

            // Process includeLayers and excludeLayers
            var newCollisions = collisions;
            for (var i = 0; i < collisions; i++)
            {
                var excluded = (colliders[i].excludeLayers & (1 << gameObject.layer)) != 0;
                var included = (colliders[i].includeLayers & (1 << gameObject.layer)) != 0;

                if (excluded && !included)
                {
                    newCollisions--;
                }
            }

            collisions = newCollisions;
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
