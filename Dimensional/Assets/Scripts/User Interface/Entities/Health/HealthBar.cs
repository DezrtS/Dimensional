using Systems.Entities;
using UnityEngine;

namespace User_Interface.Entities
{
    public abstract class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;

        private void OnEnable()
        {
            if (!health) return;
            health.HealthChanged += HealthOnHealthChanged;
            health.HealthStateChanged += HealthOnHealthStateChanged;
        }
        
        private void OnDisable()
        {
            if (!health) return;
            health.HealthChanged -= HealthOnHealthChanged;
            health.HealthStateChanged -= HealthOnHealthStateChanged;
        }

        public void AttachHealthBar(Health healthToAttach)
        {
            health = healthToAttach;
            health.HealthChanged += HealthOnHealthChanged;
            health.HealthStateChanged += HealthOnHealthStateChanged;
        }

        public void DetachHealthBar()
        {
            if (!health) return;
            health.HealthChanged -= HealthOnHealthChanged;
            health.HealthStateChanged -= HealthOnHealthStateChanged;
            health = null;
        }

        protected abstract void HealthOnHealthChanged(int oldValue, int newValue, int maxValue);
        protected abstract void HealthOnHealthStateChanged(Health health, bool isDead);
    }
}
