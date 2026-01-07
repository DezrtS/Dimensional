using UnityEngine;

namespace Interfaces
{
    public interface IInteractable
    {
		public bool IsInteractable { get; }

        public void Interact(InteractContext interactContext);
        public void Hover(InteractContext interactContext);
        public void UnHover(InteractContext interactContext);
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
