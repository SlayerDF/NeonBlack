using UnityEditor;
using UnityEngine;

public class LineOfSightBehavior : MonoBehaviour
{
    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private MeshFilter visualsMeshFilter;

    [Header("Properties")]
    [SerializeField]
    [Range(1f, 360f)]
    private float angle;

    [SerializeField]
    [Range(1f, 50f)]
    private float radius;

    [SerializeField]
    [Range(0.1f, 2f)]
    private float detectionFrequency = 0.1f;

    [Header("Visuals properties")]
    [SerializeField]
    [Range(0.1f, 20f)]
    private float innerRadius = 2f;

    [SerializeField]
    [Range(0f, 10f)]
    private float height = 1.5f;

    [SerializeField]
    [Range(1, 20)]
    private int segments = 8;

    [Header("Debug")]
    [SerializeField]
    private bool showDetectorGizmo = true;

    [SerializeField]
    private bool previewVisualsMeshGeneration;

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

        if (!visualsMeshFilter)
        {
            return;
        }

        visualsMeshFilter.mesh = MeshGenerator.GenerateLosMesh(innerRadius, radius, angle, height, segments);
        visualsMeshFilter.mesh.name = "Generated LineOfSight Mesh";
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDetectorGizmo)
        {
            return;
        }

        Handles.color = HasTarget ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        var from = Quaternion.Euler(0f, -angleCentralized, 0f) * transform.forward;
        Handles.DrawSolidArc(transform.position, Vector3.up, from, angle, radius);
    }

    private bool generationInProgress;

    private void OnValidate()
    {
        if (generationInProgress)
        {
            return;
        }

        EditorApplication.update += GeneratePreviewMesh;
        generationInProgress = true;
    }

    private void GeneratePreviewMesh()
    {
        EditorApplication.update -= GeneratePreviewMesh;

        if (!previewVisualsMeshGeneration || !visualsMeshFilter)
        {
            return;
        }

        visualsMeshFilter.sharedMesh = MeshGenerator.GenerateLosMesh(innerRadius, radius, angle, height, segments);
        visualsMeshFilter.sharedMesh.name = "Generated LineOfSight Mesh";

        generationInProgress = false;
    }
#endif
}
