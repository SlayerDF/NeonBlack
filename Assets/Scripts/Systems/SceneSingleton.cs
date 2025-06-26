using UnityEngine;

namespace NeonBlack.Systems
{
    public abstract class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
    {
        protected static T Instance { get; private set; }

        public static bool Instantiated => Instance != null;

        #region Event Functions

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Multiple instances of singleton");
            }

            Instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #endregion
    }
}
