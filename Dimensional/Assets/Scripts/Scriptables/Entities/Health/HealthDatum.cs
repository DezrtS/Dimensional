using UnityEngine;

namespace Scriptables.Entities
{
    [CreateAssetMenu(fileName = "HeathDatum", menuName = "Scriptable Objects/Health/HeathDatum")]
    public class HealthDatum : ScriptableObject
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private float hitInvincibilityDuration;
        [SerializeField] private float reviveInvincibilityDuration;
        
        public int MaxHealth => maxHealth;
        public float HitInvincibilityDuration => hitInvincibilityDuration;
        public float ReviveInvincibilityDuration => reviveInvincibilityDuration;
    }
}
