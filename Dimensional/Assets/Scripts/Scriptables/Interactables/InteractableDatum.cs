using UnityEngine;

namespace Scriptables.Interactables
{
    [CreateAssetMenu(fileName = "InteractableDatum", menuName = "Scriptable Objects/Interactables/InteractableDatum")]
    public class InteractableDatum : ScriptableObject
    {
        [SerializeField] private string interactableTitle;
        
        public string InteractableTitle => interactableTitle;
    }
}
