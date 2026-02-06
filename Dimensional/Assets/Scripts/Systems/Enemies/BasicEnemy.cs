using System;
using System.Linq;
using Systems.Forces;
using Interfaces;
using Scriptables.Entities;
using Systems.Entities.Behaviours;
using Systems.Movement;
using UnityEngine;

namespace Systems.Enemies
{
    public class BasicEnemy : Enemy, IMove
    {
        [SerializeField] private Transform root;
        
        [SerializeField] private float detectionRadius;
        [SerializeField] private LayerMask detectionLayerMask;
        [SerializeField] private EntityDatum.Type[] chaseTypes;
        
        private MovementController _movementController;
        private AttackBehaviourComponent _attackBehaviourComponent;
        private WanderBehaviourComponent _wanderBehaviourComponent;
        private PositionBehaviourComponent _positionBehaviourComponent;
        
        private Transform _target;

        protected override void OnAwake()
        {
            _movementController = GetComponent<MovementController>();
            _movementController.Initialize(this);

            _attackBehaviourComponent = GetComponent<AttackBehaviourComponent>();
            _wanderBehaviourComponent = GetComponent<WanderBehaviourComponent>();
            _positionBehaviourComponent = GetComponent<PositionBehaviourComponent>();
            
            _attackBehaviourComponent.SetAttackBehaviourData(this, _positionBehaviourComponent);
            _wanderBehaviourComponent.SetWanderBehaviourData(_positionBehaviourComponent);
            _positionBehaviourComponent.SetPositionBehaviourData(this);
            _positionBehaviourComponent.RangeLimitPassed += PositionBehaviourComponentOnRangeLimitPassed;
        }

        private void Start()
        {
            ChangeState(State.Idle);
        }

        private void FixedUpdate()
        {
            _movementController.Move();
            
            if (State == State.Stun) return;
            var velocity = _movementController.ForceController.GetVelocityComponent(VelocityType.Movement);
            velocity.y = 0;
            if (velocity.magnitude > 0.5f) root.forward = velocity;

            if (State != State.Idle) return;
            DetectSurroundings();
        }
        
        private void PositionBehaviourComponentOnRangeLimitPassed()
        {
            _target = null;
            if (State == State.Stun) return;
            ChangeState(State.Idle);
        }

        private void DetectSurroundings()
        {
            var results = new Collider[10];
            var length = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, detectionLayerMask, QueryTriggerInteraction.Ignore);

            for (var i = 0; i < length; i++)
            {
                if (!results[i].TryGetComponent(out IEntity entity)) continue;
                if (!chaseTypes.Contains(entity.EntityDatum.EntityType)) continue;
                
                _target = entity.GameObject.transform;
                ChangeState(State.Attack);
                return;
            }
        }
        
        protected override void OnStateChanged(State oldState, State newState)
        {
            switch (oldState)
            {
                case State.None:
                    break;
                case State.Idle:
                    _wanderBehaviourComponent.Deactivate();
                    break;
                case State.Attack:
                    _attackBehaviourComponent.Deactivate();
                    break;
                case State.Flee:
                    break;
                case State.Stun:
                    _movementController.IsDisabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldState), oldState, null);
            }
            
            switch (newState)
            {
                case State.None:
                    break;
                case State.Idle:
                    _wanderBehaviourComponent.Activate();
                    break;
                case State.Attack:
                    _attackBehaviourComponent.SetTarget(_target);
                    _attackBehaviourComponent.Activate();
                    break;
                case State.Flee:
                    break;
                case State.Stun:
                    _movementController.IsDisabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldState), oldState, null);
            }
        }

        public Vector3 GetInput()
        {
            return Vector3.zero;
        }
    }
}