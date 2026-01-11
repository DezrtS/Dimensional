using System;
using Scriptables.Projectiles;
using Scriptables.Projectiles.Behaviours;
using Systems.Forces;
using Systems.Movement;
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

        protected override void ProjectileOnCollided(BaseProjectile projectile, Collider hitCollider)
        {
            if (!hitCollider.TryGetComponent(out ForceController forceController)) return;
            switch (_knockbackProjectileBehaviourDatum.KnockbackType)
            {
                case KnockbackType.Directional:
                    forceController.ApplyForce(projectile.ForceController.GetVelocity().normalized * _knockbackProjectileBehaviourDatum.KnockbackPower, ForceMode.Impulse);       
                    break;
                case KnockbackType.Radial:
                    forceController.ApplyForce((hitCollider.transform.position - projectile.transform.position).normalized * _knockbackProjectileBehaviourDatum.KnockbackPower, ForceMode.Impulse);
                    break;
                case KnockbackType.None:
                default:
                    break;
            }
        }
    }
}
