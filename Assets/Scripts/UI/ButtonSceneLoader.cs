using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using NeonBlack.Systems.SceneManagement;
using UnityEngine;

namespace NeonBlack.UI
{
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
            // Execution order is critical
            mainMenuController.Unpause();

            SceneLoader.LoadScene(scene).Forget();
        }
    }
}
