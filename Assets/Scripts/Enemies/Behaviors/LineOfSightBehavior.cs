using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

    public float DistanceToPlayerNormalized { get; private set; }

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
        if ((detectionTimer += Time.fixedDeltaTime) < detectionFrequency)
        {
            return;
        }

        detectionTimer = 0f;
        DetectPlayer();
    }

    private void OnEnable()
    {
        visualsMeshFilter.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        visualsMeshFilter.gameObject.SetActive(false);
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
#endif

    #endregion

    private void DetectPlayer()
    {
        HasTarget = false;

        var vectorToPlayer = playerTransform.position - transform.position;
        var distanceToPlayerSqr = Vector3.SqrMagnitude(vectorToPlayer);
        DistanceToPlayerNormalized = distanceToPlayerSqr / radiusSqr;

        if (DistanceToPlayerNormalized > 1f)
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

#if UNITY_EDITOR
[CustomEditor(typeof(LineOfSightBehavior))]
[CanEditMultipleObjects]
public class LineOfSightBehaviorEditor : Editor
{
    private SerializedProperty angle;
    private CancellationTokenSource cts;
    private SerializedProperty height;
    private SerializedProperty innerRadius;
    private SerializedProperty previewVisualsMeshGeneration;
    private SerializedProperty radius;
    private SerializedProperty segments;
    private SerializedProperty visualsMeshFilter;

    private bool GenerationEnabled => previewVisualsMeshGeneration.boolValue;
    private MeshFilter MeshFilter => visualsMeshFilter.objectReferenceValue as MeshFilter;

    #region Event Functions

    private void OnEnable()
    {
        previewVisualsMeshGeneration = serializedObject.FindProperty("previewVisualsMeshGeneration");
        visualsMeshFilter = serializedObject.FindProperty("visualsMeshFilter");
        innerRadius = serializedObject.FindProperty("innerRadius");
        radius = serializedObject.FindProperty("radius");
        angle = serializedObject.FindProperty("angle");
        height = serializedObject.FindProperty("height");
        segments = serializedObject.FindProperty("segments");
    }

    #endregion

    private async UniTaskVoid GenerateMesh()
    {
        if (!MeshFilter)
        {
            return;
        }

        if (!GenerationEnabled)
        {
            if (MeshFilter.sharedMesh)
            {
                DestroyImmediate(MeshFilter.sharedMesh);
            }

            return;
        }

        cts?.Cancel();
        cts = new CancellationTokenSource();

        await UniTask.Delay(TimeSpan.FromMilliseconds(100f), cancellationToken: cts.Token);

        if (MeshFilter.sharedMesh)
        {
            DestroyImmediate(MeshFilter.sharedMesh);
        }

        MeshFilter.sharedMesh = MeshGenerator.GenerateLosMesh(innerRadius.floatValue, radius.floatValue,
            angle.floatValue, height.floatValue, segments.intValue);
        MeshFilter.sharedMesh.name = "Generated LineOfSight Mesh";
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        var iterator = serializedObject.GetIterator();
        while (iterator.NextVisible(true))
        {
            EditorGUILayout.PropertyField(iterator, true);
        }

        if (EditorGUI.EndChangeCheck() == false)
        {
            return;
        }

        GenerateMesh().Forget();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
