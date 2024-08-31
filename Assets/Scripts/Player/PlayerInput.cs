using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    #region Serialized Fields

    [Header("Components")]
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private PlayerAnimation playerAnimation;

    [Header("Movement")]
    [SerializeField]
    private Vector2 maxSpeed = new(5f, 30f);

    [SerializeField]
    private Vector2 naturalDeceleration = new(15f, 9.8f);

    [SerializeField]
    private AnimationCurve runAccelerationCurve = AnimationCurve.EaseInOut(0f, 30f, 5f, 15f);

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    [Range(0f, 1f)]
    private float airControl = 0.2f;

    [SerializeField]
    private float jumpInitialSpeed = 5f;

    [SerializeField]
    private Vector2 flipInitialSpeed = new(10f, 3f);

    [SerializeField]
    private float flipMinSpeed = 2f;

    [Header("Camera")]
    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private float minCameraDistance = 2f;

    [SerializeField]
    private float maxCameraDistance = 5f;

    [SerializeField]
    private float cameraHeight = 2f;

    [SerializeField]
    private float zoomStep = 0.2f;

    #endregion

    private InputActions.PlayerActions actions;

    private float cameraDistance = 3f;
    private Vector2 cameraOrbit = new(45f, 0f);
    private float currentAirControl = 1f;

    private Vector2 currentSpeed;
    private bool isAiming;
    private bool isFlipping;
    private Vector3 moveDirection;

    private Vector3 CameraTarget
    {
        get
        {
            var pos = transform.position;
            pos.y += cameraHeight;
            return pos;
        }
    }

    #region Event Functions

    private void Awake()
    {
        actions = new InputActions().Player;
    }

    private void Update()
    {
        MovePlayer();
        MoveCamera();
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    private void OnEnable()
    {
        actions.Enable();
        actions.CameraZoom.performed += OnCameraZoomChange;
        actions.Aim.performed += OnAim;
        actions.Aim.canceled += OnAim;
        actions.Jump.performed += OnJump;
        actions.Flip.performed += OnFlip;
    }

    private void OnDisable()
    {
        actions.Disable();
        actions.CameraZoom.performed -= OnCameraZoomChange;
        actions.Aim.performed -= OnAim;
        actions.Aim.canceled += OnAim;
        actions.Jump.performed -= OnJump;
        actions.Flip.performed -= OnFlip;
    }

    #endregion

    private void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed) isAiming = true;
        else if (context.canceled) isAiming = false;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded || !playerAnimation.ReadyToJump) return;

        currentSpeed.y = jumpInitialSpeed;
        playerAnimation.OnJump();
    }

    private void OnFlip(InputAction.CallbackContext obj)
    {
        if (!controller.isGrounded || !playerAnimation.ReadyToJump || currentSpeed.x < flipMinSpeed) return;

        currentSpeed = flipInitialSpeed;
        isFlipping = true;
        playerAnimation.OnFlip();
    }

    // TODO: maybe it's good idea to add finite state machine
    private void MovePlayer()
    {
        if (isFlipping) FlipPlayer();
        else if (isAiming) MovePlayerAiming();
        else MovePlayerNormally();
    }

    private void FlipPlayer()
    {
        if (controller.isGrounded)
        {
            isFlipping = false;
            return;
        }

        ApplyDeceleration();
        currentSpeed.x = Mathf.Clamp(currentSpeed.x, 0, flipInitialSpeed.x);
        currentSpeed.y = Mathf.Clamp(currentSpeed.y, -maxSpeed.y, maxSpeed.y);

        controller.Move((Vector3.up * currentSpeed.y + transform.forward * currentSpeed.x) * Time.deltaTime);
    }

    private void MovePlayerNormally()
    {
        ApplyAirControl();
        ApplyDeceleration();
        ClampSpeed();

        var input = actions.Move.ReadValue<Vector2>();
        if (input != Vector2.zero)
        {
            ApplyRunAcceleration();

            var targetDirection = Vector3.ProjectOnPlane(
                playerCamera.transform.rotation * input.ToVector3(),
                Vector3.up).normalized;

            UpdateMoveDirection(targetDirection);
            RotatePlayer(Quaternion.LookRotation(moveDirection));
        }

        controller.Move((Vector3.up * currentSpeed.y + moveDirection * currentSpeed.x) * Time.deltaTime);
    }

    private void MovePlayerAiming()
    {
        ApplyAirControl();
        ApplyDeceleration();
        ClampSpeed();

        var input = actions.Move.ReadValue<Vector2>();
        if (input != Vector2.zero)
        {
            ApplyRunAcceleration();

            var targetDirection = Vector3.ProjectOnPlane(
                transform.rotation * input.ToVector3(),
                Vector3.up).normalized;

            UpdateMoveDirection(targetDirection);
            RotatePlayer(Quaternion.Euler(0, cameraOrbit.y, 0));
        }

        controller.Move((Vector3.up * currentSpeed.y + moveDirection * currentSpeed.x) * Time.deltaTime);
    }

    private void ApplyAirControl()
    {
        currentAirControl = controller.isGrounded ? 1f : airControl;
    }

    private void ApplyRunAcceleration()
    {
        var acceleration = runAccelerationCurve.Evaluate(currentSpeed.x) * currentAirControl;

        currentSpeed.x += acceleration * Time.deltaTime;
        currentSpeed.x = Mathf.Clamp(currentSpeed.x, 0, maxSpeed.x);
    }

    private void ApplyDeceleration()
    {
        if (controller.isGrounded) currentSpeed.x -= naturalDeceleration.x * Time.deltaTime;

        currentSpeed.y -= naturalDeceleration.y * Time.deltaTime;
    }

    private void ClampSpeed()
    {
        currentSpeed.x = Mathf.Clamp(currentSpeed.x, 0, maxSpeed.x);
        currentSpeed.y = Mathf.Clamp(currentSpeed.y, -maxSpeed.y, maxSpeed.y);
    }

    private void UpdateMoveDirection(Vector3 targetDirection)
    {
        var speed = rotationSpeed * Time.deltaTime * currentAirControl;

        moveDirection = Vector3.Lerp(moveDirection, targetDirection, speed);
    }

    private void RotatePlayer(Quaternion targetRotation)
    {
        var speed = rotationSpeed * Time.deltaTime * currentAirControl;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed);
    }

    private void MoveCamera()
    {
        var moveOffset = actions.CameraMove.ReadValue<Vector2>();

        cameraOrbit.x = Mathf.Clamp(cameraOrbit.x - moveOffset.y, 5f, 90f);
        cameraOrbit.y += moveOffset.x;
    }

    private void UpdateCamera()
    {
        var lookRotation = Quaternion.Euler(cameraOrbit);
        var lookDirection = lookRotation * Vector3.forward;
        var lookPosition = CameraTarget - lookDirection * cameraDistance;
        playerCamera.transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    private void OnCameraZoomChange(InputAction.CallbackContext obj)
    {
        var value = actions.CameraZoom.ReadValue<Vector2>();
        var change = value.y < 0 ? zoomStep : -zoomStep;

        cameraDistance = Mathf.Clamp(cameraDistance + change, minCameraDistance, maxCameraDistance);
    }
}
