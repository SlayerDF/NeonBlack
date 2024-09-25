using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private RectTransform mainMenuRoot;

    #endregion

    private bool paused;
    private InputActions.UIActions uiActions;

    #region Event Functions

    private void Awake()
    {
        uiActions = new InputActions().UI;
    }

    private void Start()
    {
        // Main menu was loaded as additional scene
        if (SceneManager.GetActiveScene() != gameObject.scene)
        {
            Unpause();
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

    #endregion

    private void OnPause(InputAction.CallbackContext _)
    {
        if (SceneManager.GetActiveScene() == gameObject.scene)
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
        mainMenuRoot.gameObject.SetActive(true);
        paused = true;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mainMenuRoot.gameObject.SetActive(false);
        paused = false;
    }
}
