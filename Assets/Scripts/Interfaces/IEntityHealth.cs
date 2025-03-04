using UnityEngine;

namespace NeonBlack.Interfaces
{
    public enum DamageSource
    {
        Normal,
        DeathZone,
        Missile
    }

    public interface IEntityHealth : IGameObject
    {
        public void TakeDamage(DamageSource source, float dmg, Transform attacker = null);
    }
}
