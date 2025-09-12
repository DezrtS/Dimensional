using Interfaces;
using Scriptables.Projectiles;
using Systems.Projectiles;
using UnityEngine;

namespace Systems.Entities.Behaviours
{
    public class LaunchProjectileBehaviourComponent : BehaviourComponent
    {
        [SerializeField] private BaseProjectileDatum projectileDatum;
        [SerializeField] private float launchSpeed;
        [SerializeField] private Vector3 launchDirection;
        
        protected override void OnActivate()
        {
            LaunchProjectile();
            Deactivate();
        }

        protected override void OnDeactivate()
        {
            
        }

        private void LaunchProjectile()
        {
            var projectile = projectileDatum.Spawn();
            var position = transform.position;
            var fireContext = FireContext.Construct(position, launchDirection, TargetValueType.Direction, launchSpeed, HitResponseType.Destroy, DestroyResponseType.DestroyGameObject);
            projectile.Fire(fireContext);
        }
    }
}
