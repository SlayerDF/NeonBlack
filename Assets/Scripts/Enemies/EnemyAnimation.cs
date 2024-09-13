using UnityEngine;

public class EnemyAnimation : Animation
{
    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    public static readonly int Death = Animator.StringToHash("Death");

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

    public void OnDeath()
    {
        animator.SetTrigger(Death);
    }
}
