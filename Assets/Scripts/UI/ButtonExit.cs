using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonExit : ButtonBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private MenuManager menuManager;

    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private string mainMenuText = "Exit";

    [SerializeField]
    private string inGameText = "Main Menu";

    #endregion Serialized Fields

    private Scene activeScene;

    #region Event Functions

    /// <inheritdoc />
    protected override void OnEnable()
    {
        base.OnEnable();

        activeScene = SceneManager.GetActiveScene();
        UpdateText();

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    /// <inheritdoc />
    protected override void OnDisable()
    {
        base.OnDisable();
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    #endregion Event Functions

    private void OnActiveSceneChanged(Scene _, Scene newScene)
    {
        activeScene = newScene;

        UpdateText();
    }

    private void UpdateText()
    {
        text.text = activeScene == gameObject.scene ? mainMenuText : inGameText;
    }

    /// <inheritdoc />
    protected override void OnClick()
    {
        ProcessExitAsync().Forget();
    }

    private async UniTaskVoid ProcessExitAsync()
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
            menuManager.SwitchToMenu(MenuManager.MenuType.Loader);

            await SceneLoader.LoadScene(gameObject.scene, SceneReference.FromScenePath(gameObject.scene.path));

            menuManager.SwitchToMenu(MenuManager.MenuType.Main);
        }
    }
}