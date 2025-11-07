using System;
using Interfaces;
using Managers;
using Scriptables.Interactables;
using Scriptables.User_Interface;
using Unity.Cinemachine;
using UnityEngine;
using User_Interface;

namespace Systems.Interactables
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public delegate void InteractionEventHandler(Interactable interactable, InteractContext context);
        public event InteractionEventHandler Interacted;
        
        [SerializeField] private InteractableDatum interactableDatum;
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private CinemachineCamera interactableCamera;

        private Transform _targetTransform;
        private bool _isInRange;
        
        private bool _isInteracting;
        private float _interactionTimer;
        private WorldUIAnchor _worldUIAnchor;
        
        public bool IsDisabled { get; set; }
        public GameObject GameObject => gameObject;
        public InteractableDatum InteractableDatum => interactableDatum;
        protected CinemachineCamera InteractableCamera => interactableCamera;
        protected InteractContext PreviousInteractContext { get; private set; }

        private void Start()
        {
            if (!worldUIAnchorDatum) return;
            _worldUIAnchor = UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, transform);
        }

        private void FixedUpdate()
        {
            if (IsDisabled) return;

            if (_targetTransform) _isInRange = Vector3.Distance(_targetTransform.position, transform.position) < InteractableDatum.InteractableDistance;
            
            if (!_isInteracting) return;
            if (_interactionTimer <= 0) return;
            _interactionTimer -= Time.fixedDeltaTime;
            if (_interactionTimer > 0) return;
            OnInteraction(PreviousInteractContext);
        }

        public void Interact(InteractContext context)
        {
            if (!_isInRange || _isInteracting || IsDisabled) return;

            PreviousInteractContext = context;
            _isInteracting = true;
            _interactionTimer = interactableDatum.InteractionDelay;
            AudioManager.PlayOneShot(interactableDatum.InteractSound, transform.position);
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

        private void OnTriggerEnter(Collider other)
        {
            if (!GameManager.CheckLayerMask(InteractableDatum.InteractableLayerMask, other.gameObject)) return;
            _targetTransform = other.transform;
            _worldUIAnchor.SetTargetTransform(_targetTransform);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (_targetTransform != other.transform) return;
            _targetTransform = null;
            _worldUIAnchor.SetTargetTransform(null);
        }

        public void Hover()
        {
            
        }

        public void StopHovering()
        {
            
        }

        public void View(InteractContext interactContext, bool show)
        {
            
        }
    }
}
