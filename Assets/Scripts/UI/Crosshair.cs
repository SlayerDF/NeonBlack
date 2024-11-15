using NeonBlack.Entities.Player;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack.UI
{
    public class Crosshair : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Image crosshair;

        #endregion

        #region Event Functions

        private void OnEnable()
        {
            PlayerInput.AimStateChanged += OnAimStateChanged;
        }

        private void OnDisable()
        {
            PlayerInput.AimStateChanged -= OnAimStateChanged;
        }

        #endregion

        private void OnAimStateChanged(bool state)
        {
            crosshair.enabled = state;
        }
    }
}
