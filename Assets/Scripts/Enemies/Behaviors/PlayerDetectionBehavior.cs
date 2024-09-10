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

    private float detectionLevel;

    public bool CanSeePlayer { get; set; }

    public float DistanceToPlayerNormalized { get; set; }

    public bool PlayerIsDetected { get; private set; }

    #region Event Functions

    private void FixedUpdate()
    {
        if (CanSeePlayer)
        {
            detectionLevel += Time.fixedDeltaTime * detectionHeatingRate *
                              detectionDistanceMultiplier.Evaluate(DistanceToPlayerNormalized);
        }
        else
        {
            detectionLevel -= Time.fixedDeltaTime * detectionCoolingRate;
        }

        detectionLevel = Mathf.Clamp(detectionLevel, 0f, 1f);

        PlayerIsDetected = detectionLevel >= detectionThreshold;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = detectionLevel < detectionThreshold
            ? Color.Lerp(Color.green, Color.red, detectionLevel)
            : Color.red;

        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, detectionLevel * 360f, 1f);
    }
#endif

    #endregion
}
