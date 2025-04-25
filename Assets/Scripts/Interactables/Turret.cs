using System;
using JetBrains.Annotations;
using NeonBlack.Interfaces;
using NeonBlack.Projectiles;
using NeonBlack.Systems.AudioManagement;
using NeonBlack.Utilities;
using UnityEngine;

namespace NeonBlack.Interactables
{
    public class Turret : MonoBehaviour, IPlayerInteractable, IActivatable
    {
        private static readonly int AnimDestroyedTrigger = Animator.StringToHash("Destroyed");

        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private MissileProjectile missileProjectilePrefab;

        [SerializeField]
        private Transform projectileSpawnPoint;

        [SerializeField]
        private Transform target;

        #endregion

        #region IActivatable Members

        public bool IsActivated { get; private set; }
        public event Action Activated;

        #endregion

        #region IPlayerInteractable Members

        public bool CanBeInteracted => !IsActivated;

        public void Interact()
        {
            IsActivated = true;

            SceneObjectPool.Spawn<MissileProjectile>(missileProjectilePrefab, out var missileProjectile, true);
            missileProjectile.transform.position = projectileSpawnPoint.position;
            missileProjectile.transform.rotation = projectileSpawnPoint.rotation;
            missileProjectile.Target = target;
            missileProjectile.gameObject.SetActive(true);

            AudioManager.Play(AudioManager.EnvironmentsPrefab, AudioManager.MissileLaunchClip, transform.position);
            animator.SetTrigger(AnimDestroyedTrigger);
        }

        #endregion

        [UsedImplicitly]
        private void OnHit()
        {
            AudioManager.Play(AudioManager.HitsPrefab, AudioManager.TurretDestroyClip, transform.position);
        }
    }
}
