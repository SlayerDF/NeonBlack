using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TrapProjectile : Projectile
{
    #region Serialized Fields

    [SerializeField]
    private float speed = 10f;
    private float damage = 1f;

    #endregion

    #region Event Functions

    private void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var layer = (Layer)other.gameObject.layer;

        switch (layer)
        {
            case Layer.Terrain:
                PoolManager.Despawn(this);
                break;
            case Layer.Enemies when other.TryGetComponent(out EnemyHealth enemy):
                enemy.TakeDamage(damage);
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(layer), (int)layer, typeof(Layer));
        }
    }

    #endregion





}
