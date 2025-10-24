using System.Collections;
using Managers;
using Scriptables.Entities;
using UnityEngine;

namespace Systems.Entities
{
    public class Health : MonoBehaviour
    {
        public delegate void HealthChangedEventHandler(int oldValue, int newValue, int maxValue);
        public event HealthChangedEventHandler HealthChanged;
        
        public delegate void HealthStateEventHandler(Health health, bool isDead);
        public event HealthStateEventHandler HealthStateChanged;
        
        [SerializeField] private HealthDatum healthDatum;
        [SerializeField] private bool destroyOnDeath;

        private bool _isDead;

        public bool IsInvincible { get; private set; }
        public int CurrentHealth { get; private set; }

        private void Awake()
        {
            CurrentHealth = healthDatum.MaxHealth;
        }

        [ContextMenu("Hurt")]
        private void Hurt() => Damage(1);

        public bool CanDamage()
        {
            return !IsInvincible && !_isDead;
        }

        public void Damage(int damage)
        {
            if (damage <= 0) return;
            if (!CanDamage()) Debug.LogWarning("Damaging Invincible Entity");
            AudioManager.PlayOneShot(healthDatum.HurtSound, gameObject);
            AddHealth(-damage);
            if (!_isDead) GiveInvincibility(healthDatum.HitInvincibilityDuration);
        }
        
        public void Heal(int heal) => AddHealth(heal);

        private void AddHealth(int amount)
        {
            SetHealth(CurrentHealth + amount);
            if (_isDead)
            {
                if (CurrentHealth > 0) Revive(false);
            }
            else
            {
                if (CurrentHealth <= 0) Die();
            }
        }

        public void SetHealth(int health)
        {
            health = Mathf.Clamp(health, 0, healthDatum.MaxHealth);
            HealthChanged?.Invoke(CurrentHealth, health, healthDatum.MaxHealth);
            CurrentHealth = health;
        }

        public void Die()
        {
            SetHealth(0);
            _isDead = true;
            AudioManager.PlayOneShot(healthDatum.DeathSound, gameObject);
            HealthStateChanged?.Invoke(this, _isDead);
            if (destroyOnDeath) Destroy(gameObject);
        }

        public void Revive(bool replenishHealth = true)
        {
            if (replenishHealth) SetHealth(healthDatum.MaxHealth);
            _isDead = false;
            HealthStateChanged?.Invoke(this, _isDead);
            GiveInvincibility(healthDatum.ReviveInvincibilityDuration);
        }

        public void GiveInvincibility(float duration)
        {
            IsInvincible = true;
            StartCoroutine(InvincibilityRoutine(duration));
        }

        private IEnumerator InvincibilityRoutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            IsInvincible = false;
        }
    }
}
