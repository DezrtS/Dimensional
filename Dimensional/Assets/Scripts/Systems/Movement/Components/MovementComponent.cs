using System;
using UnityEngine;

namespace Systems.Movement.Components
{
    public abstract class MovementComponent : MonoBehaviour
    {
        public event Action Deactivated;
        
        public bool IsActive { get; private set; }
        protected MovementController MovementController { get; private set; }

        public virtual void Initialize(MovementController movementController)
        {
            MovementController = movementController;
        }

        protected virtual bool CanActivate()
        {
            return !IsActive;   
        }

        public void Activate()
        {
            if (!CanActivate()) return;
            IsActive = true;
            OnActivate();
        }
        
        protected abstract void OnActivate();

        public void Deactivate()
        {
            if (!IsActive) return;
            Deactivated?.Invoke();
            IsActive = false;
            OnDeactivate();
        }
        
        protected abstract void OnDeactivate();
    }
}
