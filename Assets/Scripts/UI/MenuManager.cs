using System;
using NeonBlack.Systems.LevelState;
using UnityEngine;

namespace NeonBlack.UI
{
    public class MenuManager : MonoBehaviour
    {
        #region MenuType enum

        public enum MenuType
        {
            Main,
            Controls,
            Options,
            Credits,
            Levels
        }

        #endregion

        #region Serialized Fields

        [SerializeField]
        private MenuItem[] menus;

        #endregion

        #region Event Functions

        private void Awake()
        {
            LevelState.LevelStarted += OnLevelStarted;
        }

        private void OnDestroy()
        {
            LevelState.LevelStarted -= OnLevelStarted;
        }

        #endregion

        public void SwitchToMenu(MenuType type)
        {
            for (var i = 0; i < menus.Length; i++)
            {
                menus[i].transform.gameObject.SetActive(menus[i].type == type);
            }
        }

        private void OnLevelStarted()
        {
            SwitchToMenu(MenuType.Main);
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
}
