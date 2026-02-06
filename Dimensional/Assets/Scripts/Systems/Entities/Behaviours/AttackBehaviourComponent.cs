using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.Actions.Movement;
using Systems.Actions;
using Systems.Forces;
using UnityEngine;
using Action = Systems.Actions.Action;
using Random = UnityEngine.Random;

namespace Systems.Entities.Behaviours
{
    public class AttackBehaviourComponent : BehaviourComponent, IActivateActions
    {
        [SerializeField] private AttackPattern[] attackPatterns;

        private IEntity _sourceEntity;
        private PositionBehaviourComponent _positionBehaviourComponent;

        private Transform _target;
        
        private Dictionary<AttackPattern, Action> _actionDictionary;
        
        private AttackPattern _selectedAttackPattern;
        private Action _selectedAction;

        private Vector3 _targetPosition;
        private bool _preparingAttack;
        private bool _isAttacking;

        private float _windUpTimer;
        private float _cooldownTimer;

        private void Awake()
        {
            _actionDictionary = new Dictionary<AttackPattern, Action>();
            foreach (var attackPattern in attackPatterns)
            {
                var attack = attackPattern.ActionDatum.AttachAction(gameObject);
                _actionDictionary.Add(attackPattern, attack);
            }
            
            SwapAttack();
        }

        public void SetAttackBehaviourData(IEntity sourceEntity, PositionBehaviourComponent positionBehaviourComponent)
        {
            _sourceEntity = sourceEntity;
            _positionBehaviourComponent = positionBehaviourComponent;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        protected override void OnActivate()
        {
            _positionBehaviourComponent.RangeLimitPassed += PositionBehaviourComponentOnRangeLimitPassed;
        }

        protected override void OnDeactivate()
        {
            _windUpTimer = 0;
            _preparingAttack = false;
            _positionBehaviourComponent.RangeLimitPassed -= PositionBehaviourComponentOnRangeLimitPassed;
        }
        
        private void PositionBehaviourComponentOnRangeLimitPassed()
        {
            Deactivate();
        }

        private void FixedUpdate()
        {
            if (_cooldownTimer > 0)
            {
                var fixedDeltaTime = Time.fixedDeltaTime;
                _cooldownTimer -= fixedDeltaTime;
                return;
            }
            
            if (_windUpTimer > 0)
            {
                var fixedDeltaTime = Time.fixedDeltaTime;
                _windUpTimer -= fixedDeltaTime;
                if (_windUpTimer > 0) return;
                _windUpTimer = 0;
                Attack();
            }
            
            if (!IsActive || _preparingAttack || _isAttacking) return;
            
            var displacement = _target.position - transform.position;
            var midpoint = (_selectedAttackPattern.MaxRange + _selectedAttackPattern.MinRange) / 2f;
            var targetPosition = _target.position - displacement.normalized * midpoint;

            if (IsInsideRange()) PrepareAttack();
            
            _positionBehaviourComponent.SetTargetPosition(targetPosition);
            if (!_positionBehaviourComponent.IsActive && !_preparingAttack && !_isAttacking) _positionBehaviourComponent.Activate();
        }

        private bool IsInsideRange()
        {
            var displacement = _target.position - transform.position;
            var distance = displacement.magnitude;
            return distance < _selectedAttackPattern.MaxRange && distance > _selectedAttackPattern.MinRange;
        }

        private void SwapAttack()
        {
            var index = Random.Range(0, attackPatterns.Length);
            _selectedAttackPattern = attackPatterns[index];
            _selectedAction = _actionDictionary[_selectedAttackPattern];
        }

        private void PrepareAttack()
        {
            _preparingAttack = true;
            _windUpTimer = _selectedAttackPattern.WindUpDuration;
            _targetPosition = _target.position;
            if (_target.TryGetComponent(out ForceController forceController))
                _targetPosition += forceController.GetVelocity() * _selectedAttackPattern.WindUpDuration;
            _positionBehaviourComponent.Deactivate();
        }

        private void Attack()
        {
            _preparingAttack = false;
            _selectedAction.Activated += SelectedActionOnActivated;
            var actionContext = GetActionContext();
            actionContext = _selectedAction.ActionDatum.ActionContextModifierData.Aggregate(actionContext, (current, actionContextModifierDatum) => actionContextModifierDatum.Modify(current));
            _selectedAction.Activate(actionContext);
            _selectedAction.Activated -= SelectedActionOnActivated;
        }

        private void SelectedActionOnActivated(Action action, ActionContext context)
        {
            _isAttacking = true;
            _positionBehaviourComponent.Deactivate();
            _selectedAction.Cancelled += SelectedActionOnDeactivated;
            _selectedAction.Deactivated += SelectedActionOnDeactivated;
        }

        private void SelectedActionOnDeactivated(Action action, ActionContext context)
        {
            _selectedAction.Cancelled -= SelectedActionOnDeactivated;
            _selectedAction.Deactivated -= SelectedActionOnDeactivated;
            _isAttacking = false;
            _cooldownTimer = _selectedAttackPattern.CooldownDuration;
            
            SwapAttack();
        }

        public ActionContext GetActionContext()
        {
            var displacement = _targetPosition - transform.position;
            displacement.y = 0;
            var actionContext = ActionContext.Construct(this, _sourceEntity, gameObject, null, displacement.normalized);
            actionContext.TargetPosition = _targetPosition;
            actionContext.TargetGameObject = _target.gameObject;
            return actionContext;
        }
    }
}
