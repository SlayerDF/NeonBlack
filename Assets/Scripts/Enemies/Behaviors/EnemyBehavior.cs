using UnityEngine;

public abstract class EnemyBehavior
{
    protected Transform transform;

    public virtual void Awake(MonoBehaviour owner)
    {
        transform = owner.transform;
    }

    public virtual void Start() {}

    public virtual void Update(float deltaTime) {}

    public virtual void OnDrawGizmos() {}
}
