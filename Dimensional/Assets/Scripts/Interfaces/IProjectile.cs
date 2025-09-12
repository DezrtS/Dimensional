using Scriptables.Projectiles;
using Systems.Projectiles;
using UnityEngine;

namespace Interfaces
{
    public interface IProjectile 
    {
        public BaseProjectileDatum ProjectileDatum { get; }
        public bool IsFired { get; }
        
        public void Initialize(BaseProjectileDatum projectileDatum);
        public void Fire(FireContext fireContext);
        public void Destroy();
    }
}