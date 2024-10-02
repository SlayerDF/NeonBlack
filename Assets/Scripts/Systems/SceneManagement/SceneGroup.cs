using System;
using Eflatun.SceneReference;
using UnityEngine;

namespace NeonBlack.Systems.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        #region Serialized Fields

        [SerializeField]
        private SceneReference mainScene;

        [SerializeField]
        private SceneReference[] additionalScenes;

        #endregion

        public SceneReference MainScene => mainScene;

        public SceneReference[] AdditionalScenes => additionalScenes;
    }
}
