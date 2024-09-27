using System.ComponentModel;
using UnityEngine;

public class TrapProjectile : Projectile
{
    #region Serialized Fields

    [SerializeField]
    private Vector2 initialVelocity = new(10f, 0f);

    [SerializeField]
    private Vector2 maxVelocity = new(10f, 9.8f);

    [SerializeField]
    private Vector2 deceleration = new(2f, 9.8f);

    [SerializeField]
    private float damage = 1f;

    #endregion

    private Vector2 velocity;

    #region Event Functions

    private void Update()
    {
        velocity -= deceleration * Time.deltaTime;
        velocity.x = Mathf.Clamp(velocity.x, 0, maxVelocity.x);
        velocity.y = Mathf.Clamp(velocity.y, -maxVelocity.y, maxVelocity.y);

        transform.position +=
            transform.forward * (velocity.x * Time.deltaTime) + transform.up * (velocity.y * Time.deltaTime);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        velocity = initialVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        var layer = (Layer)other.gameObject.layer;

        switch (layer)
        {
            case Layer.Terrain:
                ObjectPoolManager.Despawn(this);
                break;
            case Layer.Enemies:
            case Layer.Player:
                if (other.TryGetComponent(out IEntityHealth entityHealth))
                {
                    entityHealth.TakeDamage(damage);
                }

                break;
            default:
                throw new InvalidEnumArgumentException(nameof(layer), (int)layer, typeof(Layer));
        }
    }

    #endregion
}
