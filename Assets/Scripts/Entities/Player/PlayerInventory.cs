using System;
using System.Collections.Generic;
using NeonBlack.Weapons;
using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private bool autoChangePickedUpWeapon = true;

        #endregion

        private readonly List<Weapon> weapons = new();
        private readonly Dictionary<Weapon, int> weaponsAmmo = new();

        private int weaponIndex;

        internal Weapon CurrentWeapon { get; private set; }
        internal bool CurrentWeaponHasAmmo => weaponsAmmo[CurrentWeapon] > 0;

        public void AddWeapon(Weapon weapon, int ammo = 1)
        {
            if (weaponsAmmo.TryGetValue(weapon, out var currentAmmo))
            {
                weaponsAmmo[weapon] = currentAmmo + ammo;
                CheckPickedUpWeapon(weapon);
                return;
            }

            weapons.Add(weapon);
            weaponsAmmo.Add(weapon, ammo);
            CheckPickedUpWeapon(weapon);
        }

        public void DecreaseWeaponAmmo(Weapon weapon, int ammo = 1)
        {
            if (!weaponsAmmo.ContainsKey(weapon))
            {
                return;
            }

            weaponsAmmo[weapon] -= ammo;
        }

        internal void PrevWeapon()
        {
            UpdateWeaponIndex(weaponIndex - 1);
        }

        internal void NextWeapon()
        {
            UpdateWeaponIndex(weaponIndex + 1);
        }

        private void CheckPickedUpWeapon(Weapon weapon)
        {
            if (CurrentWeapon == null)
            {
                UpdateActiveWeapon(weapon);
            }
            else if (autoChangePickedUpWeapon && weaponsAmmo[CurrentWeapon] < 1 && weaponsAmmo[weapon] > 0)
            {
                UpdateActiveWeapon(weapon);
            }
        }

        private void UpdateWeaponIndex(int newIndex)
        {
            weaponIndex = Math.Clamp(newIndex, 0, weapons.Count - 1);
            CurrentWeapon = weapons[weaponIndex];
        }

        private void UpdateActiveWeapon(Weapon weapon)
        {
            UpdateWeaponIndex(weapons.IndexOf(weapon));
        }
    }
}
