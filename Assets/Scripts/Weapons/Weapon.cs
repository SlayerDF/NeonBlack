using NeonBlack.Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBlack.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField]
        private float shootInterval;

        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField]
        private Sprite icon;

        protected float ShootInterval => shootInterval;
        protected Projectile ProjectilePrefab => projectilePrefab;
        public Sprite Icon => icon;

        public abstract void Shoot();
    }
}
