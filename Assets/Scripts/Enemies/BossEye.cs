using JetBrains.Annotations;
using UnityEngine;

public class BossEye : MonoBehaviour
{
    private static readonly string EmissionName = "_EmissionColor";
    private static readonly int Emission = Shader.PropertyToID(EmissionName);

    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private MeshRenderer visuals;

    [SerializeField]
    private Light spotlight;

    [Header("Behaviors")]
    [SerializeField]
    private LookAtTargetBehavior lookAtTargetBehavior;

    [Header("Default behaviour")]
    [SerializeField]
    private float defaultRadius;

    [SerializeField]
    private float defaultDistance;

    [SerializeField]
    [ColorUsage(true, true)]
    private Color defaultColor;

    [Header("Focus behaviour")]
    [SerializeField]
    private float focusRadius;

    [SerializeField]
    [ColorUsage(true, true)]
    private Color focusColor;

    #endregion

    private Color currentColor;

    private Vector3 scale;
    private float spotAngle;
    private float spotRange;

    [CanBeNull]
    private Transform target;

    private Color targetColor;

    public float FocusSpeed { get; set; }

    public bool IsFocused { get; set; }

    [CanBeNull]
    public Transform Target
    {
        get => target;
        set
        {
            target = value;
            lookAtTargetBehavior.Target = target;
        }
    }

    #region Event Functions

    private void Awake()
    {
        scale = transform.localScale;
        spotRange = spotlight.range;
        spotAngle = CalculateSpotAngle(defaultRadius, defaultDistance);
        targetColor = visuals.material.color;

        visuals.material.EnableKeyword(EmissionName);
    }

    private void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, scale, FocusSpeed * Time.deltaTime);
        spotlight.range = Mathf.MoveTowards(spotlight.range, spotRange, FocusSpeed * Time.deltaTime);
        spotlight.spotAngle = Mathf.MoveTowards(spotlight.spotAngle, spotAngle, FocusSpeed * Time.deltaTime);

        currentColor = currentColor.MoveTowards(targetColor, FocusSpeed * Time.deltaTime);
        visuals.material.color = currentColor;
        visuals.material.SetColor(Emission, currentColor);
    }

    private void FixedUpdate()
    {
        var distance = IsFocused && target
            ? Vector3.Distance(transform.position, target.position)
            : defaultDistance;

        var radius = IsFocused ? focusRadius : defaultRadius;
        var diameter = radius * 2f;

        // Spotlight should always reach actual distance
        spotAngle = CalculateSpotAngle(radius, distance);
        spotRange = distance;

        // Visuals width and height should match the spotlight
        scale = new Vector3(diameter, diameter, distance);

        targetColor = IsFocused ? focusColor : defaultColor;
    }

    #endregion

    public bool CanSeePoint(Vector3 point)
    {
        if (target == null)
        {
            return false;
        }

        var direction = point - transform.position;
        var angle = Vector3.Angle(transform.forward, direction.normalized);
        var testAngle = CalculateSpotAngle(IsFocused ? focusRadius : defaultRadius, direction.magnitude) * 0.5f;

        return angle <= testAngle;
    }

    private static float CalculateSpotAngle(float radius, float distance)
    {
        return Mathf.Atan2(radius, distance) * Mathf.Rad2Deg * 2f;
    }
}
