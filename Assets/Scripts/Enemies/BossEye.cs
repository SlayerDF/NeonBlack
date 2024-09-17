using UnityEngine;

public class BossEye : MonoBehaviour
{
    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private MeshRenderer visuals;

    [SerializeField]
    private Light spotlight;

    [Header("Default behaviour")]
    [SerializeField]
    private float defaultRadius;

    [SerializeField]
    private float defaultDistance;

    [SerializeField]
    private Color defaultColor;

    [Header("Focus behaviour")]
    [SerializeField]
    private float focusRadius;

    [SerializeField]
    private float focusSpeed;

    [SerializeField]
    private Color focusColor;

    #endregion

    private Vector3 scale;
    private float spotAngle;
    private float spotRange;

    private Vector3 targetDirection;

    [Header("Properties")]
    private float updateTargetFrequency = 1f;

    private float updateTargetTime;

    public Transform Target { get; set; }

    #region Event Functions

    private void Update()
    {
        transform.forward = Vector3.Lerp(transform.forward, targetDirection, focusSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, scale, focusSpeed * Time.deltaTime);
        spotlight.range = Mathf.Lerp(spotlight.range, spotRange, focusSpeed * Time.deltaTime);
        spotlight.spotAngle = Mathf.Lerp(spotlight.spotAngle, spotAngle, focusSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if ((updateTargetTime += Time.fixedDeltaTime) < updateTargetFrequency)
        {
            return;
        }

        updateTargetTime = 0f;

        UpdateTarget();
    }

    #endregion

    private void UpdateTarget()
    {
        visuals.material.color = Target != null ? focusColor : defaultColor;

        var distance = Target != null
            ? Vector3.Distance(transform.position, Target.position)
            : defaultDistance;

        targetDirection = Target != null ? Target.position - transform.position : Vector3.zero;
        spotAngle = Mathf.Atan2(focusRadius, distance) * Mathf.Rad2Deg;
        spotRange = distance;
        scale = new Vector3(spotAngle * 0.5f, spotAngle * 0.5f, distance);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!preview)
        {
            return;
        }

        Target = previewTarget;
        Update();
    }


    [Header("Debug")]
    [SerializeField]
    private bool preview;

    [SerializeField]
    private Transform previewTarget;
#endif
}
