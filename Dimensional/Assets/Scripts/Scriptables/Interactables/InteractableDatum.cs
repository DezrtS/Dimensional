using UnityEngine;

namespace Scriptables.Interactables
{
    [CreateAssetMenu(fileName = "InteractableDatum", menuName = "Scriptable Objects/Interactables/InteractableDatum")]
    public class InteractableDatum : ScriptableObject
    {
        [SerializeField] private string interactableTitle;
        [SerializeField] private float interactableDistance;
        [SerializeField] private LayerMask interactableLayerMask;
        
        [SerializeField] private float interactionDelay;
        [SerializeField] private float enterTransitionDuration;
        [SerializeField] private float exitTransitionDuration;
        
        public string InteractableTitle => interactableTitle;
        public float InteractableDistance => interactableDistance;
        public LayerMask InteractableLayerMask => interactableLayerMask;
        
        public float InteractionDelay => interactionDelay;
        public float EnterTransitionDuration => enterTransitionDuration;
        public float ExitTransitionDuration => exitTransitionDuration;
    }
}
