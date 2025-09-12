using Scriptables.Projectiles;
using Scriptables.Projectiles.Behaviours;
using Systems.Entities;
using UnityEngine;

namespace Systems.Projectiles.Behaviours
{
    public class DamageProjectileBehaviour : BaseProjectileBehaviour
    {
        private DamageProjectileBehaviourDatum _damageProjectileBehaviourDatum;

        public override void Initialize(BaseProjectileBehaviourDatum projectileBehaviourDatum)
        {
            base.Initialize(projectileBehaviourDatum);
            _damageProjectileBehaviourDatum = (DamageProjectileBehaviourDatum)projectileBehaviourDatum;
        }

        protected override void ProjectileOnCollided(BaseProjectile projectile, Collider hitCollider)
        {
            //if ((_damageProjectileBehaviourDatum.DamageLayerMask.value & (1 << hitCollider.gameObject.layer)) == 0) return;
            if (hitCollider.TryGetComponent(out Health health))
            {
                health.Damage(_damageProjectileBehaviourDatum.Damage);
            }
        }
    }
}
