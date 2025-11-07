using System;
using Interfaces;
using Managers;
using Scriptables.Dialogue;
using Scriptables.Interactables;
using Scriptables.User_Interface;
using UnityEngine;
using User_Interface;
using User_Interface.Dialogue;

namespace Systems.NPCs
{
    public class Npc : MonoBehaviour, IInteractable
    {
        [SerializeField] private InteractableDatum interactableDatum;
        [SerializeField] private bool isDisabled;
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        private WorldUIAnchor _worldUIAnchor;
        
        public bool IsDisabled => isDisabled;
        public GameObject GameObject => gameObject;
        public InteractableDatum InteractableDatum => interactableDatum;

        private void Start()
        {
            _worldUIAnchor = UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, transform);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!GameManager.CheckLayerMask(InteractableDatum.InteractableLayerMask, other.gameObject)) return;
            _worldUIAnchor.SetTargetTransform(other.transform);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (_worldUIAnchor.TargetTransform != other.transform) return;
            _worldUIAnchor.SetTargetTransform(null);
        }

        public void Interact(InteractContext interactContext)
        {
            
        }
        
        public void Hover()
        {
            
        }

        public void StopHovering()
        {
            
        }

        public void View(InteractContext interactContext, bool show)
        {
            _worldUIAnchor.SetTargetTransform(show ? interactContext.SourceGameObject.transform : null);
        }
    }
}