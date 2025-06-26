using System;
using UnityEngine;

namespace NeonBlack.Utilities
{
    public static class Settings
    {
        private const string FramerateKey = "Framerate";
        private const string MouseSensitivityKey = "MouseSensitivity";
        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string SfxVolumeKey = "SfxVolume";
        private const string LanguageKey = "Language";

        private static int _framerate;
        private static float _mouseSensitivity;
        private static float _masterVolume;
        private static float _musicVolume;
        private static float _sfxVolume;
        private static int _language;

        public static int Framerate
        {
            get => _framerate;
            set
            {
                _framerate = value;

                Application.targetFrameRate = _framerate;
                PlayerPrefs.SetInt(FramerateKey, _framerate);
                FramerateChanged?.Invoke(_framerate);
            }
        }

        public static float MouseSensitivity
        {
            get => _mouseSensitivity;
            set
            {
                _mouseSensitivity = value;

                PlayerPrefs.SetFloat(MouseSensitivityKey, _mouseSensitivity);
                MouseSensitivityChanged?.Invoke(_mouseSensitivity);
            }
        }

        public static float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = value;

                PlayerPrefs.SetFloat(MasterVolumeKey, _masterVolume);
                MasterVolumeChanged?.Invoke(_masterVolume);
            }
        }

        public static float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = value;

                PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolume);
                MusicVolumeChanged?.Invoke(_musicVolume);
            }
        }

        public static float SfxVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = value;

                PlayerPrefs.SetFloat(SfxVolumeKey, _sfxVolume);
                SfxVolumeChanged?.Invoke(_sfxVolume);
            }
        }

        public static int Language
        {
            get => _language;
            set
            {
                _language = value;

                PlayerPrefs.SetInt(LanguageKey, _language);
                LanguageChanged?.Invoke(_language);
            }
        }

        public static event Action<int> FramerateChanged;
        public static event Action<float> MouseSensitivityChanged;
        public static event Action<float> MasterVolumeChanged;
        public static event Action<float> MusicVolumeChanged;
        public static event Action<float> SfxVolumeChanged;
        public static event Action<int> LanguageChanged;

        public static void Save()
        {
            PlayerPrefs.Save();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameStart()
        {
            Framerate = PlayerPrefs.GetInt(FramerateKey, 60);
            MouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 1f);
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            Language = PlayerPrefs.GetInt(LanguageKey, 0);
        }
    }
}
