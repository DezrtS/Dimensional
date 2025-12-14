using System;
using Interfaces;
using Managers;
using Scriptables.Events;
using Scriptables.User_Interface;
using Systems.Interactables;
using UnityEngine;

namespace Systems.NPCs
{
    public class InteractableNpc : Interactable
    {
        [SerializeField] private Transform elementTransform;
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private EventDatum[] eventData;

        private void Start()
        {
            UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
        }

        public override bool CanInteract(InteractContext interactContext)
        {
            return true;
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            EventManager.SendEvents(eventData);
        }
    }
}