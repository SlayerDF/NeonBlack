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

        #region IEnemyHealth Members

        public void TakeDamage(DamageSource source, float dmg, Transform attacker = null)
        {
            if (source != DamageSource.Missile)
            {
                return;
            }

            health -= dmg;

            if (health > 0 || Dead)
            {
                return;
            }

            Dead = true;

            Death?.Invoke();
        }

        public bool Dead { get; private set; }

        #endregion

        public event Action Death;
    }
}
