using UnityEngine;

public class BossBrain : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private BossEye leftEye;

    [SerializeField]
    private BossEye rightEye;

    [SerializeField]
    private Transform playerTransform;

    #endregion

    private readonly bool canSeePlayer = true;

    #region Event Functions

    private void Awake()
    {
        rightEye.Target = playerTransform;
    }

    #endregion
}
