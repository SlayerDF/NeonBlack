using R3;
using UnityEditor;
using UnityEngine;

public class PlayerDetectionBehavior : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private float detectionHeatingRate = 0.4f;

    [SerializeField]
    private float detectionCoolingRate = 0.05f;

    [SerializeField]
    private float detectionThreshold = 0.9f;

    [SerializeField]
    private AnimationCurve detectionDistanceMultiplier = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    #endregion

    private readonly ReactiveProperty<bool> playerIsDetected = new();

    public bool CanSeePlayer { get; set; }

    public float DistanceToPlayerNormalized { get; set; }

    public ReadOnlyReactiveProperty<bool> PlayerIsDetected => playerIsDetected;

    public float DetectionLevel { get; private set; }

    #region Event Functions

    private void FixedUpdate()
    {
        if (CanSeePlayer)
        {
            DetectionLevel += Time.fixedDeltaTime * detectionHeatingRate *
                              detectionDistanceMultiplier.Evaluate(DistanceToPlayerNormalized);
        }
        else
        {
            DetectionLevel -= Time.fixedDeltaTime * detectionCoolingRate;
        }

        DetectionLevel = Mathf.Clamp(DetectionLevel, 0f, 1f);

        playerIsDetected.Value = DetectionLevel >= detectionThreshold;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = DetectionLevel < detectionThreshold
            ? Color.Lerp(Color.green, Color.red, DetectionLevel)
            : Color.red;

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, DetectionLevel * 360f, 1f);
    }
#endif

    #endregion
}
