using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using NeonBlack.Interfaces;
using NeonBlack.Systems.SceneManagement;
using UnityEngine;

namespace NeonBlack.Interactables
{
    public class PortalExit : MonoBehaviour, IPlayerInteractable
    {
        #region Serialized Fields

        [SerializeField]
        private SceneReference exitScene;

        [SerializeField]
        private GameObject visuals;

        [SerializeField]
        private MonoBehaviour[] activatables;

        #endregion

        private int activatedCounter;

        #region Event Functions

        private void OnEnable()
        {
            foreach (var activatable in activatables)
            {
                ((IActivatable)activatable).Activated += OnActivatableActivated;
            }
        }

        private void OnDisable()
        {
            foreach (var activatable in activatables)
            {
                ((IActivatable)activatable).Activated -= OnActivatableActivated;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (activatables == null)
            {
                return;
            }

            foreach (var activatable in activatables)
            {
                if (activatable is not IActivatable)
                {
                    Debug.LogError("Activatables must be of type IActivatable");
                }
            }
        }
#endif

        #endregion

        #region IPlayerInteractable Members

        public bool CanBeInteracted => activatedCounter >= activatables.Length;

        public void Interact()
        {
            SceneLoader.LoadScene(exitScene).Forget();
        }

        #endregion

        private void OnActivatableActivated()
        {
            activatedCounter++;

            if (CanBeInteracted)
            {
                visuals.SetActive(true);
            }
        }
    }
}
