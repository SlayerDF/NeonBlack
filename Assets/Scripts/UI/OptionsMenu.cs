using System.Threading;
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
        private Slider masterVolumeSlider;

        [SerializeField]
        private TMP_Text masterVolumeText;

        [SerializeField]
        private Slider musicVolumeSlider;

        [SerializeField]
        private TMP_Text musicVolumeText;

        [SerializeField]
        private Slider sfxVolumeSlider;

        [SerializeField]
        private TMP_Text sfxVolumeText;

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
            masterVolumeSlider.value = Settings.MasterVolume;
            musicVolumeSlider.value = Settings.MusicVolume;
            sfxVolumeSlider.value = Settings.SfxVolume;
        }

        private void OnEnable()
        {
            framerateSlider.onValueChanged.AddListener(UpdateFramerate);
            mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivity);
            masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(UpdateSfxVolume);
        }

        private void OnDisable()
        {
            framerateSlider.onValueChanged.RemoveListener(UpdateFramerate);
            mouseSensitivitySlider.onValueChanged.RemoveListener(UpdateMouseSensitivity);
            masterVolumeSlider.onValueChanged.RemoveListener(UpdateMasterVolume);
            musicVolumeSlider.onValueChanged.RemoveListener(UpdateMusicVolume);
            sfxVolumeSlider.onValueChanged.RemoveListener(UpdateSfxVolume);

            Settings.Save();
        }

        #endregion

        private void UpdateFramerate(float value)
        {
            var intValue = (int)value;

            Settings.Framerate = intValue;
            framerateText.text = intValue.ToString();
        }

        private void UpdateMouseSensitivity(float value)
        {
            Settings.MouseSensitivity = value;
            mouseSensitivityText.text = value.ToString("F2");
        }

        private void UpdateMasterVolume(float value)
        {
            Settings.MasterVolume = value;
            masterVolumeText.text = value.ToString("F2");
        }

        private void UpdateMusicVolume(float value)
        {
            Settings.MusicVolume = value;
            musicVolumeText.text = value.ToString("F2");
        }

        private void UpdateSfxVolume(float value)
        {
            Settings.SfxVolume = value;
            sfxVolumeText.text = value.ToString("F2");
        }
    }
}
