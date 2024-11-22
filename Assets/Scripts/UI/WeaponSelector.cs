using System.Collections.Generic;
using NeonBlack.Entities.Player;
using NeonBlack.Systems.LevelState;
using NeonBlack.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack.UI
{
    public class WeaponSelector : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private HorizontalLayoutGroup weaponsRoot;

        [SerializeField]
        private GameObject selectionFrame;

        [SerializeField]
        private CollectibleIcon collectibleIconPrefab;

        #endregion

        private readonly List<CollectibleIcon> collectibleIcons = new();

        private float selectionFrameOffset;

        #region Event Functions

        private void Awake()
        {
            selectionFrameOffset = weaponsRoot.spacing + collectibleIconPrefab.GetComponent<RectTransform>().rect.width;
        }

        private void OnEnable()
        {
            LevelState.LevelStarted += OnLevelStarted;
            PlayerInventory.WeaponAdded += OnWeaponAdded;
            PlayerInventory.AmmoChanged += OnAmmoChanged;
            PlayerInventory.WeaponIndexChanged += OnWeaponIndexChanged;
        }

        private void OnDisable()
        {
            LevelState.LevelStarted -= OnLevelStarted;
            PlayerInventory.WeaponAdded -= OnWeaponAdded;
            PlayerInventory.AmmoChanged -= OnAmmoChanged;
            PlayerInventory.WeaponIndexChanged -= OnWeaponIndexChanged;
        }

        #endregion

        private void OnLevelStarted()
        {
            selectionFrame.SetActive(false);

            foreach (var icon in collectibleIcons)
            {
                Destroy(icon.gameObject);
            }

            collectibleIcons.Clear();
        }

        private void OnWeaponAdded(int index, Weapon weapon, int ammo)
        {
            var collectibleIcon = Instantiate(collectibleIconPrefab, weaponsRoot.transform);
            collectibleIcon.Icon = weapon.Icon;
            collectibleIcon.IconAlpha = weapon.IconAlpha;
            collectibleIcon.Counter = ammo;
            collectibleIcons.Add(collectibleIcon);

            if (index != collectibleIcons.Count - 1)
            {
                Debug.LogWarning("GUI weapon index is got out of sync with the player inventory");
            }
        }

        private void OnAmmoChanged(int index, int ammo)
        {
            collectibleIcons[index].Counter = ammo;
        }

        private void OnWeaponIndexChanged(int index)
        {
            if (!selectionFrame.activeInHierarchy)
            {
                selectionFrame.SetActive(true);
            }

            selectionFrame.transform.localPosition = new Vector3(selectionFrameOffset * index, 0f, 0f);
        }
    }
}
