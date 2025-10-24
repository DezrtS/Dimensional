using System;
using System.Collections.Generic;
using Managers;
using Scriptables.Actions;
using Scriptables.Actions.Movement;
using Systems.Entities;
using Systems.Entities.Behaviours;
using Systems.Projectiles;
using Systems.Projectiles.Behaviours;
using UnityEngine;
using Utilities;

namespace Systems.Actions.Movement
{
    public class AttackPositionMovementAction : PositionMovementAction
    {
        private AttackPositionMovementActionDatum _attackPositionMovementActionDatum;
        private List<Collider> _colliders;
        private ObjectPool<BaseProjectile> _projectilePool;

        private bool _isAttacking;

        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            _attackPositionMovementActionDatum = (AttackPositionMovementActionDatum)actionDatum;

            if (!_attackPositionMovementActionDatum.ProjectileDatum) return;
            _projectilePool = new ObjectPool<BaseProjectile>(_attackPositionMovementActionDatum.ProjectilePoolDatum,
                _attackPositionMovementActionDatum.ProjectileDatum, null);
        }
        
        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
            
            if (!_isAttacking) return;
            Attack();
        }

        protected override void OnActivation(ActionContext context)
        {
            _isAttacking = false;
            base.OnActivation(context);
            _colliders = new List<Collider>();
            if (_attackPositionMovementActionDatum.ProjectileEventType == ActionEventType.Activated) ProjectileAttack();
            if (_attackPositionMovementActionDatum.AttackEventType == ActionEventType.Activated) _isAttacking = true;
        }

        protected override void OnTrigger(ActionContext context)
        {
            base.OnTrigger(context);
            if (_attackPositionMovementActionDatum.ProjectileEventType == ActionEventType.Triggered) ProjectileAttack();
            if (_attackPositionMovementActionDatum.AttackEventType == ActionEventType.Triggered) _isAttacking = true;
        }
        
        protected override void OnReachedMaxTime()
        {
            OnDeactivation(PreviousContext);
            base.OnReachedMaxTime();
        }

        protected override void OnDeactivation(ActionContext context)
        {
            base.OnDeactivation(context);
            if (_attackPositionMovementActionDatum.ProjectileEventType == ActionEventType.Deactivated) ProjectileAttack();
            if (_attackPositionMovementActionDatum.AttackEventType == ActionEventType.Deactivated) Attack();
        }

        protected override void OnInterruption(ActionContext context)
        {
            base.OnInterruption(context);
            if (_attackPositionMovementActionDatum.ProjectileEventType == ActionEventType.Interrupted) ProjectileAttack();
            if (_attackPositionMovementActionDatum.AttackEventType == ActionEventType.Interrupted) Attack();
        }

        protected override void OnCancellation(ActionContext context)
        {
            base.OnCancellation(context);
            if (_attackPositionMovementActionDatum.ProjectileEventType == ActionEventType.Cancelled) ProjectileAttack();
            if (_attackPositionMovementActionDatum.AttackEventType == ActionEventType.Cancelled) Attack();
        }
        
        private void Attack()
        {
            var results = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(
                transform.position + Quaternion.LookRotation(PreviousContext.TargetDirection) * _attackPositionMovementActionDatum.AttackOffset,
                _attackPositionMovementActionDatum.AttackRadius, results,
                _attackPositionMovementActionDatum.AttackLayerMask);

            for (var i = 0; i < size; i++)
            {
                var hitCollider = results[i];
                if (_colliders.Contains(hitCollider)) continue;
                _colliders.Add(hitCollider);
                
                if (hitCollider.TryGetComponent(out Health health))
                {
                    health.Damage(_attackPositionMovementActionDatum.AttackDamage);
                }
                
                if (!_attackPositionMovementActionDatum.HasKnockback) continue;
                if (!hitCollider.TryGetComponent(out StunBehaviourComponent stunBehaviour)) continue;
                //if (!hitCollider.TryGetComponent(out ForceController forceController)) continue;

                switch (_attackPositionMovementActionDatum.KnockbackType)
                {
                    case KnockbackType.Directional:
                        var velocity = MovementController.ForceController.GetVelocity();
                        velocity.y = 0;
                        stunBehaviour.Stun(velocity.normalized);
                        //forceController.ApplyForceEvent(_attackPositionMovementActionDatum.ForceEventDatum, Quaternion.LookRotation(velocity));
                        break;
                    case KnockbackType.Radial:
                        var direction = (hitCollider.transform.position - transform.position);
                        direction.y = 0;
                        stunBehaviour.Stun(direction.normalized);
                        //forceController.ApplyForceEvent(_attackPositionMovementActionDatum.ForceEventDatum, Quaternion.LookRotation(direction));
                        break;
                    case KnockbackType.None:
                    default:
                        break;
                }
            }

            if (size <= 0) return;
            if (_attackPositionMovementActionDatum.DoScreenShakeOnHit) ShakeScreen();
            if (_attackPositionMovementActionDatum.HasHitStop) GameManager.Instance.TriggerHitStop(_attackPositionMovementActionDatum.HitStopDuration);
            if (_attackPositionMovementActionDatum.EndActionOnHit) Cancel(PreviousContext);
        }

        private void ProjectileAttack()
        {
            var projectile = _projectilePool.GetObject();
            projectile.Fire(FireContext.Construct(transform.position, _attackPositionMovementActionDatum.FireDirection,
                TargetValueType.Direction, 10, HitResponseType.Destroy, DestroyResponseType.ReturnToPool));
        }
    }
}
