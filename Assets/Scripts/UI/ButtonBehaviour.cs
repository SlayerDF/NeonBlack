using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ButtonBehaviour : MonoBehaviour
{
    private Button button;

    #region Event Functions

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }

    protected virtual void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    protected virtual void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    #endregion

    protected abstract void OnClick();
}
