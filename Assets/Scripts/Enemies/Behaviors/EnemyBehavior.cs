using System;
using UnityEngine;

public enum ExecutionKind
{
    Update, FixedUpdate
}

public abstract class EnemyBehavior
{
    public abstract ExecutionKind ExecutionKind { get; }

    protected Transform Transform { get; private set; }

    public virtual void Awake(MonoBehaviour owner)
    {
        Transform = owner.transform;
    }

    public virtual void Start() {}

    public virtual void Update(float deltaTime) {}

#if UNITY_EDITOR
    public virtual void OnDrawGizmos() {}
#endif
}
