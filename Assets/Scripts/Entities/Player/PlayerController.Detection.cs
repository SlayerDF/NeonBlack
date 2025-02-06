using System.Linq;
using NeonBlack.Enums;
using NeonBlack.Extensions;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Systems.LevelState;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public partial class PlayerController
    {
        #region Serialized Fields

        [Header("Player detection features")]
        [SerializeField]
        private Transform visibilityChecker;

        [SerializeField]
        private FootstepNoiseMapping[] footstepNoiseSettings;

        [SerializeField]
        private float resetNoiseTime = 0.1f;

        [SerializeField]
        private float footstepDetectionRadius = 5f;

        [SerializeField]
        private float footstepDetectionTime = 1f;

        [SerializeField]
        private EnemyDistractor footstepsDistractorPrefab;

        #endregion

        private readonly RaycastHit[] footstepHits = new RaycastHit[1];
        private float resetNoiseTimer;

        public Transform VisibilityChecker => visibilityChecker;
        public bool IsInShadowZone { get; set; }

        public bool IsInSilenceMode { get; set; }
        public bool IsVisible => !IsInShadowZone;
        public bool IsDetectableBySound => IsVisible && !IsInSilenceMode;

        private void DetectionUpdate()
        {
            if ((resetNoiseTimer += Time.deltaTime) > resetNoiseTime)
            {
                resetNoiseTimer = 0f;
                LevelState.UpdateNoise(0f);
            }
        }

        private void OnFootstep()
        {
            var casts = Physics.RaycastNonAlloc(transform.position + new Vector3(0f, 0.5f, 0f), Vector3.down,
                footstepHits, 0.51f, Layer.Terrain.ToMask());

            if (casts < 1 || terrainController == null)
            {
                AudioManager.Play(AudioManager.FootstepsPrefab, AudioManager.PlayerFootstepsClip, transform.position);
                return;
            }

            if (!IsDetectableBySound)
            {
                return;
            }

            var layer = terrainController.LayerAt(transform.position);
            var noise = footstepNoiseSettings.FirstOrDefault(x => x.TerrainLayer == layer)?.NoiseLevel;
            var clip = AudioManager.PlayerSurfaceFootstepClips.FirstOrDefault(x => x.TerrainLayer == layer)?.Clip;

            noise ??= 0f;
            if (clip == null)
            {
                clip = AudioManager.PlayerFootstepsClip;
            }

            AudioManager.Play(AudioManager.FootstepsPrefab, clip, transform.position);

            LevelState.UpdateNoise(noise.Value);
            resetNoiseTimer = 0f;

            ObjectPoolManager.Spawn<EnemyDistractor>(footstepsDistractorPrefab, out var distractor, true);
            distractor.transform.position = transform.position;
            distractor.DistractionRadius = footstepDetectionRadius * noise.Value;
            distractor.DistractionTime = footstepDetectionTime;
            distractor.gameObject.SetActive(true);
        }
    }
}
