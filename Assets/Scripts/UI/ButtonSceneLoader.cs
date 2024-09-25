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

    #endregion

    /// <inheritdoc />
    protected override void OnClick()
    {
        SceneLoader.LoadScene(scene).Forget();

        mainMenuController.Unpause();
    }
}
