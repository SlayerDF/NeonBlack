using System;
using NeonBlack.Interfaces;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Boss
{
    public class BossHealth : MonoBehaviour, IEnemyHealth
    {
        #region Serialized Fields

        [Header("Properties")]
        [SerializeField]
        private float health = 1f;

        #endregion

        private float maxHealth;

        #region Event Functions

        private void Awake()
        {
            maxHealth = health;
        }

        #endregion

        #region IEnemyHealth Members

        public void TakeDamage(DamageSource source, float dmg, Transform attacker = null)
        {
            if (source != DamageSource.Missile)
            {
                return;
            }

            health -= dmg;

            HealthChanged?.Invoke(health, maxHealth);

            if (health > 0 || Dead)
            {
                return;
            }

            Dead = true;

            Death?.Invoke();
        }

        public bool Dead { get; private set; }

        #endregion

        public static event Action Death;
        public static event Action<float, float> HealthChanged;
    }
}
