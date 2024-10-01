using System;
using NeonBlack.Interfaces;
using UnityEngine;

namespace NeonBlack.Entities.Enemies
{
    public class EnemyHealth : MonoBehaviour, IEntityHealth
    {
        #region Serialized Fields

        [SerializeField]
        private float health = 1f;

        #endregion

        public bool Dead { get; private set; }

        public bool Invincible { get; set; }

        #region IEntityHealth Members

        public void TakeDamage(float damage)
        {
            if (Invincible)
            {
                return;
            }

            health -= damage;

            if (health > 0 || Dead)
            {
                return;
            }

            Dead = true;

            Death?.Invoke();
        }

        #endregion

        public void Kill()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        public event Action Death;
    }
}
