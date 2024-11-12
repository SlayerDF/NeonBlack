using NeonBlack.Weapons;
using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Weapon[] weapons;

        #endregion


        internal Weapon[] Weapons => weapons;

        // TODO: add weapon selection
        internal Weapon CurrentWeapon => weapons[0];
    }
}
