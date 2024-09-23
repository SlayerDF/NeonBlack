using UnityEngine;

public static class Settings
{
    public delegate void SettingChangedEventHandler(string settingKey);

    public const string FramerateKey = "Framerate";
    public const string MouseSensitivityKey = "MouseSensitivity";

    private static int _framerate;

    private static float _mouseSensitivity;

    public static int Framerate
    {
        get => _framerate;
        set
        {
            _framerate = value;

            Application.targetFrameRate = _framerate;
            PlayerPrefs.SetInt(FramerateKey, _framerate);
            SettingChanged?.Invoke(FramerateKey);
        }
    }

    public static float MouseSensitivity
    {
        get => _mouseSensitivity;
        set
        {
            _mouseSensitivity = value;

            PlayerPrefs.SetFloat(MouseSensitivityKey, _mouseSensitivity);
            SettingChanged?.Invoke(MouseSensitivityKey);
        }
    }

    public static event SettingChangedEventHandler SettingChanged;

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnGameStart()
    {
        Framerate = PlayerPrefs.GetInt(FramerateKey, 60);
        MouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 1f);
    }
}
