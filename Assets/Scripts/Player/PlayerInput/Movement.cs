using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInput
{
    #region Serialized Fields

    [Header("Movement")]
    [SerializeField]
    private Vector2 maxSpeed = new(30f, 30f);

    [SerializeField]
    private Vector2 naturalDeceleration = new(1f, 9.8f);

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    [Range(0f, 1f)]
    private float airControl = 0.2f;

    [SerializeField]
    private float jumpInitialSpeed = 5f;

    [SerializeField]
    private Vector2 dashInitialSpeed = new(10f, 3f);

    #endregion

    private Vector2 currentSpeed;
    private float dashTimer;
    private bool isDashing;
    private Vector3 moveDirection;

    private bool IsGrounded => controller.isGrounded && !isDashing;

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!IsGrounded)
        {
            return;
        }

        currentSpeed.y = jumpInitialSpeed;
        playerAnimation.OnJump();
    }

    private void OnDash(InputAction.CallbackContext obj)
    {
        if (isDashing)
        {
            return;
        }

        var input = actions.Move.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            return;
        }

        moveDirection = Vector3.ProjectOnPlane(playerCamera.transform.rotation * input.ToVector3(),
            Vector3.up).normalized;

        currentSpeed = dashInitialSpeed;
        isDashing = true;
        dashTimer = 0f;
        playerAnimation.OnDash();
    }

    private void DashPlayer()
    {
        if (controller.isGrounded && dashTimer > 0.5f)
        {
            isDashing = false;
            return;
        }

        dashTimer += Time.deltaTime;

        ApplyDeceleration();
        ClampSpeed();

        transform.rotation = Quaternion.LookRotation(moveDirection);
        controller.Move((Vector3.up * currentSpeed.y + moveDirection * currentSpeed.x) * Time.deltaTime);
    }

    private void MovePlayer()
    {
        ApplyDeceleration();
        ClampSpeed();

        var input = actions.Move.ReadValue<Vector2>();
        if (input != Vector2.zero)
        {
            ApplyRunAcceleration(Vector2.SqrMagnitude(input));

            var targetDirection = Vector3.ProjectOnPlane(
                playerCamera.transform.rotation * input.ToVector3(),
                Vector3.up).normalized;

            UpdateMoveDirection(targetDirection);
            RotatePlayer(Quaternion.Euler(0, cameraOrbit.y, 0));
        }

        controller.Move((Vector3.up * currentSpeed.y + moveDirection * currentSpeed.x) * Time.deltaTime);
    }

    private void ApplyRunAcceleration(float inputMagnitude)
    {
        if (IsGrounded)
        {
            currentSpeed.x = moveSpeed * inputMagnitude;
        }
        else
        {
            currentSpeed.x += moveSpeed * inputMagnitude * airControl * Time.deltaTime;
        }
    }

    private void ApplyDeceleration()
    {
        if (IsGrounded)
        {
            currentSpeed.x = 0f;
        }
        else
        {
            currentSpeed.x -= naturalDeceleration.x * Time.deltaTime;
        }

        currentSpeed.y -= naturalDeceleration.y * Time.deltaTime;
    }

    private void ClampSpeed()
    {
        currentSpeed.x = Mathf.Clamp(currentSpeed.x, 0, maxSpeed.x);
        currentSpeed.y = Mathf.Clamp(currentSpeed.y, -maxSpeed.y, maxSpeed.y);
    }

    private void UpdateMoveDirection(Vector3 targetDirection)
    {
        if (IsGrounded)
        {
            moveDirection = targetDirection;
        }
        else
        {
            var speed = rotationSpeed * Time.deltaTime;

            if (!IsGrounded)
            {
                speed *= airControl;
            }

            moveDirection = Vector3.Lerp(moveDirection, targetDirection, speed);
        }
    }

    private void RotatePlayer(Quaternion targetRotation)
    {
        var speed = rotationSpeed * Time.deltaTime;

        if (!IsGrounded)
        {
            speed *= airControl;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed);
    }
}
