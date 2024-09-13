using Player;
using UnityEngine;

public partial class PlayerInput : MonoBehaviour
{
    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private PlayerAnimation playerAnimation;

    #endregion

    private InputActions.PlayerActions actions;
    private Vector2 input;

    public bool MovementEnabled { get; set; } = true;

    #region Event Functions

    private void Awake()
    {
        actions = new InputActions().Player;
    }

    private void Update()
    {
        input = MovementEnabled ? actions.Move.ReadValue<Vector2>() : Vector2.zero;

        playerAnimation.SetInputMagnitude(input.SqrMagnitude());

        if (isDashing)
        {
            DashPlayer();
        }
        else
        {
            MovePlayer();
        }

        MoveCamera();
        UpdateAttack();
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    private void OnEnable()
    {
        actions.Enable();
        actions.CameraZoom.performed += OnCameraZoomChange;
        actions.Jump.performed += OnJump;
        actions.Dash.performed += OnDash;
        actions.Attack.started += OnAttackStarted;
        actions.Attack.canceled += OnAttackCanceled;

        OnEnableAttack();
    }

    private void OnDisable()
    {
        actions.Disable();
        actions.CameraZoom.performed -= OnCameraZoomChange;
        actions.Jump.performed -= OnJump;
        actions.Dash.performed -= OnDash;
        actions.Attack.started -= OnAttackStarted;
        actions.Attack.canceled -= OnAttackCanceled;

        OnDisableAttack();
    }

    #endregion
}
