using Eflatun.SceneReference;
using UnityEngine;

namespace NeonBlack.Systems.SceneManagement
{
    public class Bootstrapper : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private SceneGroup[] sceneGroups;

        [SerializeField]
        private SceneReference defaultScene;

        [SerializeField]
        private AudioClip defaultSceneMusicClip;

        #endregion

        public SceneGroup[] SceneGroups => sceneGroups;

        public SceneReference DefaultScene => defaultScene;
        public AudioClip DefaultSceneMusicClip => defaultSceneMusicClip;
    }
}
