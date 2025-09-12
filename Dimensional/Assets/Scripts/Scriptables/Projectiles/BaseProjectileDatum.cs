using Interfaces;
using Scriptables.Projectiles.Behaviours;
using Systems.Projectiles;
using UnityEngine;

namespace Scriptables.Projectiles
{
    [CreateAssetMenu(fileName = "BaseProjectileDatum", menuName = "Scriptable Objects/Projectiles/BaseProjectileDatum")]
    public class BaseProjectileDatum : ScriptableObject, ISpawnPoolableObjects<BaseProjectile>
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float lifetimeDuration;
        [SerializeField] private LayerMask projectileLayerMask;
        [SerializeField] private BaseProjectileBehaviourDatum[] projectileBehaviourData;
        
        protected GameObject ProjectilePrefab => projectilePrefab;
        public float LifetimeDuration => lifetimeDuration;
        public LayerMask ProjectileLayerMask => projectileLayerMask;
        public BaseProjectileBehaviourDatum[] ProjectileBehaviourData => projectileBehaviourData;

        protected void AttachProjectileBehaviours(BaseProjectile projectile, GameObject projectileBehaviourHolder)
        {
            foreach (var projectileBehaviourDatum in projectileBehaviourData)
            {
                var projectileBehaviour = projectileBehaviourDatum.AttachProjectileBehaviour(projectileBehaviourHolder);
                projectileBehaviour.SetProjectile(projectile);
            }
        }
        
        public virtual BaseProjectile Spawn()
        {
            var projectileObject = Instantiate(projectilePrefab);
            var projectile = projectileObject.GetComponent<BaseProjectile>();
            projectile.Initialize(this);
            AttachProjectileBehaviours(projectile, projectileObject);
            return projectile;
        }
    }
}
