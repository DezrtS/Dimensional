using System;
using Interfaces;
using Scriptables.Interactables;
using UnityEngine;

namespace Systems.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        public delegate void InteractableEventHandler(Interactable interactable, InteractContext interactContext);
        public event InteractableEventHandler Hovered;
        public event InteractableEventHandler UnHovered;
        public event InteractableEventHandler Interacted;
        
        [SerializeField] private InteractableDatum interactableDatum;
        
        protected InteractableDatum InteractableDatum => interactableDatum;
        public bool IsHovered { get; protected set; }
        protected InteractContext PreviousInteractContext { get; private set; }
        
        public abstract bool CanInteract(InteractContext interactContext);

        public virtual void Interact(InteractContext interactContext)
        {
            Interacted?.Invoke(this, interactContext);
            PreviousInteractContext = interactContext;
        }

        public virtual void Hover(InteractContext interactContext)
        {
            if (IsHovered) return;
            IsHovered = true;
            Hovered?.Invoke(this, interactContext);
            PreviousInteractContext = interactContext;
        }

        public virtual void UnHover(InteractContext interactContext)
        {
            if (!IsHovered) return;
            IsHovered = false;
            UnHovered?.Invoke(this, interactContext);
            PreviousInteractContext = interactContext;
        }
    }
}
