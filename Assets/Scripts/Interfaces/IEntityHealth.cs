using UnityEngine;

namespace NeonBlack.Interfaces
{
    public enum DamageSource
    {
        Normal,
        DeathZone
    }

    public interface IEntityHealth
    {
        public void TakeDamage(DamageSource source, float dmg, Transform attacker = null);
    }
}
