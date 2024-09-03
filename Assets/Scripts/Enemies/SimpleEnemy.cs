using UnityEngine;

public class SimpleEnemy : Enemy
{
    [SerializeField]
    private FollowPlayerBehavior followPlayerBehavior;

    [SerializeField]
    private LineOfSightBehavior lineOfSightBehavior;

    protected override EnemyBehavior[] RegisterBehaviors()
    {
        return new EnemyBehavior[] { followPlayerBehavior, lineOfSightBehavior };
    }
}
