using FMODUnity;
using Scriptables.Events;
using UnityEngine;

namespace Scriptables.Interactables
{
    [CreateAssetMenu(fileName = "InteractableDatum", menuName = "Scriptable Objects/Interactables/InteractableDatum")]
    public class InteractableDatum : ScriptableObject
    {
        [SerializeField] private EventReference interactSound;
        [SerializeField] private EventDatum[] eventData;

        public EventReference InteractSound => interactSound;
        public EventDatum[] EventData => eventData;
    }
}
