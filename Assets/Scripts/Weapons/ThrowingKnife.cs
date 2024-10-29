using NeonBlack.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBlack.Weapons
{
    public class ThrowingKnife : Weapon
    {
        [SerializeField]
        private float damage;

        public override void Shoot()
        {
            //ObjectPoolManager.Spawn<ThrowingKnifeProjectile>(ProjectilePrefab, out var projectile);
            //projectile.
        }
    }
}
