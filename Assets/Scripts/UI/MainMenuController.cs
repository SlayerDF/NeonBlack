using Cysharp.Threading.Tasks;
using NeonBlack.Systems.LevelState;
using NeonBlack.Systems.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace NeonBlack.UI
{
    public class MainMenuController : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private RectTransform mainMenuRoot;

        [SerializeField]
        private EndingUI endingUI;

        #endregion Serialized Fields

        private bool paused;
        private InputActions.UIActions uiActions;

        #region Event Functions

        private void Awake()
        {
            uiActions = new InputActions().UI;
            LevelState.LevelEnded += OnLevelEnded;
        }

        private void OnLevelEnded(bool value)
        {
            Pause();
            mainMenuRoot.gameObject.SetActive(false);

            if (value)
            {
                endingUI.ShowWinScreen(LevelState.Score);
            }
            else
            {
                endingUI.ShowLoseScreen();
            }
        }

        private void Start()
        {
            // Main menu was loaded as additional scene
            if (SceneManager.GetActiveScene() != gameObject.scene)
            {
                Unpause();
            }
            else
            {
                Time.timeScale = 1f;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                AudioListener.pause = false;
            }
        }

        private void OnEnable()
        {
            uiActions.Enable();
            uiActions.Pause.performed += OnPause;
        }

        private void OnDisable()
        {
            uiActions.Disable();
            uiActions.Pause.performed -= OnPause;
        }

        #endregion Event Functions

        private void OnPause(InputAction.CallbackContext _)
        {
            if (SceneManager.GetActiveScene() == gameObject.scene ||
                endingUI.gameObject.activeSelf)
            {
                return;
            }

            if (paused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            Time.timeScale = 0f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            AudioListener.pause = true;

            mainMenuRoot.gameObject.SetActive(true);

            paused = true;
        }

        public void Unpause()
        {
            Time.timeScale = 1f;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            AudioListener.pause = false;

            mainMenuRoot.gameObject.SetActive(false);

            paused = false;
        }

        private void OnDestroy()
        {
            LevelState.LevelEnded -= OnLevelEnded;
        }
    }
}
