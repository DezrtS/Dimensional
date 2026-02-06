using Managers;
using Systems.Entities;
using Systems.Entities.Behaviours;
using UnityEngine;

namespace Systems
{
    public class Spike : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private LayerMask damageLayerMask;

        private void OnCollisionEnter(Collision other)
        {
            var hitObject = other.rigidbody ? other.rigidbody.gameObject : other.gameObject;
            if (!GameManager.CheckLayerMask(damageLayerMask, hitObject)) return;
            
            if (hitObject.TryGetComponent(out Health health))
            {
                health.Damage(damage);
            }
            
            if (!hitObject.TryGetComponent(out StunBehaviourComponent stunBehaviour)) return;
            var direction = hitObject.transform.position - transform.position;
            stunBehaviour.Stun(direction);
        }
    }
}
