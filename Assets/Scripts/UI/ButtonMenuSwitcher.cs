using UnityEngine;

namespace NeonBlack.UI
{
    public class ButtonMenuSwitcher : ButtonBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private MenuManager menuManager;

        [SerializeField]
        private MenuManager.MenuType menuType;

        #endregion

        /// <inheritdoc />
        protected override void OnClick()
        {
            menuManager.SwitchToMenu(menuType);
        }
    }
}
