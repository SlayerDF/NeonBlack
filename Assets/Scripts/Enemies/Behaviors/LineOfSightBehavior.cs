using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LineOfSightBehavior : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private float angle;

    [SerializeField]
    private float radius;

    #endregion

    #region Event Functions

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = new Color(1f, 0f, 0f, 0.5f);

        var from = Quaternion.Euler(0f, -angle * 0.5f, 0f) * transform.forward;
        Handles.DrawSolidArc(transform.position, Vector3.up, from, angle, radius);
    }
#endif

    #endregion
}
