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
    private float focusSpeed = 1f;

    [SerializeField]
    private Color focusColor;

    #endregion

    private Vector3 scale;
    private float spotAngle;
    private float spotRange;

    private Vector3 targetDirection;

    #region Event Functions

    private void Update()
    {
        transform.forward = Vector3.Lerp(transform.forward, targetDirection, focusSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, scale, focusSpeed * Time.deltaTime);
        spotlight.range = Mathf.Lerp(spotlight.range, spotRange, focusSpeed * Time.deltaTime);
        spotlight.spotAngle = Mathf.Lerp(spotlight.spotAngle, spotAngle, focusSpeed * Time.deltaTime);
    }

    public void SetTargetPosition(Vector3 targetPosition, bool focus = false)
    {
        visuals.material.color = focus ? focusColor : defaultColor;

        var distance = focus
            ? Vector3.Distance(transform.position, targetPosition)
            : defaultDistance;

        targetDirection = (targetPosition - transform.position).normalized;
        spotAngle = Mathf.Atan2(focusRadius, distance) * Mathf.Rad2Deg;
        spotRange = distance;
        scale = new Vector3(spotAngle * 0.5f, spotAngle * 0.5f, distance);
    }

    #endregion
}
