using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [SerializeField]
    private FollowPlayerBehavior followPlayerBehavior;

    private void Awake()
    {
        followPlayerBehavior.Awake(this);
    }

    private void Start()
    {
        followPlayerBehavior.Start();
    }

    private void FixedUpdate()
    {
        followPlayerBehavior.Update(Time.fixedDeltaTime);
    }
}
