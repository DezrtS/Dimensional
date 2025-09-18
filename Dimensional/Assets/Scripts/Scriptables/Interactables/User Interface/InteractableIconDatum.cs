using UnityEngine;
using User_Interface.Interactables;

namespace Scriptables.Interactables
{
    [CreateAssetMenu(fileName = "InteractableIconDatum", menuName = "Scriptable Objects/Interactables/InteractableIconDatum")]
    public class InteractableIconDatum : ScriptableObject
    {
        [SerializeField] private GameObject interactableIconPrefab;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Sprite icon;
        [SerializeField] private float minRange;
        [SerializeField] private float maxRange;
        [SerializeField] private float minDot;
        [SerializeField] private float maxDot;
        [SerializeField] private AnimationCurve sizeCurve;
        
        public Vector3 Offset => offset;
        public Sprite Icon => icon;
        public float MinRange => minRange;
        public float MaxRange => maxRange;
        public float MinDot => minDot;
        public float MaxDot => maxDot;
        public AnimationCurve SizeCurve => sizeCurve;

        public InteractableIcon Spawn(Transform parent, Transform interactableTransform)
        {
            var interactableIconObject = Instantiate(interactableIconPrefab, parent);
            var interactableIcon = interactableIconObject.GetComponent<InteractableIcon>();
            interactableIcon.Initialize(this, interactableTransform);
            return interactableIcon;
        }
    }
}
