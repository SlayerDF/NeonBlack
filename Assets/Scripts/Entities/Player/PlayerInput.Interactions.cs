using System;
using System.Collections.Generic;
using NeonBlack.Interfaces;
using NeonBlack.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeonBlack.Entities.Player
{
    public partial class PlayerInput
    {
        #region Serialized Fields

        [Header("Interactions")]
        [SerializeField]
        private SubscribableCollider interactionCollider;

        #endregion

        private readonly List<IPlayerInteractable> interactables = new();

        public static event Action<bool> InteractionStateChanged;

        private void OnEnableInteractions()
        {
            interactionCollider.TriggerEnter += OnInteractionTriggerEnter;
            interactionCollider.TriggerExit += OnInteractionTriggerExit;
        }

        private void OnDisableInteractions()
        {
            interactionCollider.TriggerEnter -= OnInteractionTriggerEnter;
            interactionCollider.TriggerExit -= OnInteractionTriggerExit;
        }

        private void OnInteractionTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IPlayerInteractable interactable) || !interactable.CanBeInteracted)
            {
                return;
            }

            interactables.Add(interactable);
            InteractionStateChanged?.Invoke(true);
        }

        private void OnInteractionTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IPlayerInteractable interactable))
            {
                return;
            }

            if (interactables.Remove(interactable))
            {
                InteractionStateChanged?.Invoke(interactables.Count > 0);
            }
        }


        private void OnInteract(InputAction.CallbackContext context)
        {
            var count = interactables.Count;

            if (count < 1)
            {
                return;
            }

            var interactable = interactables[count - 1];
            interactable.Interact();

            if (!interactable.CanBeInteracted && interactables.Remove(interactable))
            {
                InteractionStateChanged?.Invoke(interactables.Count > 0);
            }
        }
    }
}
