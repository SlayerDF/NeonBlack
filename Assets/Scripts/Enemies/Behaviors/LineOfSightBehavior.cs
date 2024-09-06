using UnityEditor;
using UnityEngine;

public class LineOfSightBehavior : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private float angle;

    [SerializeField]
    private float radius;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private float detectionFrequency = 0.1f;

    #endregion

    private float angleCentralized;
    private float detectionTimer;

    private float radiusSqr;

    public bool HasTarget { get; private set; }

    #region Event Functions

    private void Awake()
    {
        radiusSqr = radius * radius;
        angleCentralized = angle * 0.5f;
    }

    private void FixedUpdate()
    {
        detectionTimer += Time.fixedDeltaTime;

        if (detectionTimer > detectionFrequency)
        {
            detectionTimer = 0f;
            DetectPlayer();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = HasTarget ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        var from = Quaternion.Euler(0f, -angleCentralized, 0f) * transform.forward;
        Handles.DrawSolidArc(transform.position, Vector3.up, from, angle, radius);
    }
#endif

    #endregion

    private void DetectPlayer()
    {
        HasTarget = false;

        var vectorToPlayer = playerTransform.position - transform.position;
        if (Vector3.SqrMagnitude(vectorToPlayer) > radiusSqr)
        {
            return;
        }

        var angleToPlayer = Vector3.Angle(transform.forward, Vector3.ProjectOnPlane(vectorToPlayer, Vector3.up));
        if (angleToPlayer > angleCentralized)
        {
            return;
        }

        HasTarget = true;
    }
}
