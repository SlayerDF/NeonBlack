using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");

    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private Animator animator;

    #endregion

    private Vector3 lastPosition;

    #region Event Functions

    private void FixedUpdate()
    {
        var currentPosition = transform.position;
        currentPosition.y = 0f;

        var velocity = Vector3.SqrMagnitude(currentPosition - lastPosition) / Time.fixedDeltaTime;

        lastPosition = currentPosition;

        animator.SetFloat(Velocity, velocity);
    }

    #endregion

    public void SetIsAttacking(bool value)
    {
        animator.SetBool(IsAttacking, value);
    }
}
