using Systems.Entities;
using Systems.Entities.Behaviours;
using UnityEngine;

namespace Systems.Triggers
{
    public class HurtTrigger : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;
        [SerializeField] private int damage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Health health))
            {
                health.Damage(damage);
            }
            
            if (!other.TryGetComponent(out StunBehaviourComponent stunBehaviour)) return;
            stunBehaviour.Stun(direction);
        }
    }
}
