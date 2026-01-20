using Scriptables.Projectiles;
using Scriptables.Projectiles.Behaviours;
using UnityEngine;

namespace Systems.Projectiles.Behaviours
{
    public abstract class BaseProjectileBehaviour : MonoBehaviour
    {
        private BaseProjectileBehaviourDatum _projectileBehaviourDatum;
        protected BaseProjectile Projectile { get; private set; }

        public virtual void Initialize(BaseProjectileBehaviourDatum projectileBehaviourDatum)
        {
            _projectileBehaviourDatum = projectileBehaviourDatum;
        }
        
        public void SetProjectile(BaseProjectile projectile)
        {
            if (Projectile)
            {
                Projectile.Fired -= ProjectileOnFired;
                Projectile.Expired -= ProjectileOnExpired;
                Projectile.Destroyed -= ProjectileOnDestroyed;
                Projectile.Collided -= ProjectileOnCollided;
            }
            
            Projectile = projectile;
            Projectile.Fired += ProjectileOnFired;
            Projectile.Expired += ProjectileOnExpired;
            Projectile.Destroyed += ProjectileOnDestroyed;
            Projectile.Collided += ProjectileOnCollided;
        }

        protected virtual void ProjectileOnFired(BaseProjectile projectile) {}
        protected virtual void ProjectileOnExpired(BaseProjectile projectile) {}
        protected virtual void ProjectileOnDestroyed(BaseProjectile projectile) {}
        protected virtual void ProjectileOnCollided(BaseProjectile projectile, GameObject hitObject) {}
    }
}
