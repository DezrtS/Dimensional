using FMODUnity;
using Scriptables.Events;
using UnityEngine;

namespace Scriptables.Interactables
{
    [CreateAssetMenu(fileName = "InteractableDatum", menuName = "Scriptable Objects/Interactables/InteractableDatum")]
    public class InteractableDatum : ScriptableObject
    {
        [SerializeField] private string interactableTitle;
        [SerializeField] private float interactableDistance;
        [SerializeField] private LayerMask interactableLayerMask;

        [SerializeField] private EventReference interactSound;
        
        [SerializeField] private float interactionDelay;
        [SerializeField] private float enterTransitionDuration;
        [SerializeField] private float exitTransitionDuration;

        [SerializeField] private EventDatum[] eventData;
        
        public string InteractableTitle => interactableTitle;
        public float InteractableDistance => interactableDistance;
        public LayerMask InteractableLayerMask => interactableLayerMask;
        
        public EventReference InteractSound => interactSound;
        
        public float InteractionDelay => interactionDelay;
        public float EnterTransitionDuration => enterTransitionDuration;
        public float ExitTransitionDuration => exitTransitionDuration;
        
        public EventDatum[] EventData => eventData;
    }
}
