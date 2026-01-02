using Scriptables.Actions;
using Systems.Projectiles;
using UnityEngine;
using Utilities;

namespace Systems.Actions
{
    public class ProjectileAction : Action
    {
        private ProjectileActionDatum _projectileActionDatum;
        private ObjectPool<BaseProjectile> _projectilePool;

        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            _projectileActionDatum = (ProjectileActionDatum)actionDatum;
            
            if (!_projectileActionDatum.ProjectileDatum) return;
            _projectilePool = new ObjectPool<BaseProjectile>(_projectileActionDatum.ProjectilePoolDatum,
                _projectileActionDatum.ProjectileDatum, null);
        }

        protected override void OnTrigger(ActionContext context)
        {
            base.OnTrigger(context);
            ProjectileAttack(context);
            Deactivate(context);
        }

        protected override void OnEntityChanged(ActionContext context) {}

        private void ProjectileAttack(ActionContext context)
        {
            var projectile = _projectilePool.GetObject();
            projectile.Fire(FireContext.Construct(transform.position, context.TargetDirection,
                TargetValueType.Direction, context.ProjectileSpeed, HitResponseType.Destroy, DestroyResponseType.ReturnToPool));
        }
    }
}
