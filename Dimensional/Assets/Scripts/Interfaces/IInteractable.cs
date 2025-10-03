using Scriptables.Interactables;
using UnityEngine;

namespace Interfaces
{
    public interface IInteractable
    {
        public delegate void DatumChangedEventHandler(InteractableDatum oldValue, InteractableDatum newValue, IInteractable interactable);
        public delegate void InteractableStateEventHandler(IInteractable interactable, bool isDisabled);
        
        public bool IsDisabled { get; }
        public InteractableDatum InteractableDatum { get; }
        public void Interact(InteractContext interactContext);
        public void View(InteractContext interactContext, bool show);
    }

    public struct InteractContext
    {
        public GameObject SourceGameObject;

        public static InteractContext Construct(GameObject sourceGameObject)
        {
            return new InteractContext()
            {
                SourceGameObject = sourceGameObject
            };
        }
    }
}
