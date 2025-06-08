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
    }

    public struct InteractContext
    {
        public GameObject SourceGameObject;

        public InteractContext Construct(GameObject sourceGameObject)
        {
            return new InteractContext()
            {
                SourceGameObject = sourceGameObject
            };
        }
    }
}
