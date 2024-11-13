using NeonBlack.Projectiles;
using UnityEngine;

namespace NeonBlack.Weapons
{
    public abstract class Weapon : ScriptableObject
    {
        #region Serialized Fields

        [Header("Assets")]
        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private Projectile projectilePrefab;

        #endregion

        public Sprite Icon => icon;
        public Projectile ProjectilePrefab => projectilePrefab;

        public abstract void Shoot(Vector3 origin, Vector3 direction);
    }
}
