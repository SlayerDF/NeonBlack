using NeonBlack.Interactables;
using NeonBlack.Interfaces;
using NeonBlack.Systems.LevelState;
using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public partial class PlayerController : MonoBehaviour, IEntityHealth, ILosBehaviorTarget
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private PlayerInput playerInput;

        [SerializeField]
        private PlayerAnimation playerAnimation;

        [SerializeField]
        private TerrainController terrainController;

        #endregion

        #region Event Functions

        private void Update()
        {
            DetectionUpdate();
        }

        private void OnEnable()
        {
            LevelState.AlertChanged += OnAlertChanged;
            playerAnimation.FootstepClipPlayed += OnFootstep;
            playerInput.EnemyHit += OnEnemyHit;
        }

        private void OnDisable()
        {
            LevelState.AlertChanged -= OnAlertChanged;
            playerAnimation.FootstepClipPlayed -= OnFootstep;
            playerInput.EnemyHit -= OnEnemyHit;
        }

        #endregion
    }
}
