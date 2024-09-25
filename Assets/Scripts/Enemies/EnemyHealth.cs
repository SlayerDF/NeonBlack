using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private float health = 1f;

    #endregion

    public bool Dead { get; private set; }

    public bool Invincible { get; set; }

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

    public void Kill()
    {
        Destroy(gameObject);
    }

    public event Action Death;
}
