using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public partial class PlayerInput : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private CharacterController controller;

        [SerializeField]
        private PlayerInventory inventory;

        [SerializeField]
        private PlayerAnimation playerAnimation;

        #endregion

        private InputActions.PlayerAttackActions attackActions;
        private InputActions.PlayerCameraActions cameraActions;

        private Vector2 input;

        private InputActions.PlayerMovementActions movementActions;

        #region Event Functions

        private void Awake()
        {
            movementActions = new InputActions().PlayerMovement;
            attackActions = new InputActions().PlayerAttack;
            cameraActions = new InputActions().PlayerCamera;
        }

        private void Start()
        {
            StartCamera();
        }

        private void Update()
        {
            if (Time.deltaTime <= 0)
            {
                return;
            }

            input = movementActions.Move.ReadValue<Vector2>();

            playerAnimation.SetInputMagnitude(input.SqrMagnitude());

            UpdateCameraTarget();
            RaycastObstacleCamera();

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
            UpdateShoot();
        }

        private void LateUpdate()
        {
            UpdateCameraTarget();
            UpdateCamera();
        }

        private void OnEnable()
        {
            ToggleMovementActions(true);
            ToggleAttackActions(true);
            ToggleCameraActions(true);

            movementActions.Jump.performed += OnJump;
            movementActions.Dash.performed += OnDash;
            attackActions.Attack.started += OnAttackStarted;
            attackActions.Attack.canceled += OnAttackCanceled;
            attackActions.Attack.performed += OnShoot;
            attackActions.Aim.started += OnAimStarted;
            attackActions.Aim.canceled += OnAimCancelled;

            OnEnableAttack();
            OnEnableCamera();
        }

        private void OnDisable()
        {
            ToggleMovementActions(false);
            ToggleAttackActions(false);
            ToggleCameraActions(false);

            movementActions.Jump.performed -= OnJump;
            movementActions.Dash.performed -= OnDash;
            attackActions.Attack.started -= OnAttackStarted;
            attackActions.Attack.canceled -= OnAttackCanceled;
            attackActions.Attack.performed -= OnShoot;
            attackActions.Aim.started -= OnAimStarted;
            attackActions.Aim.canceled -= OnAimCancelled;

            OnDisableAttack();
            OnDisableCamera();
        }

        #endregion

        public void ToggleMovementActions(bool value)
        {
            if (value)
            {
                movementActions.Enable();
            }
            else
            {
                movementActions.Disable();
            }
        }

        public void ToggleAttackActions(bool value)
        {
            if (value)
            {
                attackActions.Enable();
            }
            else
            {
                attackActions.Disable();
            }
        }

        public void ToggleCameraActions(bool value)
        {
            if (value)
            {
                cameraActions.Enable();
            }
            else
            {
                cameraActions.Disable();
            }
        }
    }
}
