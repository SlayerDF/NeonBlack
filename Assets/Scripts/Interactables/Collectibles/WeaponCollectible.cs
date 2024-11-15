using NeonBlack.Entities.Player;
using NeonBlack.Weapons;
using UnityEngine;

namespace NeonBlack.Interactables
{
    public class WeaponCollectible : Collectible<PlayerInventory>
    {
        #region Serialized Fields

        [SerializeField]
        private Weapon weapon;

        [SerializeField]
        private int ammoCount = 1;

        #endregion

        protected override void OnCollect(PlayerInventory playerInventory)
        {
            playerInventory.AddWeapon(weapon, ammoCount);
        }
    }
}
