using System;
using UnityEngine;

namespace Systems.Entities.Behaviours
{
    public abstract class BehaviourComponent : MonoBehaviour
    {
        public event Action Deactivated;
        public bool IsActive { get; private set; }

        protected virtual bool CanActivate()
        {
            return !IsActive;   
        }

        [ContextMenu("Activate")]
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
