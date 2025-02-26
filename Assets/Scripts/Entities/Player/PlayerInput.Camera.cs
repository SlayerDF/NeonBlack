using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public partial class PlayerInput
    {
        private const float DefaultSensitivity = 0.25f;

        #region Serialized Fields

        [Header("Camera")]
        [SerializeField]
        private Camera playerCamera;

        [SerializeField]
        private Transform cameraDefaultTarget;

        [SerializeField]
        private Transform cameraAimTarget;

        [SerializeField]
        private Vector2 minMaxCameraPitch = new(-40f, 85f);

        [SerializeField]
        private float cameraDefaultZoom = 3f;

        [SerializeField]
        private float cameraAimZoom = 1f;

        [SerializeField]
        private LayerMask cameraCollisionMask;

        [SerializeField]
        private float cameraCollisionRadius = 0.4f;

        [SerializeField]
        private float cameraMinObstacleDistance = 0.1f;

        #endregion

        private Vector3 cameraCurrentTarget;

        private float cameraCurrentZoom;

        private bool cameraHasObstacle;
        private RaycastHit cameraObstacleHit;
        private Vector3 cameraOrbit;
        private float cameraSensitivity;
        private float currentCameraDistance;

        private void StartCamera()
        {
            cameraOrbit.x = (minMaxCameraPitch.x + minMaxCameraPitch.y) / 2f;
            cameraOrbit.y = transform.rotation.eulerAngles.y;
        }

        private void OnEnableCamera()
        {
            Settings.MouseSensitivityChanged += OnMouseSensitivityChanged;

            cameraSensitivity = Settings.MouseSensitivity * DefaultSensitivity;
        }

        private void OnDisableCamera()
        {
            Settings.MouseSensitivityChanged -= OnMouseSensitivityChanged;
        }

        private void OnMouseSensitivityChanged(float value)
        {
            cameraSensitivity = value * DefaultSensitivity;
        }

        private void RaycastObstacleCamera()
        {
            const float raycastSphereRadius = 0.5f;

            var castDirection = Quaternion.Euler(cameraOrbit) * -Vector3.forward;

            cameraHasObstacle = Physics.SphereCast(cameraCurrentTarget, raycastSphereRadius, castDirection,
                out cameraObstacleHit, cameraCurrentZoom + cameraMinObstacleDistance, cameraCollisionMask,
                QueryTriggerInteraction.Ignore);
        }

        // Retrieve camera input
        private void MoveCamera()
        {
            var moveOffset = cameraActions.CameraMove.ReadValue<Vector2>() * cameraSensitivity;

            cameraOrbit.x = Mathf.Clamp(cameraOrbit.x - moveOffset.y, minMaxCameraPitch.x, minMaxCameraPitch.y);
            cameraOrbit.y += moveOffset.x;
        }

        // Update camera rotation and position
        private void UpdateCamera()
        {
            var target = cameraCurrentTarget;
            var lookRotation = Quaternion.Euler(cameraOrbit);
            var lookDirection = lookRotation * Vector3.forward;
            var cameraDistance = cameraCurrentZoom;

            if (cameraHasObstacle)
            {
                cameraDistance = Mathf.Max(0f, cameraObstacleHit.distance - cameraMinObstacleDistance);
            }

            // Set camera position to the circle orbit around the player head if the obstacle is too close
            if (cameraDistance < cameraCollisionRadius)
            {
                cameraDistance = 0f;

                target = new Vector3(
                    target.x - cameraCollisionRadius * Mathf.Sin(cameraOrbit.y * Mathf.Deg2Rad),
                    target.y,
                    target.z - cameraCollisionRadius * Mathf.Cos(cameraOrbit.y * Mathf.Deg2Rad));
            }

            // Smoothly change distance between the camera and the target
            currentCameraDistance = Mathf.MoveTowards(currentCameraDistance, cameraDistance, Time.deltaTime * 10f);

            playerCamera.transform.SetPositionAndRotation(target - lookDirection * currentCameraDistance, lookRotation);
        }

        private void UpdateCameraTarget()
        {
            if (isAiming && IsGrounded)
            {
                cameraCurrentTarget = cameraAimTarget.position;
                cameraCurrentZoom = cameraAimZoom;
            }
            else
            {
                cameraCurrentTarget = cameraDefaultTarget.position;
                cameraCurrentZoom = cameraDefaultZoom;
            }
        }
    }
}
