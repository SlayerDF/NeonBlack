using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("Components")]

    [SerializeField]
    private CharacterController controller;

    [Header("Movement")]

    [SerializeField]
    private float moveHorizontalSpeed = 5f;

    [SerializeField]
    private float moveVerticalSpeed = 3f;

    [SerializeField]
    private float rotateSpeed = 5f;

    [SerializeField]
    private float gravity = 3f;

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

    private float currentVerticalSpeed;
    private float cameraDistance = 3f;
    private Vector2 cameraOrbit = new(45f, 0f);

    private InputActions.PlayerActions actions;

    private Vector3 CameraTarget
    {
        get
        {
            var pos = transform.position;
            pos.y += cameraHeight;
            return pos;
        }
    }

    private void Awake()
    {
        actions = new InputActions().Player;
    }

    private void OnEnable()
    {
        actions.Enable();
        actions.CameraZoom.performed += OnCameraZoomChange;
        actions.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        actions.Disable();
        actions.CameraZoom.performed -= OnCameraZoomChange;
        actions.Jump.performed -= OnJump;
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

    private void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded)
        {
            currentVerticalSpeed = moveVerticalSpeed;
        }
    }

    private void MovePlayer()
    {
        var input = actions.Move.ReadValue<Vector2>();
        
        currentVerticalSpeed += -gravity * Time.deltaTime;
        currentVerticalSpeed = Mathf.Clamp(currentVerticalSpeed, -moveVerticalSpeed, moveVerticalSpeed);

        var moveDirection = Vector3.up * currentVerticalSpeed;

        if (input != Vector2.zero)
        {
            moveDirection += transform.forward * input.y + transform.right * input.x;

            RotatePlayer();
        }

        controller.Move(moveHorizontalSpeed * Time.deltaTime * moveDirection);
    }

    private void RotatePlayer()
    {
        var targetRotation = Quaternion.Euler(0, cameraOrbit.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
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

    private void OnCameraZoomChange(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {   
        var value = actions.CameraZoom.ReadValue<Vector2>();
        var change = value.y < 0 ? zoomStep : -zoomStep;

        cameraDistance = Mathf.Clamp(cameraDistance + change, minCameraDistance, maxCameraDistance);
        //UpdateCameraZoom();
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.black;
        Handles.Label(transform.position.With(y: transform.position.y + 2f), cameraOrbit.ToString());

        var dir = Quaternion.Euler(0, cameraOrbit.y, 0) * transform.forward;
        Handles.DrawLine(transform.position, transform.position + dir * 2f);
    }
}
