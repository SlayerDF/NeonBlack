using UnityEngine;

public class ButtonMenuSwitcher : ButtonBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private MenuManager menuManager;

    [SerializeField]
    private MenuManager.MenuType menuType;

    #endregion Serialized Fields

    /// <inheritdoc />
    protected override void OnClick()
    {
        menuManager.SwitchToMenu(menuType);
    }
}