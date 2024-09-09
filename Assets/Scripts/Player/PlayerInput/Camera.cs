using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInput
{
    #region Serialized Fields

    [Header("Camera")]
    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private Vector2 minMaxCameraYaw = new(5f, 90f);

    [SerializeField]
    private Vector2 minMaxCameraDistance = new(2f, 5f);

    [SerializeField]
    private float cameraHeight = 2f;

    [SerializeField]
    private float cameraZoomStep = 0.2f;

    #endregion

    private float cameraDistance = 3f;
    private Vector2 cameraOrbit = new(45f, 0f);

    private Vector3 CameraTarget
    {
        get
        {
            var pos = transform.position;
            pos.y += cameraHeight;
            return pos;
        }
    }

    private void MoveCamera()
    {
        var moveOffset = actions.CameraMove.ReadValue<Vector2>();

        cameraOrbit.x = Mathf.Clamp(cameraOrbit.x - moveOffset.y, minMaxCameraYaw.x, minMaxCameraYaw.y);
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
        var change = value.y < 0 ? cameraZoomStep : -cameraZoomStep;

        cameraDistance = Mathf.Clamp(cameraDistance + change, minMaxCameraDistance.x, minMaxCameraDistance.y);
    }
}
