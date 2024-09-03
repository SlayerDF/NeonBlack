using System.Linq;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    private EnemyBehavior[] behaviors;
    private EnemyBehavior[] updateBehaviors;
    private EnemyBehavior[] fixedUpdateBehaviors;

    protected abstract EnemyBehavior[] RegisterBehaviors();

    protected virtual void Awake()
    {
        behaviors = RegisterBehaviors();
        updateBehaviors = behaviors.Where(x => x.ExecutionKind == ExecutionKind.Update).ToArray();
        fixedUpdateBehaviors = behaviors.Where(x => x.ExecutionKind == ExecutionKind.FixedUpdate).ToArray();

        foreach (var behavior in behaviors)
        {
            behavior.Awake(this);
        }
    }

    protected virtual void Start()
    {
        foreach (var behavior in behaviors)
        {
            behavior.Start();
        }
    }

    protected virtual void Update()
    {
        for (int i = 0; i < updateBehaviors.Length; i++)
        {
            fixedUpdateBehaviors[i].Update(Time.deltaTime);
        }
    }

    protected virtual void FixedUpdate()
    {
        for (int i = 0; i < fixedUpdateBehaviors.Length; i++)
        {
            fixedUpdateBehaviors[i].Update(Time.fixedDeltaTime);
        }
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (behaviors == null)
        {
            behaviors = RegisterBehaviors();

            foreach (var behavior in behaviors)
            {
                behavior.Awake(this);
            }
        }

        for (int i = 0; i < behaviors.Length; i++)
        {
            behaviors[i].OnDrawGizmos();
        }
    }
#endif
}
