using System;
using NeonBlack.Interfaces;
using UnityEditor;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.SimpleEnemy
{
    public class SimpleEnemyHealth : MonoBehaviour, IEnemyHealth
    {
        #region Serialized Fields

        [Header("Properties")]
        [SerializeField]
        private float health = 1f;

        [SerializeField]
        private bool couldBeResurrected = true;

        [Header("Vulnerability Arc")]
        [SerializeField]
        private bool vulnerabilityArcEnabled;

        [SerializeField]
        [Tooltip("The center of the arc, measured by rotating clockwise from the forward vector in degrees")]
        private float vulnerabilityArcCenter = 180f;

        [SerializeField]
        private float vulnerabilityArcAngleSize = 180f;

        [SerializeField]
        private Transform vulnerabilityArcDebugTransform;

        [Header("Visuals")]
        [SerializeField]
        private ParticleSystem bloodParticles;

        #endregion

        private float healthDefault;
        private bool killed;

        public bool CouldBeResurrected => couldBeResurrected && killed;

        public bool Invincible { get; set; }

        #region Event Functions

        private void Awake()
        {
            healthDefault = health;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!vulnerabilityArcEnabled)
            {
                return;
            }

            var from = Quaternion.Euler(0, vulnerabilityArcCenter - vulnerabilityArcAngleSize * 0.5f, 0) *
                       transform.forward;

            Handles.color = Color.magenta;

            if (vulnerabilityArcDebugTransform && CheckAttackAngle(vulnerabilityArcDebugTransform))
            {
                Handles.color = Color.red;
            }

            Handles.DrawSolidArc(transform.position, Vector3.up, from, vulnerabilityArcAngleSize, 1f);
        }
#endif

        #endregion

        #region IEnemyHealth Members

        public bool Dead { get; private set; }

        public void TakeDamage(DamageSource source, float damage, Transform attacker = null)
        {
            if (Invincible)
            {
                return;
            }

            if (source == DamageSource.Normal && !CheckAttackAngle(attacker))
            {
                return;
            }

            health -= damage;

            if (health > 0 || Dead)
            {
                return;
            }

            Dead = true;

            if (bloodParticles)
            {
                bloodParticles.Play();
            }

            Death?.Invoke();
        }

        #endregion

        public void Kill(bool forceDestroy = false)
        {
            if (gameObject != null && (!couldBeResurrected || forceDestroy))
            {
                Destroy(gameObject);
            }

            killed = true;
        }

        public bool Resurrect()
        {
            if (!couldBeResurrected)
            {
                return false;
            }

            health = healthDefault;
            Dead = false;
            killed = false;
            return true;
        }

        private bool CheckAttackAngle(Transform attacker)
        {
            if (!vulnerabilityArcEnabled || attacker == null)
            {
                return true;
            }

            var arcCenter = Quaternion.Euler(0, vulnerabilityArcCenter, 0) * transform.forward;

            var angle = Vector2.Angle(
                new Vector2(arcCenter.x, arcCenter.z),
                new Vector2(attacker.position.x - transform.position.x, attacker.position.z - transform.position.z));

            return angle * 2 <= vulnerabilityArcAngleSize;
        }

        public event Action Death;
    }
}
