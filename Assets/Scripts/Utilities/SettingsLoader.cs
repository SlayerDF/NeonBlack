using UnityEngine;

public static class SettingsLoader
{
    [RuntimeInitializeOnLoadMethod]
    private static void OnGameStart()
    {
        LoadSettings();
    }

    private static void LoadSettings()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 60);
    }
}
