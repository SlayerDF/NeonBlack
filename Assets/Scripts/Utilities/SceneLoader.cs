using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private const int ConstantSceneBuildIndex = 0;

    public static async UniTask LoadScene(int sceneToLoadBuildIndex)
    {
        var constantScene = SceneManager.GetSceneByBuildIndex(ConstantSceneBuildIndex);

        // Just load scene in normal mode if there is no constant scene
        if (!constantScene.isLoaded)
        {
            SceneManager.LoadScene(sceneToLoadBuildIndex);
            return;
        }

        // Switch active scene to the constant one
        if (SceneManager.GetActiveScene().buildIndex != ConstantSceneBuildIndex)
        {
            SwitchActiveScene(constantScene);
        }

        var operations = new List<AsyncOperation>();

        // Unload all scenes except the constant one
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var sc = SceneManager.GetSceneAt(i);

            if (sc.buildIndex == ConstantSceneBuildIndex)
            {
                continue;
            }

            operations.Add(SceneManager.UnloadSceneAsync(sc));
        }

        // Load requested scene
        if (sceneToLoadBuildIndex != ConstantSceneBuildIndex)
        {
            operations.Add(SceneManager.LoadSceneAsync(sceneToLoadBuildIndex, LoadSceneMode.Additive));
        }

        Time.timeScale = 0f;

        // Wait tasks to finish
        await UniTask.WhenAll(operations.Select(op => op.ToUniTask()));

        // Switch active scene
        if (sceneToLoadBuildIndex != ConstantSceneBuildIndex)
        {
            SwitchActiveScene(SceneManager.GetSceneByBuildIndex(sceneToLoadBuildIndex));
        }

        // Imitate loading
        await UniTask.WaitForSeconds(1f, true);

        Time.timeScale = 1f;
    }

    public static async UniTask RestartLevel()
    {
        // Temporary hack
        // TODO: move loader to another scene to simplify things
        //
        var mainMenuController = Object.FindObjectOfType<MainMenuController>();
        var menuManager = Object.FindObjectOfType<MenuManager>(true);

        mainMenuController?.Pause();
        menuManager?.SwitchToMenu(MenuManager.MenuType.Loader);

        await LoadScene(SceneManager.GetActiveScene().buildIndex);

        menuManager?.SwitchToMenu(MenuManager.MenuType.Main);
        mainMenuController?.Unpause();
    }

    private static void SwitchActiveScene(Scene scene)
    {
        SceneManager.SetActiveScene(scene);
        DisableInactiveScenesCameras();
    }

    private static void DisableInactiveScenesCameras()
    {
        var scene = SceneManager.GetActiveScene();
        var cameras = Object.FindObjectsOfType<Camera>(true);

        for (var i = 0; i < cameras.Length; ++i)
        {
            cameras[i].gameObject.SetActive(cameras[i].gameObject.scene == scene);
        }
    }
}
