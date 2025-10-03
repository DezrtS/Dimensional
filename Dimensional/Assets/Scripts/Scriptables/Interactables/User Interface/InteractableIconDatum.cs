using Scriptables.User_Interface;
using UnityEngine;
using User_Interface;
using User_Interface.Interactables;

namespace Scriptables.Interactables
{
    [CreateAssetMenu(fileName = "InteractableIconDatum", menuName = "Scriptable Objects/User Interface/InteractableIconDatum")]
    public class InteractableIconDatum : WorldUIAnchorDatum
    {
        [SerializeField] private Sprite keyboardIcon;
        [SerializeField] private Sprite gamepadIcon;
        
        public Sprite KeyboardIcon => keyboardIcon;
        public Sprite GamepadIcon => gamepadIcon;

        public override WorldUIAnchor SpawnWorldUIAnchor(Transform parent, Transform worldTransform)
        {
            var interactableIconObject = Instantiate(Prefab, parent);
            var interactableIcon = interactableIconObject.GetComponent<InteractableIcon>();
            interactableIcon.Initialize(this, worldTransform);
            return interactableIcon;
        }
    }
}
