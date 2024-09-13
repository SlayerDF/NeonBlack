using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class SceneLoader
{
    private const string BootstrapSceneName = "Bootstrapper";

    private static Scene _bootstrapScene = SceneManager.GetSceneByName(BootstrapSceneName);
    private static GameObject _bootstrapSceneRoot;
    private static CancellationToken _ct;

    public static async UniTask LoadSceneGroup(SceneGroup sceneGroup, bool reloadActiveScene = true)
    {
        ShowLoader();
        Time.timeScale = 0f;

        var sceneCount = SceneManager.sceneCount;
        var scenesToLoad = sceneGroup.Scenes.Select(x => x.Name).ToList();
        var activeSceneName = sceneGroup.FindSceneNameByType(SceneGroup.SceneType.ActiveScene);
        var tasks = new List<UniTask>();

        for (var i = 0; i < sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);

            if (!scene.isLoaded)
            {
                continue;
            }

            switch (scenesToLoad.Contains(scene.name))
            {
                // Unload active scene if flag is active
                case true when scene.name == activeSceneName && reloadActiveScene:
                // Unload any scene that is not in the list except the bootstrap
                case false when scene.name != BootstrapSceneName:
                    tasks.Add(SceneManager.UnloadSceneAsync(scene).ToUniTask(cancellationToken: _ct));
                    break;
                // Remove already loaded scenes from the list
                case true:
                    scenesToLoad.Remove(scene.name);
                    break;
            }
        }

        tasks.AddRange(Enumerable.Select(scenesToLoad,
            t => SceneManager.LoadSceneAsync(t, LoadSceneMode.Additive).ToUniTask(cancellationToken: _ct)));

        await UniTask.WhenAll(tasks);

        var activeScene = SceneManager.GetSceneByName(activeSceneName);
        if (activeScene.IsValid())
        {
            SceneManager.SetActiveScene(activeScene);
            DisableInactiveAudioListeners(activeScene);
        }

        // Imitate loading
        await UniTask.WaitForSeconds(1f, true, PlayerLoopTiming.EarlyUpdate, _ct);

        Time.timeScale = 1f;
        HideLoader();
    }

    public static async UniTask RestartLevel()
    {
        ShowLoader();

        Time.timeScale = 0f;

        var scene = SceneManager.GetActiveScene();
        var sceneName = scene.name;

        await UniTask.WhenAll(
            SceneManager.UnloadSceneAsync(scene).ToUniTask(cancellationToken: _ct),
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask(cancellationToken: _ct));

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        // Imitate loading
        await UniTask.WaitForSeconds(1f, true, cancellationToken: _ct);

        Time.timeScale = 1f;

        HideLoader();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void OnGameStart()
    {
        PreloadBoostrapScene().Forget();
    }

    private static async UniTaskVoid PreloadBoostrapScene()
    {
        if (!_bootstrapScene.IsValid())
        {
            await SceneManager.LoadSceneAsync(BootstrapSceneName, LoadSceneMode.Additive).ToUniTask();
            _bootstrapScene = SceneManager.GetSceneByName(BootstrapSceneName);
        }

        if (_bootstrapSceneRoot)
        {
            return;
        }

        _bootstrapSceneRoot = _bootstrapScene.GetRootGameObjects()[0];
        _ct = _bootstrapSceneRoot.GetCancellationTokenOnDestroy();

        if (SceneManager.GetActiveScene() == _bootstrapScene)
        {
            var sceneGroup = _bootstrapSceneRoot.GetComponent<Bootstrapper>().DefaultSceneGroup;

            await LoadSceneGroup(sceneGroup);
        }
        else
        {
            _bootstrapSceneRoot.SetActive(false);
        }
    }

    private static void ShowLoader()
    {
        _bootstrapSceneRoot.SetActive(true);
    }

    private static void HideLoader()
    {
        _bootstrapSceneRoot.SetActive(false);
    }

    private static void DisableInactiveAudioListeners(Scene activeScene)
    {
        var listeners = Object.FindObjectsOfType<AudioListener>();

        for (var i = 0; i < listeners.Length; ++i)
        {
            listeners[i].gameObject.SetActive(listeners[i].gameObject.scene == activeScene);
        }
    }
}
