using System.Threading;
using Cysharp.Threading.Tasks;
using NeonBlack.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Slider framerateSlider;

        [SerializeField]
        private TMP_Text framerateText;

        [SerializeField]
        private Slider mouseSensitivitySlider;

        [SerializeField]
        private TMP_Text mouseSensitivityText;

        #endregion

        private CancellationTokenSource saveTaskCts;

        #region Event Functions

        private void Start()
        {
            framerateSlider.value = Settings.Framerate;
            mouseSensitivitySlider.value = Settings.MouseSensitivity;
        }

        private void OnEnable()
        {
            framerateSlider.onValueChanged.AddListener(UpdateFramerate);
            mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivity);
        }

        private void OnDisable()
        {
            framerateSlider.onValueChanged.RemoveListener(UpdateFramerate);
            mouseSensitivitySlider.onValueChanged.RemoveListener(UpdateMouseSensitivity);

            Settings.Save();
        }

        #endregion

        private void UpdateFramerate(float value)
        {
            var intValue = (int)value;

            Settings.Framerate = intValue;
            framerateText.text = intValue.ToString();

            SaveSettings().Forget();
        }

        private void UpdateMouseSensitivity(float value)
        {
            Settings.MouseSensitivity = value;
            mouseSensitivityText.text = value.ToString("F2");

            SaveSettings().Forget();
        }

        private async UniTask SaveSettings()
        {
            // Debounce saving
            saveTaskCts?.Cancel();
            saveTaskCts = new CancellationTokenSource();

            await UniTask.WaitForSeconds(1f, cancellationToken: saveTaskCts.Token);

            PlayerPrefs.SetInt("FPS", Application.targetFrameRate);
            PlayerPrefs.Save();
            Debug.Log("Settings saved");
        }
    }
}
