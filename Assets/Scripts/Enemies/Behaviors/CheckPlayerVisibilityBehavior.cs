using UnityEngine;

public class CheckPlayerVisibilityBehavior : MonoBehaviour
{
    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    [SingleLayer]
    private int playerLayer;

    [SerializeField]
    [SingleLayer]
    private int obstacleLayer;

    #endregion

    private int combinedLayer;

    private Transform playerTransform;

    private RaycastHit raycastHit;

    #region Event Functions

    private void Awake()
    {
        playerTransform = player.VisibilityChecker;
        combinedLayer = (1 << playerLayer) | (1 << obstacleLayer);
    }

    #endregion

    public bool IsPlayerVisible()
    {
        // TODO: add invisible zone check here
        return RaycastToPlayer();
    }

    private bool RaycastToPlayer()
    {
        var direction = playerTransform.position - transform.position;
        var raycast = Physics.Raycast(transform.position, direction.normalized, out raycastHit,
            direction.magnitude,
            combinedLayer);

        return raycast && raycastHit.collider.gameObject.layer == playerLayer;
    }
}
