using NeonBlack.Entities.Player;
using TMPro;
using UnityEngine;

namespace NeonBlack.UI
{
    public class InteractHintText : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private TMP_Text text;

        #endregion

        #region Event Functions

        private void OnEnable()
        {
            PlayerInput.InteractionStateChanged += OnInteractionStateChanged;
        }

        private void OnDisable()
        {
            PlayerInput.InteractionStateChanged -= OnInteractionStateChanged;
        }

        #endregion

        private void OnInteractionStateChanged(bool state)
        {
            text.enabled = state;
        }
    }
}
