using System;
using Debugging.New_Movement_System;
using Interfaces;
using Managers;
using Scriptables.Projectiles;
using Systems.Movement;
using UnityEngine;

namespace Systems.Projectiles
{
    public enum TargetValueType
    {
        None,
        Direction,
        Position
    }

    public enum HitResponseType
    {
        None,
        Destroy
    }

    public enum DestroyResponseType
    {
        None,
        ReturnToPool,
        DestroyGameObject,
    }
    
    public struct FireContext
    {
        public Vector3 FirePosition;
        public float FireSpeed;
        
        public HitResponseType HitResponseType;
        public DestroyResponseType DestroyResponseType;
        
        private Vector3 _targetValue;
        private TargetValueType _targetValueType;

        public static FireContext Construct(Vector3 firePosition, Vector3 targetValue, TargetValueType targetValueType, float fireSpeed, HitResponseType hitResponseType, DestroyResponseType destroyResponseType)
        {
            return new FireContext()
            {
                FirePosition = firePosition,
                FireSpeed = fireSpeed,
                
                HitResponseType = hitResponseType,
                DestroyResponseType = destroyResponseType,
                
                _targetValue = targetValue,
                _targetValueType = targetValueType,
            };
        }

        public Vector3 GetDirection()
        {
            switch (_targetValueType)
            {
                case TargetValueType.Direction:
                    return _targetValue;
                case TargetValueType.Position:
                    return (_targetValue - FirePosition).normalized;
                case TargetValueType.None:
                default:
                    return Vector3.forward;
            }
        }
    }
    
    public class BaseProjectile : MonoBehaviour, IObjectPoolable<BaseProjectile>
    {
        public delegate void ProjectileEventHandler(BaseProjectile projectile);
        public delegate void CollideEventHandler(BaseProjectile projectile, Collider hitCollider);
        public event ProjectileEventHandler Fired;
        public event ProjectileEventHandler Expired;
        public event ProjectileEventHandler Destroyed;
        public event CollideEventHandler Collided;
        public event Action<BaseProjectile> Returned;

        private float _lifetimeTimer;
        private FireContext _previousFireContext;
        
        public BaseProjectileDatum ProjectileDatum { get; private set; }
        public bool IsFired { get; private set; }
        public ForceController ForceController { get; private set; }

        private void Awake()
        {
            ForceController = GetComponent<ForceController>();
        }

        private void FixedUpdate()
        {
            if (!IsFired) return;
            var fixedDeltaTime = Time.fixedDeltaTime;
            UpdateTimers(fixedDeltaTime);
            OnFixedUpdate(fixedDeltaTime);
        }
        
        protected virtual void OnFixedUpdate(float fixedDeltaTime) { }

        private void UpdateTimers(float fixedDeltaTime)
        {
            if (_lifetimeTimer <= 0) return;
            
            _lifetimeTimer -= fixedDeltaTime;
            if (_lifetimeTimer > 0) return;
            Expire();
        }

        public virtual void Initialize(BaseProjectileDatum projectileDatum)
        {
            ProjectileDatum = projectileDatum;
        }

        public void Fire(FireContext fireContext)
        {
            if (IsFired) return;
            OnFire(fireContext);
        }

        protected virtual void OnFire(FireContext fireContext)
        {
            HandleFiring(fireContext);
            ForceController.Teleport(fireContext.FirePosition);
            ForceController.SetVelocity(fireContext.GetDirection() * fireContext.FireSpeed);
        }

        private void HandleFiring(FireContext fireContext)
        {
            IsFired = true;
            _lifetimeTimer = ProjectileDatum.LifetimeDuration;
            _previousFireContext = fireContext;
            Fired?.Invoke(this);
        }

        private void Expire()
        {
            if (!IsFired) return;
            OnExpire();
        }

        protected virtual void OnExpire()
        {
            HandleExpiring();
            Destroy();
        }

        private void HandleExpiring()
        {
            Expired?.Invoke(this);
        }

        public void Destroy()
        {
            OnDestroy();
        }

        protected virtual void OnDestroy()
        {
            HandleDestruction();
            ForceController.SetVelocity(Vector3.zero);
            switch (_previousFireContext.DestroyResponseType)
            {
                case DestroyResponseType.None:
                    break;
                case DestroyResponseType.ReturnToPool:
                    ReturnToPool();
                    break;
                case DestroyResponseType.DestroyGameObject:
                    Destroy(gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleDestruction()
        {
            IsFired = false;
            _lifetimeTimer = 0;
            Destroyed?.Invoke(this);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Collide(other);
        }

        private void Collide(Collider hitCollider)
        {
            if (!GameManager.CheckLayerMask(ProjectileDatum.ProjectileLayerMask, hitCollider.gameObject)) return;
            if (hitCollider.isTrigger) return;
            OnCollide(hitCollider);
        }

        protected virtual void OnCollide(Collider hitCollider)
        {
            HandleCollision(hitCollider);
            if (_previousFireContext.HitResponseType == HitResponseType.Destroy) Destroy();
        }

        protected void HandleCollision(Collider hitCollider)
        {
            Collided?.Invoke(this, hitCollider);
        }

        public void ReturnToPool()
        {
            Returned?.Invoke(this);
        }
    }
}