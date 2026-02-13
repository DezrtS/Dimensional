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

        protected override void ProjectileOnCollided(BaseProjectile projectile, GameObject hitObject)
        {
            if (hitObject.TryGetComponent(out Health health))
            {
                health.Damage(_damageProjectileBehaviourDatum.Damage);
            }
        }
    }
}
