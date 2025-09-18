using System;
using Interfaces;
using Managers;
using Scriptables.Interactables;
using Unity.Cinemachine;
using UnityEngine;

namespace Systems.Interactables
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public delegate void InteractionEventHandler(Interactable interactable, InteractContext context);
        public event InteractionEventHandler Interacted;
        
        [SerializeField] private InteractableDatum interactableDatum;
        [SerializeField] private InteractableIconDatum interactableIconDatum;
        [SerializeField] private CinemachineCamera interactableCamera;

        private bool _isInteracting;
        private float _interactionTimer;
        
        public bool IsDisabled { get; set; }
        public InteractableDatum InteractableDatum => interactableDatum;
        protected CinemachineCamera InteractableCamera => interactableCamera;
        protected InteractContext PreviousInteractContext { get; private set; }

        private void Start()
        {
            if (interactableIconDatum) UIManager.Instance.SpawnInteractableIcon(interactableIconDatum, transform);
        }

        private void FixedUpdate()
        {
            if (!_isInteracting || IsDisabled) return;

            if (_interactionTimer <= 0) return;
            _interactionTimer -= Time.fixedDeltaTime;
            if (_interactionTimer > 0) return;
            OnInteraction(PreviousInteractContext);
        }

        public void Interact(InteractContext context)
        {
            if (_isInteracting || IsDisabled) return;

            PreviousInteractContext = context;
            _isInteracting = true;
            _interactionTimer = interactableDatum.InteractionDelay;
            if (_interactionTimer > 0)
            {
                if (interactableCamera) CameraManager.Instance.Transition(null, interactableCamera, interactableDatum.EnterTransitionDuration);
            }
            else
            {
                OnInteraction(context);   
            }
        }

        protected virtual void OnInteraction(InteractContext context)
        {
            HandleInteraction(context);
            if (interactableCamera) CameraManager.Instance.TransitionToDefault(false, interactableDatum.ExitTransitionDuration);
        }

        protected void HandleInteraction(InteractContext context)
        {
            Interacted?.Invoke(this, context);
            _isInteracting = false;
        }

        public void View(bool show)
        {
            
        }
    }
}
