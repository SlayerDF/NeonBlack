using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;

public class ButtonSceneLoader : ButtonBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private MainMenuController mainMenuController;

    [SerializeField]
    private MenuManager menuManager;

    [SerializeField]
    private SceneReference scene;

    #endregion

    /// <inheritdoc />
    protected override void OnClick()
    {
        ProcessSceneLoadingAsync().Forget();
    }

    private async UniTaskVoid ProcessSceneLoadingAsync()
    {
        menuManager.SwitchToMenu(MenuManager.MenuType.Loader);

        await SceneLoader.LoadScene(scene.BuildIndex);

        mainMenuController.Unpause();

        menuManager.SwitchToMenu(MenuManager.MenuType.Main);
    }
}
