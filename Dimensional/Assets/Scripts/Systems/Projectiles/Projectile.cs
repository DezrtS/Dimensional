using System;
using System.Collections;
using Interfaces;
using Scriptables.Projectiles;
//using Systems.Entities;
using Systems.Movement;
using UnityEngine;

namespace Systems.Projectiles
{
    public class Projectile : MonoBehaviour, IProjectile<Projectile>
    {
        public event IProjectile<Projectile>.ProjectileEventHandler Fired;
        public event IProjectile<Projectile>.ProjectileEventHandler Destroyed;
        public event IProjectile<Projectile>.HitEventHandler Hit;
        
        private FireContext _fireContext;
        private ForceController _forceController;
        
        public ProjectileDatum ProjectileDatum { get; private set; }
        public bool IsFired { get; private set; }

        private void Awake()
        {
            _forceController = GetComponent<ForceController>();
        }

        public void Initialize(ProjectileDatum projectileDatum)
        {
            ProjectileDatum = projectileDatum;
        }

        private bool CanFire()
        {
            return !IsFired & ProjectileDatum;
        }

        public void Fire(FireContext fireContext)
        {
            if (!CanFire()) return;
            IsFired = true;
            _fireContext = fireContext;
            OnFire(fireContext);
            Fired?.Invoke(this);
        }

        private void OnFire(FireContext fireContext)
        {
            StartCoroutine(LifetimeRoutine());
            _forceController.Teleport(fireContext.FirePosition);
            _forceController.ApplyForce(fireContext.GetDirection() * fireContext.FireSpeed, ForceMode.Impulse);
        }

        public void Destroy()
        {
            IsFired = false;
            Destroyed?.Invoke(this);
            OnDestroy();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            if (_fireContext.DestroyObjectOnDestroy) Destroy(gameObject);
        }

        private IEnumerator LifetimeRoutine()
        {
            yield return new WaitForSeconds(ProjectileDatum.LifetimeDuration);
            Destroy();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnHit(other);
        }

        private void OnHit(Collider other)
        {
            if (other.CompareTag("Player")) return;
            //if (!other.transform.TryGetComponent(out Health health)) return;
            //if (!health.CanDamage()) return;
            Hit?.Invoke(this, other);
            //health.Damage(ProjectileDatum.Damage);
            Destroy();
        }
    }
}
