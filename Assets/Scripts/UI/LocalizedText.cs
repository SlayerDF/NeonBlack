using NeonBlack.Systems.LocalizationManager;
using TMPro;
using UnityEngine;

namespace NeonBlack.UI
{
    public class LocalizedText : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private string localizationId;

        [SerializeField]
        private TMP_Text textElement;

        #endregion

        #region Event Functions

        private void OnEnable()
        {
            LocalizationManager.LanguageChanged += Translate;
            Translate();
        }

        private void OnDisable()
        {
            LocalizationManager.LanguageChanged -= Translate;
        }

        #endregion

        private void Translate()
        {
            textElement.text = LocalizationManager.GetTranslation(localizationId);
        }
    }
}
