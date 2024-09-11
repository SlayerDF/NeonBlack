using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    #region MenuType enum

    public enum MenuType
    {
        Main,
        Controls,
        Credits,
        Loader
    }

    #endregion

    #region Serialized Fields

    [SerializeField]
    private MenuItem[] menus;

    #endregion

    public void SwitchToMenu(MenuType type)
    {
        for (var i = 0; i < menus.Length; i++)
        {
            menus[i].transform.gameObject.SetActive(menus[i].type == type);
        }
    }

    #region Nested type: ${0}

    [Serializable]
    public struct MenuItem
    {
        public MenuType type;
        public RectTransform transform;
    }

    #endregion
}
