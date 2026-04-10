using Systems.Entities;
using Systems.Entities.Behaviours;
using UnityEngine;

namespace Systems.Triggers
{
    public class HurtTrigger : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;
        [SerializeField] private bool autoDirection;
        [SerializeField] private int damage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Health health))
            {
                health.Damage(damage);
            }
            
            if (!other.TryGetComponent(out StunBehaviourComponent stunBehaviour)) return;
            var dir = autoDirection ? (other.transform.position - transform.position).normalized : direction;
            stunBehaviour.Stun(dir);
        }
    }
}
