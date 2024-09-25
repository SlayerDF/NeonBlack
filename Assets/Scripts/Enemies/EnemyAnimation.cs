using UnityEngine;

public class EnemyAnimation : Animation
{
    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int IsNotifyingBoss = Animator.StringToHash("IsNotifyingBoss");
    public static readonly int Death = Animator.StringToHash("Death");

    private Vector3 lastPosition;

    #region Event Functions

    private void Update()
    {
        if (Time.deltaTime <= 0)
        {
            return;
        }

        var currentPosition = transform.position;
        currentPosition.y = 0f;

        var velocity = Vector3.SqrMagnitude(currentPosition - lastPosition) / Time.deltaTime;

        lastPosition = currentPosition;

        animator.SetFloat(Velocity, velocity);
    }

    #endregion

    public void SetIsAttacking(bool value)
    {
        animator.SetBool(IsAttacking, value);
    }

    public void SetIsNotifyingBoss(bool value)
    {
        animator.SetBool(IsNotifyingBoss, value);
    }

    public void OnDeath()
    {
        animator.SetTrigger(Death);
    }
}
