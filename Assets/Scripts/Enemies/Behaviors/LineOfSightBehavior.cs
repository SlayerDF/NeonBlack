using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LineOfSightBehavior : EnemyBehavior
{
    [SerializeField]
    private float angle;

    [SerializeField]
    private float radius;

    public override ExecutionKind ExecutionKind => ExecutionKind.FixedUpdate;

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        Handles.color = new(1f, 0f, 0f, 0.5f);

        var from = Quaternion.Euler(0f, -angle * 0.5f, 0f) * Transform.forward;
        Handles.DrawSolidArc(Transform.position, Vector3.up, from, angle, radius);
    }
#endif
}
