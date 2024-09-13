using System;
using System.Linq;
using Eflatun.SceneReference;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SceneGroup")]
public class SceneGroup : ScriptableObject
{
    #region SceneType enum

    public enum SceneType
    {
        ActiveScene,
        MainMenu,
        UI,
        HUD
    }

    #endregion

    #region Serialized Fields

    [SerializeField]
    private string groupName;

    [SerializeField]
    private SceneData[] scenes;

    #endregion

    public string GroupName => groupName;

    public SceneData[] Scenes => scenes;

    public string FindSceneNameByType(SceneType type)
    {
        return Scenes.FirstOrDefault(scene => scene.Type == type)?.Reference.Name;
    }

    #region Nested type: ${0}

    [Serializable]
    public class SceneData
    {
        #region Serialized Fields

        [SerializeField]
        private SceneReference reference;

        [SerializeField]
        private SceneType type;

        #endregion

        public SceneReference Reference => reference;

        public SceneType Type => type;

        public string Name => reference.Name;
    }

    #endregion
}
