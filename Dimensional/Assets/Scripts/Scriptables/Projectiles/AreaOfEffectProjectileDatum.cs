using Systems.Projectiles;
using UnityEngine;

namespace Scriptables.Projectiles
{
    [CreateAssetMenu(fileName = "AreaOfEffectProjectileDatum", menuName = "Scriptable Objects/Projectiles/AreaOfEffectProjectileDatum")]
    public class AreaOfEffectProjectileDatum : BaseProjectileDatum
    {
        [SerializeField] private float radius;
        [SerializeField] private LayerMask aoeLayerMask;
        [SerializeField] private bool triggerOnExpire;
        
        public float Radius => radius;
        public LayerMask AoeLayerMask => aoeLayerMask;
        public bool TriggerOnExpire => triggerOnExpire;
        
        public override BaseProjectile Spawn()
        {
            var projectileObject = Instantiate(ProjectilePrefab);
            var projectile = projectileObject.GetComponent<AreaOfEffectProjectile>();
            projectile.Initialize(this);
            AttachProjectileBehaviours(projectile, projectileObject);
            return projectile;
        }
    }
}
