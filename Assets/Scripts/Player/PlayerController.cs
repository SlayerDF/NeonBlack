using Cysharp.Threading.Tasks;
using Player;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private PlayerAnimation playerAnimation;

    [SerializeField]
    private Transform visibilityChecker;

    [SerializeField]
    private BossBrain bossBrain;

    [Header("Properties")]
    [SerializeField]
    private float bossStateDebounceTime = 0.5f;

    #endregion

    private DebouncedValue<BossBrain.State> bossStateDebounce;
    private bool killed;

    public Transform VisibilityChecker => visibilityChecker;

    #region Event Functions

    private void Awake()
    {
        bossStateDebounce = new DebouncedValue<BossBrain.State>(() => bossBrain.CurrentState,
            OnBossStateDebouncedChanged, bossStateDebounceTime);
    }

    private void FixedUpdate()
    {
        bossStateDebounce.Update(Time.fixedDeltaTime);
    }

    private void OnEnable()
    {
        LevelState.AlertChanged += OnAlertChanged;
    }

    private void OnDisable()
    {
        LevelState.AlertChanged -= OnAlertChanged;
    }

    #endregion

    private void OnBossStateDebouncedChanged(BossBrain.State state)
    {
        var followsPlayer = state == BossBrain.State.FollowPlayer;

        playerInput.DashEnabled = !followsPlayer;
        PlayerAudio.NotifyDanger(followsPlayer);
    }

    private void OnAlertChanged(float value)
    {
        if (value >= 1)
        {
            Kill();
        }
    }

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
