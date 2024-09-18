using Cysharp.Threading.Tasks;
using Player;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private PlayerAnimation playerAnimation;

    [SerializeField]
    private Transform visibilityChecker;

    #endregion

    private bool killed;

    public Transform VisibilityChecker => visibilityChecker;

    public void Kill()
    {
        if (killed)
        {
            return;
        }

        killed = true;
        playerInput.MovementEnabled = false;
        playerAnimation.OnDeath();

        playerAnimation.WaitAnimationEnd(PlayerAnimation.Death, 0).ContinueWith(SceneLoader.RestartLevel);
    }
}
