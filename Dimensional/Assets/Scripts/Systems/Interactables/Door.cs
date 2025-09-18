using System;
using System.Collections;
using Interfaces;
using Managers;
using Systems.Movement;
using UnityEngine;

namespace Systems.Interactables
{
    public class Door : Interactable
    {
        private static readonly int Show = Animator.StringToHash("Show");

        public delegate void DoorEventHandler(InteractContext interactContext, string from, string to);
        public static event DoorEventHandler Opened;
        
        [SerializeField] private Transform spawnTransform;
        [SerializeField] private string id;
        [Space]
        [SerializeField] private bool destinationIsScene;
        [SerializeField] private string destinationId;

        private Animator _animator;

        private void Awake()
        {
            Opened += DoorOnOpened;
            _animator = GetComponent<Animator>();
        }

        private void OnDisable()
        {
            Opened -= DoorOnOpened;
        }

        private void DoorOnOpened(InteractContext interactContext, string from, string to)
        {
            if (id != to) return;
            if (InteractableCamera) CameraManager.Instance.Transition(null, InteractableCamera, 0);
            interactContext.SourceGameObject.GetComponent<ForceController>().Teleport(spawnTransform.position);
            if (InteractableCamera) CameraManager.Instance.TransitionToDefault(true, InteractableDatum.ExitTransitionDuration);
        }
        
        public bool CanInteract(InteractContext interactContext)
        {
            return !IsDisabled;
        }

        protected override void OnInteraction(InteractContext context)
        {
            HandleInteraction(context);
            if (destinationId == string.Empty) return;
            
            if (destinationIsScene)
            {
                SceneManager.Instance.LoadSceneWithTransition(destinationId);
            }
            else
            {
                Opened?.Invoke(context, id, destinationId);   
            }
        }
    }
}
