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
        private float noiseResetTime = 0.1f;

        [SerializeField]
        private float footstepNoiseRadius = 5f;

        [SerializeField]
        private float hitNoiseRadius = 20f;

        [SerializeField]
        private float noiseDistractionTime = 1f;

        [SerializeField]
        private EnemyDistractor noiseDistractorPrefab;

        #endregion

        private readonly RaycastHit[] footstepHits = new RaycastHit[1];
        private float resetNoiseTimer;
        public bool IsInShadowZone { get; set; }

        public bool IsInSilenceMode { get; set; }
        public bool IsDetectableBySound => IsVisible && !IsInSilenceMode;

        #region ICheckVisibilityBehaviorTarget Members

        public Transform VisibilityChecker => visibilityChecker;
        public bool IsVisible => !IsInShadowZone;
        public Layer VisibilityLayer => Layer.Player;

        #endregion

        private void DetectionUpdate()
        {
            if ((resetNoiseTimer += Time.deltaTime) > noiseResetTime)
            {
                resetNoiseTimer = 0f;
                LevelState.UpdateNoise(0f);
            }
        }

        private void OnFootstep()
        {
            var casts = Physics.RaycastNonAlloc(transform.position + new Vector3(0f, 0.5f, 0f), Vector3.down,
                footstepHits, 0.51f, Layer.Terrain.ToMask());
            var layer = casts > 0 && terrainController ? terrainController.LayerAt(transform.position) : null;

            if (layer == null)
            {
                AudioManager.Play(AudioManager.FootstepsPrefab, AudioManager.PlayerFootstepsClip, transform.position);
                return;
            }

            if (!IsDetectableBySound)
            {
                return;
            }

            var noise = footstepNoiseSettings.FirstOrDefault(x => x.TerrainLayer == layer)?.NoiseLevel;
            var clip = AudioManager.PlayerSurfaceFootstepClips.FirstOrDefault(x => x.TerrainLayer == layer)?.Clip;

            noise ??= 0f;
            if (clip == null)
            {
                clip = AudioManager.PlayerFootstepsClip;
            }

            SpawnNoise(AudioManager.FootstepsPrefab, clip, noise.Value, footstepNoiseRadius * noise.Value);
        }

        private void OnEnemyHit(Transform enemyTransform)
        {
            SpawnNoise(AudioManager.HitsPrefab, AudioManager.PlayerHitResultClip, 1f, hitNoiseRadius);
        }

        private void SpawnNoise(SpatialAudio spatialAudio, AudioClip clip, float value, float radius)
        {
            if (!IsDetectableBySound)
            {
                return;
            }

            AudioManager.Play(spatialAudio, clip, transform.position);

            LevelState.UpdateNoise(value);
            resetNoiseTimer = 0f;

            ObjectPoolManager.Spawn<EnemyDistractor>(noiseDistractorPrefab, out var distractor, true);
            distractor.transform.position = transform.position;
            distractor.DistractionRadius = radius;
            distractor.DistractionTime = noiseDistractionTime;
            distractor.gameObject.SetActive(true);
        }
    }
}
