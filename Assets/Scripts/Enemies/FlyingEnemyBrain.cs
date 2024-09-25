using System;
using R3;
using Systems.AudioManagement;
using UnityEngine;

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

    #endregion

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
    }

    #endregion

    private void CheckVisibility(bool visible)
    {
        if (!visible)
        {
            return;
        }

        bossBrain.Notify(lineOfSightByPathBehavior.TargetPoint.position);
        AudioManager.Play(AudioManager.EnemiesNotifications, AudioManager.EnemyAlertedClip);
    }
}
