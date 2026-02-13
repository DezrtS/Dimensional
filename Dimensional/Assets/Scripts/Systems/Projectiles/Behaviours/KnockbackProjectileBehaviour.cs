using System;
using Scriptables.Projectiles.Behaviours;
using Systems.Entities.Behaviours;
using Systems.Forces;
using UnityEngine;

namespace Systems.Projectiles.Behaviours
{
    public enum KnockbackType
    {
        None,
        Directional,
        Radial,
    }
    
    public class KnockbackProjectileBehaviour : BaseProjectileBehaviour
    {
        private KnockbackProjectileBehaviourDatum _knockbackProjectileBehaviourDatum;

        public override void Initialize(BaseProjectileBehaviourDatum projectileBehaviourDatum)
        {
            base.Initialize(projectileBehaviourDatum);
            _knockbackProjectileBehaviourDatum = (KnockbackProjectileBehaviourDatum)projectileBehaviourDatum;
        }

        protected override void ProjectileOnCollided(BaseProjectile projectile, GameObject hitObject)
        {
            if (!hitObject.TryGetComponent(out StunBehaviourComponent stunBehaviour)) return;

            switch (_knockbackProjectileBehaviourDatum.KnockbackType)
            {
                case KnockbackType.Directional:
                    var velocity = projectile.ForceController.GetVelocity();
                    velocity.y = 0;
                    stunBehaviour.Stun(velocity.normalized);
                    break;
                case KnockbackType.Radial:
                    var direction = (hitObject.transform.position - transform.position);
                    direction.y = 0;
                    stunBehaviour.Stun(direction.normalized);
                    break;
                case KnockbackType.None:
                default:
                    break;
            }            
        }
    }
}
