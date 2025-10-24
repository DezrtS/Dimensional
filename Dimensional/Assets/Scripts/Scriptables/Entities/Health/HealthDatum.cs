using FMODUnity;
using UnityEngine;

namespace Scriptables.Entities
{
    [CreateAssetMenu(fileName = "HeathDatum", menuName = "Scriptable Objects/Health/HeathDatum")]
    public class HealthDatum : ScriptableObject
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private float hitInvincibilityDuration;
        [SerializeField] private float reviveInvincibilityDuration;

        [SerializeField] private EventReference hurtSound;
        [SerializeField] private EventReference deathSound;
        
        public int MaxHealth => maxHealth;
        public float HitInvincibilityDuration => hitInvincibilityDuration;
        public float ReviveInvincibilityDuration => reviveInvincibilityDuration;
        
        public EventReference HurtSound => hurtSound;
        public EventReference DeathSound => deathSound;
    }
}
