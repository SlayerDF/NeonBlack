using NeonBlack.Systems.LocalizationManager;
using NeonBlack.Utilities;
using TMPro;
using UnityEngine;

namespace NeonBlack.UI
{
    public class LanguageDropdown : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private TMP_Dropdown languageDropdown;

        [SerializeField]
        private string[] languages;

        #endregion

        #region Event Functions

        private void Start()
        {
            languageDropdown.value = Settings.Language;
        }

        private void OnEnable()
        {
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }

        private void OnDisable()
        {
            languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
        }

        #endregion

        private void OnLanguageChanged(int index)
        {
            Settings.Language = index;
            LocalizationManager.ChangeLanguage(languages[index]);
        }
    }
}
