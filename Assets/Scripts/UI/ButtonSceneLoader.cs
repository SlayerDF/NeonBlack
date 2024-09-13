using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;

public class ButtonSceneLoader : ButtonBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private MainMenuController mainMenuController;

    [SerializeField]
    private SceneReference scene;

    [SerializeField]
    private SceneGroup sceneGroup;

    #endregion

    /// <inheritdoc />
    protected override void OnClick()
    {
        SceneLoader.LoadSceneGroup(sceneGroup).Forget();

        mainMenuController.Unpause();
    }
}
