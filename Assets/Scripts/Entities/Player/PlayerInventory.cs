using System;
using System.Collections.Generic;
using NeonBlack.Systems.AudioManagement;
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

        public void AddWeapon(Weapon weapon, int ammoChange = 1)
        {
            if (weaponsAmmo.ContainsKey(weapon))
            {
                UpdateWeaponAmmo(weapon, ammoChange);
                CheckPickedUpWeapon(weapon);
                return;
            }

            weapons.Add(weapon);
            weaponsAmmo.Add(weapon, ammoChange);
            WeaponAdded?.Invoke(weapons.Count - 1, weapon, ammoChange);

            AudioManager.Play(AudioManager.InteractionsPrefab, AudioManager.ItemPickupClip, transform.position);

            CheckPickedUpWeapon(weapon);
        }

        public void DecreaseWeaponAmmo(Weapon weapon, int ammoChange = 1)
        {
            if (!weaponsAmmo.ContainsKey(weapon))
            {
                Debug.LogWarning("Decreasing ammo of a non-existent weapon");
                return;
            }

            UpdateWeaponAmmo(weapon, -ammoChange);
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
            if (weapons.Count < 1)
            {
                return;
            }

            weaponIndex = Math.Clamp(newIndex, 0, weapons.Count - 1);
            CurrentWeapon = weapons[weaponIndex];
            WeaponIndexChanged?.Invoke(weaponIndex);
        }

        private void UpdateWeaponAmmo(Weapon weapon, int ammoChange)
        {
            var newAmmo = weaponsAmmo[weapon] += ammoChange;
            AmmoChanged?.Invoke(weapons.IndexOf(weapon), newAmmo);
        }

        private void UpdateActiveWeapon(Weapon weapon)
        {
            UpdateWeaponIndex(weapons.IndexOf(weapon));
        }

        #region Events

        public static event Action<int> WeaponIndexChanged;

        public static event Action<int, int> AmmoChanged;

        public static event Action<int, Weapon, int> WeaponAdded;

        #endregion
    }
}
