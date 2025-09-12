using System;
using Interfaces;
using Scriptables.Interactables;
using UnityEngine;

namespace Systems.Interactables
{
    public class BoomerangTarget : MonoBehaviour, IInteractable
    {
        [SerializeField] private InteractableDatum interactableDatum;
        [SerializeField] private float rechargeDuration;

        private float _timer;
        
        public bool IsDisabled { get; private set; }
        public InteractableDatum InteractableDatum => interactableDatum;

        private void FixedUpdate()
        {
            if (!(_timer > 0)) return;
            
            _timer -= Time.fixedDeltaTime;
            if (_timer <= 0)
            {
                IsDisabled = false;
            }
        }

        public bool CanInteract(InteractContext interactContext)
        {
            return !IsDisabled;
        }
        
        public void Interact(InteractContext interactContext)
        {
            _timer = rechargeDuration;
            IsDisabled = true;
        }

        public void View(bool show)
        {
            throw new NotImplementedException();
        }
    }
}