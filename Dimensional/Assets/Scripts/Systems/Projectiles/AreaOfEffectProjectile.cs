using Scriptables.Projectiles;
using UnityEngine;

namespace Systems.Projectiles
{
    public class AreaOfEffectProjectile : BaseProjectile
    {
        private AreaOfEffectProjectileDatum _areaOfEffectProjectileDatum;
        
        public override void Initialize(BaseProjectileDatum projectileDatum)
        {
            base.Initialize(projectileDatum);
            _areaOfEffectProjectileDatum = (AreaOfEffectProjectileDatum)projectileDatum;
        }

        protected override void OnExpire()
        {
            if (_areaOfEffectProjectileDatum.TriggerOnExpire) TriggerAreaOfEffect();
            base.OnExpire();
        }

        protected override void OnCollide(Collider hitCollider)
        {
            TriggerAreaOfEffect();
            base.OnCollide(hitCollider);
        }

        private void TriggerAreaOfEffect()
        {
            var position = transform.position;
            var results = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(position, _areaOfEffectProjectileDatum.Radius, results,
                _areaOfEffectProjectileDatum.AoeLayerMask);
            for (var i = 0; i < size; i++)
            {
                HandleCollision(results[i]);
            }
        }
    }
}
