using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static async UniTask LoadScene(Scene constantScene, SceneReference sceneToLoad)
    {
        var operations = new List<AsyncOperation>();

        // Switch active scene to the constant one
        if (SceneManager.GetActiveScene().buildIndex != constantScene.buildIndex)
        {
            SwitchActiveScene(constantScene);
        }

        // Unload all scenes except the constant one
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var sc = SceneManager.GetSceneAt(i);

            if (sc.buildIndex == constantScene.buildIndex)
            {
                continue;
            }

            operations.Add(SceneManager.UnloadSceneAsync(sc));
        }

        // Load requested scene
        if (sceneToLoad.BuildIndex != constantScene.buildIndex)
        {
            operations.Add(SceneManager.LoadSceneAsync(sceneToLoad.BuildIndex, LoadSceneMode.Additive));
        }

        Time.timeScale = 0f;

        // Wait tasks to finish
        await UniTask.WhenAll(operations.Select(op => op.ToUniTask()));

        // Switch active scene
        if (sceneToLoad.BuildIndex != constantScene.buildIndex)
        {
            SwitchActiveScene(sceneToLoad.LoadedScene);
        }

        // Imitate loading
        await UniTask.WaitForSeconds(1f, true);

        Time.timeScale = 1f;
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
