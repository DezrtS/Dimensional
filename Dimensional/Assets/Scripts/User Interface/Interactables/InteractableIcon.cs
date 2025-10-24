using Scriptables.Interactables;
using Scriptables.User_Interface;
using UnityEngine;
using UnityEngine.UI;

namespace User_Interface.Interactables
{
    public class InteractableIcon : WorldUIAnchor
    {
        [SerializeField] private Image image;
        
        private InteractableIconDatum _interactableIconDatum;

        protected override void OnInitialize(WorldUIAnchorDatum worldUIAnchorDatum, Transform worldTransform)
        {
            _interactableIconDatum = (InteractableIconDatum)worldUIAnchorDatum;
            image.sprite = _interactableIconDatum.KeyboardIcon;
        }

        protected override void OnSetTargetTransform(Transform targetTransform)
        {
            if (!targetTransform) transform.localScale = Vector3.zero;
        }
    }
}
