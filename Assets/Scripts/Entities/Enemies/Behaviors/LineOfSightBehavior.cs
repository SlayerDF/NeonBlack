using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeonBlack.Interfaces;
using NeonBlack.Utilities;
using UnityEditor;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Behaviors
{
    public class LineOfSightBehavior : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private MeshFilter visualsMeshFilter;

        [SerializeField]
        private SubscribableCollider losCollider;

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

        private readonly Dictionary<Type, List<ILosBehaviorTarget>> activeTargets = new();
        private readonly Dictionary<ILosBehaviorTarget, float> activeTargetsNormalizedDistances = new();
        private readonly List<ILosBehaviorTarget> targets = new();
        private int activeTargetsCount;

        private float angleCentralized;
        private float detectionTimer;

        private float radiusSqr;

        public bool HasAnyTarget => activeTargetsCount > 0;

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
            CheckTargets();
        }

        private void OnEnable()
        {
            visualsMeshFilter.gameObject.SetActive(true);
            losCollider.TriggerEnter += OnLosColliderTriggerEnter;
            losCollider.TriggerExit += OnLosColliderTriggerExit;
        }

        private void OnDisable()
        {
            visualsMeshFilter.gameObject.SetActive(false);
            losCollider.TriggerEnter -= OnLosColliderTriggerEnter;
            losCollider.TriggerExit -= OnLosColliderTriggerExit;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showDetectorGizmo)
            {
                return;
            }

            Handles.color = HasAnyTarget ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

            var from = Quaternion.Euler(0f, -angle * 0.5f, 0f) * transform.forward;
            Handles.DrawSolidArc(transform.position, Vector3.up, from, angle, radius);
        }
#endif

        #endregion

        private void OnLosColliderTriggerEnter(Collider coll)
        {
            if (!coll.TryGetComponent<ILosBehaviorTarget>(out var target))
            {
                return;
            }

            targets.Add(target);
        }

        private void OnLosColliderTriggerExit(Collider coll)
        {
            if (!coll.TryGetComponent<ILosBehaviorTarget>(out var target))
            {
                return;
            }

            targets.Remove(target);
            DeactivateTarget(target);
        }

        private void ActivateTarget(ILosBehaviorTarget target, float normalizedDistance)
        {
            var type = target.GetType();

            if (!activeTargets.TryGetValue(type, out var list))
            {
                list = new List<ILosBehaviorTarget>();
                activeTargets.Add(type, list);
            }

            list.Add(target);
            activeTargetsCount++;

            activeTargetsNormalizedDistances.Add(target, normalizedDistance);
        }

        private void DeactivateTarget(ILosBehaviorTarget target)
        {
            if (activeTargets.TryGetValue(target.GetType(), out var list) && list.Remove(target))
            {
                activeTargetsCount--;
            }

            activeTargetsNormalizedDistances.Remove(target);
        }

        private bool IsTargetActive(ILosBehaviorTarget target)
        {
            return activeTargets.TryGetValue(target.GetType(), out var list) && list.Contains(target);
        }

        private void CheckTargets()
        {
            foreach (var target in targets)
            {
                var active = IsTargetActive(target);
                var targetVector = target.transform.position - transform.position;
                var distanceToTargetSqr = Vector3.SqrMagnitude(targetVector);

                if (distanceToTargetSqr / radiusSqr > 1f)
                {
                    if (active)
                    {
                        DeactivateTarget(target);
                    }

                    continue;
                }

                var targetAngle = Vector3.Angle(transform.forward, Vector3.ProjectOnPlane(targetVector, Vector3.up));
                if (targetAngle > angleCentralized)
                {
                    if (active)
                    {
                        DeactivateTarget(target);
                    }

                    continue;
                }

                if (active)
                {
                    activeTargetsNormalizedDistances[target] = distanceToTargetSqr;
                }
                else
                {
                    ActivateTarget(target, distanceToTargetSqr);
                }
            }
        }

        public IEnumerable<T> FindTargets<T>() where T : ILosBehaviorTarget
        {
            return activeTargets.TryGetValue(typeof(T), out var list) ? list.Cast<T>() : Enumerable.Empty<T>();
        }

        public T FirstTarget<T>() where T : ILosBehaviorTarget
        {
            return FindTargets<T>().FirstOrDefault();
        }

        public bool HasTarget<T>() where T : ILosBehaviorTarget
        {
            return FindTargets<T>().Any();
        }

        public float? NormalizedDistanceToTarget<T>(T target) where T : ILosBehaviorTarget
        {
            if (target == null || !activeTargetsNormalizedDistances.TryGetValue(target, out var distance))
            {
                return null;
            }

            return distance;
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
}
