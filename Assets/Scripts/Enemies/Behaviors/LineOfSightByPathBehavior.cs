using UnityEngine;
using static Path;

/// <summary>
/// Contains the logic to smoothly transition between path points and check if a player
/// is inside a target sphere. Where path point is a radius of the target sphere.
/// </summary>
public class LineOfSightByPathBehavior : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private float changePointTimeSec = 3.0f;

    [SerializeField]
    private float walkingSpeed = 0.01f;

    [SerializeField]
    private float sphereRadius = 1.0f;

    [SerializeField]
    private Transform targetPointVisuals;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private Path path;

    #endregion Serialized Fields

    private Vector3 currentTargetPosition;
    private float radiusSqr;

    private bool waiting;
    private float waitTimer;

    private Waypoint currentWaypoint;

    /// <summary>
    /// True if player is detected in the current moment.
    /// </summary>
    public bool IsPlayerDetected { get; private set; }

    public Vector3 CurrentTargetPosition => currentTargetPosition;

    #region Event Functions

    private void Awake()
    {
        radiusSqr = sphereRadius * sphereRadius;
    }

    private void Start()
    {
        currentWaypoint = path.NextWaypoint();
        SetTargetPosition(currentWaypoint.Position);
    }

    private void FixedUpdate()
    {
        var d =
            Mathf.Pow(player.transform.position.x - currentTargetPosition.x, 2) +
            Mathf.Pow(player.transform.position.y - currentTargetPosition.y, 2) +
            Mathf.Pow(player.transform.position.z - currentTargetPosition.z, 2);

        IsPlayerDetected = d < radiusSqr;

        if (waiting)
        {
            // Wait on the path point for some time.
            if (waitTimer < changePointTimeSec)
            {
                waitTimer += Time.fixedDeltaTime;
                return;
            }

            waiting = false;
            waitTimer = 0.0f;

            // Go to the next point.
            currentWaypoint = path.NextWaypoint(currentWaypoint);
        }
        else if (Vector3.SqrMagnitude(currentTargetPosition - currentWaypoint.Position) < 0.1f) // Check if we arrived to the next path point.
        {
            waiting = true;
            return;
        }

        // Go the next path point smoothly.
        SetTargetPosition(Vector3.Lerp(currentTargetPosition, currentWaypoint.Position, walkingSpeed * Time.fixedDeltaTime));
    }

    private void OnEnable()
    {
        if (targetPointVisuals != null)
        {
            targetPointVisuals.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (targetPointVisuals != null)
        {
            targetPointVisuals.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentTargetPosition, sphereRadius);
    }

    #endregion Event Functions

    private void SetTargetPosition(Vector3 position)
    {
        currentTargetPosition = position;

        if (targetPointVisuals != null)
        {
            targetPointVisuals.position = currentTargetPosition;
        }
    }
}