using Scriptables.Projectiles;
using UnityEngine;

namespace Interfaces
{
    public interface IProjectile<T> where T : MonoBehaviour 
    {
        public delegate void ProjectileEventHandler(IProjectile<T> projectile);
        public delegate void HitEventHandler(IProjectile<T> projectile, Collider collider);
        
        public ProjectileDatum ProjectileDatum { get; }
        public bool IsFired { get; }
        
        public void Initialize(ProjectileDatum projectileDatum);
        public void Fire(FireContext fireContext);
        public void Destroy();
    }
    
    public struct FireContext
    {
        public Vector3 FirePosition;
        public Vector3 TargetPosition;

        public float FireSpeed;
        
        public bool DestroyObjectOnDestroy;

        public static FireContext Construct(Vector3 firePosition, Vector3 targetPosition, float fireSpeed, bool destroyObjectOnDestroy)
        {
            return new FireContext()
            {
                FirePosition = firePosition,
                TargetPosition = targetPosition,
                FireSpeed = fireSpeed,
                DestroyObjectOnDestroy = destroyObjectOnDestroy
            };
        }
        
        public Vector3 GetDirection() => (TargetPosition - FirePosition).normalized;
    }
}