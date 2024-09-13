using UnityEngine;

public class LookAtPlayerBehavior : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private Transform playerTransform;

    #endregion

    #region Event Functions

    private void Update()
    {
        var targetDirection = (playerTransform.position - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, targetDirection.With(y: 0f), Time.deltaTime * 5f);
    }

    #endregion
}
