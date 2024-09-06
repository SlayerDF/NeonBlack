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
    private LayerMask playerLayerMask;

    [SerializeField]
    private float detectionFrequency = 0.1f;

    #endregion

    private readonly Collider[] results = new Collider[1];
    private float detectionTimer;

    public bool HasTarget { get; private set; }

    #region Event Functions

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

        var from = Quaternion.Euler(0f, -angle * 0.5f, 0f) * transform.forward;
        Handles.DrawSolidArc(transform.position, Vector3.up, from, angle, radius);
    }
#endif

    #endregion

    private void DetectPlayer()
    {
        HasTarget = false;

        Physics.OverlapSphereNonAlloc(transform.position, radius, results, playerLayerMask.value);

        var player = results[0];
        if (!player)
        {
            return;
        }

        var playerAngle = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        if (playerAngle > angle)
        {
            return;
        }

        HasTarget = true;
    }
}
