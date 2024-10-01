using System;
using NeonBlack.Entities.Enemies.Behaviors;
using NeonBlack.Extensions;
using NeonBlack.Systems.AudioManagement;
using R3;
using UnityEngine;

namespace NeonBlack.Entities.Enemies
{
    public class FlyingEnemyBrain : MonoBehaviour
    {
        private const float CheckVisibilityInterval = 0.5f;

        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private BossBrain bossBrain;

        [Header("Behaviors")]
        [SerializeField]
        private LineOfSightByPathBehavior lineOfSightByPathBehavior;

        [SerializeField]
        private CheckPlayerVisibilityBehavior checkPlayerVisibilityBehavior;

        [SerializeField]
        private PlayerDetectionBehavior playerDetectionBehavior;

        [Header("Visuals")]
        [SerializeField]
        private MeshRenderer lineOfSightVisuals;

        [SerializeField]
        private Light targetPointLight;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color highAlertColor;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color lowAlertColor;

        #endregion

        private Color currentAlertColor;

        #region Event Functions

        private void Awake()
        {
            playerDetectionBehavior.PlayerIsDetected.Debounce(TimeSpan.FromSeconds(CheckVisibilityInterval))
                .Subscribe(CheckVisibility).AddTo(this);
        }

        private void FixedUpdate()
        {
            playerDetectionBehavior.CanSeePlayer =
                lineOfSightByPathBehavior.IsPlayerDetected && checkPlayerVisibilityBehavior.IsPlayerVisible();

            var color = Color.Lerp(lowAlertColor, highAlertColor, playerDetectionBehavior.DetectionLevel);
            lineOfSightVisuals.material.SetEmissionColor(color);

            targetPointLight.color = color;
        }

        #endregion

        private void CheckVisibility(bool visible)
        {
            if (!visible)
            {
                return;
            }

            bossBrain.Notify(lineOfSightByPathBehavior.TargetPoint.position);
            AudioManager.Play(AudioManager.EnemiesNotificationsPrefab, AudioManager.EnemyAlertedClip,
                transform.position);
        }
    }
}
