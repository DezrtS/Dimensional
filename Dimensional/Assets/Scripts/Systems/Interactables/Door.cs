using System;
using System.Collections;
using Interfaces;
using Managers;
using Scriptables.Events;
using Scriptables.User_Interface;
using Systems.Cameras;
using Systems.Movement;
using UnityEngine;

namespace Systems.Interactables
{
    public class Door : Interactable
    {
        public delegate void DoorEventHandler(InteractContext interactContext, string from, string to);
        public static event DoorEventHandler Opened;
        
        [Space]
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private Transform elementTransform;
        [SerializeField] private EventDatum[] eventData;
        [Space]
        [SerializeField] private Transform spawnTransform;
        [SerializeField] private string id;
        [Space]
        [SerializeField] private bool destinationIsScene;
        [SerializeField] private string destinationId;

        private CameraTransition _cameraTransition;

        private void Awake()
        {
            Opened += DoorOnOpened;
            if (!TryGetComponent(out _cameraTransition)) return;
            _cameraTransition.TransitionToFinished += CameraTransitionOnTransitionToFinished;
            _cameraTransition.TransitionFromFinished += CameraTransitionOnTransitionFromFinished;
        }

        private void Start()
        {
            UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
        }

        private void CameraTransitionOnTransitionToFinished()
        {
            HandleInteraction();
        }
        
        private void CameraTransitionOnTransitionFromFinished()
        {
            
        }

        private void OnDisable()
        {
            Opened -= DoorOnOpened;
        }

        private void DoorOnOpened(InteractContext interactContext, string from, string to)
        {
            if (id != to) return;
            interactContext.SourceGameObject.GetComponent<ForceController>().Teleport(spawnTransform.position);
            if (_cameraTransition) _cameraTransition.TransitionFrom();
        }
        
        public override bool CanInteract(InteractContext interactContext)
        {
            return true;
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            if (_cameraTransition) _cameraTransition.TransitionTo();
            else HandleInteraction();
        }

        private void HandleInteraction()
        {
            EventManager.SendEvents(eventData);
            if (destinationId == string.Empty) return;
            
            if (destinationIsScene)
            {
                SceneManager.Instance.LoadSceneWithTransition(destinationId);
            }
            else
            {
                Opened?.Invoke(PreviousInteractContext, id, destinationId);   
            }
        }
    }
}
