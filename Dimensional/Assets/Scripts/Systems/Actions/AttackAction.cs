using System.Collections.Generic;
using Managers;
using Scriptables.Actions;
using Systems.Entities;
using Systems.Entities.Behaviours;
using Systems.Forces;
using Systems.Projectiles.Behaviours;
using UnityEngine;

namespace Systems.Actions
{
    public class AttackAction : Action
    {
        private AttackActionDatum _attackActionDatum;
        private List<Collider> _colliders;
        private ComplexForceController _forceController;

        private bool _isAttacking;    
        
        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            _attackActionDatum = (AttackActionDatum)actionDatum;
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
            if (_attackActionDatum.AttackEventType == ActionEventType.Activated) _isAttacking = true;
        }

        protected override void OnTrigger(ActionContext context)
        {
            base.OnTrigger(context);
            if (_attackActionDatum.AttackEventType == ActionEventType.Triggered) _isAttacking = true;
        }

        protected override void OnDeactivation(ActionContext context)
        {
            base.OnDeactivation(context);
            if (_attackActionDatum.AttackEventType == ActionEventType.Deactivated) Attack();
            _isAttacking = false;
        }

        protected override void OnInterruption(ActionContext context)
        {
            base.OnInterruption(context);
            if (_attackActionDatum.AttackEventType == ActionEventType.Interrupted) Attack();
            _isAttacking = false;
        }

        protected override void OnCancellation(ActionContext context)
        {
            base.OnCancellation(context);
            if (_attackActionDatum.AttackEventType == ActionEventType.Cancelled) Attack();
            _isAttacking = false;
        }
        
        private void Attack()
        {
            var hasHit = false;
            var results = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(
                transform.position + Quaternion.LookRotation(PreviousContext.TargetDirection) * _attackActionDatum.AttackOffset,
                _attackActionDatum.AttackRadius, results,
                _attackActionDatum.AttackLayerMask);

            for (var i = 0; i < size; i++)
            {
                var hitCollider = results[i];
                if (_colliders.Contains(hitCollider)) continue;
                _colliders.Add(hitCollider);
                var hitObject = hitCollider.attachedRigidbody ? hitCollider.attachedRigidbody.gameObject : hitCollider.gameObject;
                hasHit = true;
                
                if (hitObject.TryGetComponent(out Health health))
                {
                    health.Damage(_attackActionDatum.AttackDamage);
                }
                
                if (!_attackActionDatum.HasKnockback) continue;
                if (!hitObject.TryGetComponent(out StunBehaviourComponent stunBehaviour)) continue;

                switch (_attackActionDatum.KnockbackType)
                {
                    case KnockbackType.Directional:
                        var velocity = _forceController.GetVelocityComponent(VelocityType.Movement);
                        velocity.y = 0;
                        stunBehaviour.Stun(velocity.normalized);
                        break;
                    case KnockbackType.Radial:
                        var direction = (hitObject.transform.position - transform.position);
                        direction.y = 0;
                        stunBehaviour.Stun(direction.normalized);
                        break;
                    case KnockbackType.None:
                    default:
                        break;
                }
            }

            if (size <= 0 || !hasHit) return;
            if (_attackActionDatum.DoScreenShakeOnHit) ShakeScreen();
            if (_attackActionDatum.HasHitStop) GameManager.Instance.TriggerHitStop(_attackActionDatum.HitStopDuration);
            if (_attackActionDatum.EndActionOnHit && _isAttacking) Deactivate(PreviousContext);
        }
        
        protected override void OnEntityChanged(ActionContext context)
        {
            _forceController = context.SourceGameObject.GetComponent<ComplexForceController>();
        }
    }
}
