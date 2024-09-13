﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class SceneLoader
{
    private const string BootstrapSceneName = "Bootstrapper";

    private static Scene _bootstrapScene = SceneManager.GetSceneByName(BootstrapSceneName);
    private static GameObject _bootstrapSceneRoot;
    private static CancellationToken _ct;

    private static readonly Dictionary<string, List<string>> AdditionalScenes = new();

    public static async UniTask LoadScene(SceneReference mainScene, bool reloadMainScene = true)
    {
        ShowLoader();
        Time.timeScale = 0f;

        var tasks = new List<UniTask>();

        var scenesToLoad = new List<string> { mainScene.Name };

        if (AdditionalScenes.TryGetValue(mainScene.Name, out var additionalScenes))
        {
            scenesToLoad.AddRange(additionalScenes);
        }

        var sceneCount = SceneManager.sceneCount;
        for (var i = 0; i < sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);

            if (!scene.isLoaded)
            {
                continue;
            }

            switch (scenesToLoad.Contains(scene.name))
            {
                // Unload main scene if flag is active
                case true when scene.name == mainScene.Name && reloadMainScene:
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

        if (mainScene.LoadedScene.IsValid())
        {
            SceneManager.SetActiveScene(mainScene.LoadedScene);
            DisableInactiveAudioListeners(mainScene.LoadedScene);
        }

        // Imitate loading
        await UniTask.WaitForSeconds(1f, true, PlayerLoopTiming.EarlyUpdate, _ct);

        Time.timeScale = 1f;
        HideLoader();
    }

    public static async UniTask RestartLevel()
    {
        var activeScene = SceneManager.GetActiveScene();

        await LoadScene(SceneReference.FromScenePath(activeScene.path));
    }

    [RuntimeInitializeOnLoadMethod]
    private static void OnGameStart()
    {
        LoadBoostrapScene().Forget();
    }

    private static async UniTaskVoid LoadBoostrapScene()
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

        var bootstrapper = _bootstrapSceneRoot.GetComponent<Bootstrapper>();

        for (var i = 0; i < bootstrapper.SceneGroups.Length; i++)
        {
            var group = bootstrapper.SceneGroups[i];
            AdditionalScenes.Add(group.MainScene.Name, group.AdditionalScenes.Select(x => x.Name).ToList());
        }

        var activeScene = SceneManager.GetActiveScene();

        if (activeScene == _bootstrapScene)
        {
            await LoadScene(bootstrapper.DefaultScene);
        }
        else
        {
            await LoadScene(SceneReference.FromScenePath(activeScene.path), false);
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