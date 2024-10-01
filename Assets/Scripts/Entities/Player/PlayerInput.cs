using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public partial class PlayerInput : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private CharacterController controller;

        #endregion

        private InputActions.PlayerAttackActions attackActions;
        private InputActions.PlayerCameraActions cameraActions;

        private Vector2 input;

        private InputActions.PlayerMovementActions movementActions;

        [SerializeField]
        private PlayerAnimation playerAnimation;

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
        }

        private void LateUpdate()
        {
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
            cameraActions.CameraZoom.performed += OnCameraZoomChange;

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
            cameraActions.CameraZoom.performed -= OnCameraZoomChange;

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
