using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using NeonBlack.Systems.LocalizationManager;
using NeonBlack.Systems.SceneManagement;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeonBlack.UI
{
    public class ButtonExit : ButtonBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private TMP_Text text;

        [SerializeField]
        private string mainMenuTextId = "btn_exit";

        [SerializeField]
        private string inGameTextId = "btn_main_menu";

        [SerializeField]
        private SceneReference mainMenuScene;

        #endregion

        private Scene activeScene;

        #region Event Functions

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            LocalizationManager.LanguageChanged += UpdateText;

            activeScene = SceneManager.GetActiveScene();
            UpdateText();

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            LocalizationManager.LanguageChanged -= UpdateText;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        #endregion

        private void OnActiveSceneChanged(Scene _, Scene newScene)
        {
            activeScene = newScene;

            UpdateText();
        }

        private void UpdateText()
        {
            var translatedText = activeScene == gameObject.scene
                ? LocalizationManager.GetTranslation(mainMenuTextId)
                : LocalizationManager.GetTranslation(inGameTextId);

            text.text = translatedText;
        }

        /// <inheritdoc />
        protected override void OnClick()
        {
            if (activeScene == gameObject.scene)
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            }
            else
            {
                SceneLoader.LoadScene(mainMenuScene).Forget();
            }
        }
    }
}
