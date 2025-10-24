using System;
using System.Linq;
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
        private WanderBehaviourComponent _wanderBehaviourComponent;
        private PositionBehaviourComponent _positionBehaviourComponent;
        
        private Transform _target;

        protected override void OnAwake()
        {
            _movementController = GetComponent<MovementController>();
            _movementController.Initialize(this);
            
            _wanderBehaviourComponent = GetComponent<WanderBehaviourComponent>();
            _positionBehaviourComponent = GetComponent<PositionBehaviourComponent>();
            
            _wanderBehaviourComponent.SetWanderBehaviourData(_positionBehaviourComponent, this);
            _positionBehaviourComponent.RangeLimitPassed += PositionBehaviourComponentOnRangeLimitPassed;
        }

        private void Start()
        {
            ChangeMovementState(MovementState.Wandering);
        }

        private void FixedUpdate()
        {
            _movementController.Move();
            
            if (MovementState == MovementState.Stunned) return;
            var velocity = _movementController.ForceController.GetVelocity();
            velocity.y = 0;
            if (velocity.magnitude > 0.1f) root.forward = velocity;
            
            DetectSurroundings();
            if (MovementState == MovementState.Chasing) _positionBehaviourComponent.SetTargetPosition(_target.position);
        }
        
        private void PositionBehaviourComponentOnRangeLimitPassed()
        {
            if (MovementState == MovementState.Chasing) ChangeMovementState(MovementState.Wandering);
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
                ChangeMovementState(MovementState.Chasing);
                return;
            }
        }
        
        protected override void OnStateChanged(State oldState, State newState)
        {
            switch (oldState)
            {
                case State.None:
                    
                    break;
                case State.Spawning:
                    
                    break;
                case State.Idling:
                    
                    break;
                case State.Attacking:
                    
                    break;
                case State.Dying:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldState), oldState, null);
            }

            switch (newState)
            {
                case State.None:
                    
                    break;
                case State.Spawning:
                    
                    break;
                case State.Idling:
                    
                    break;
                case State.Attacking:
                    
                    break;
                case State.Dying:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        protected override void OnMovementStateChanged(MovementState oldState, MovementState newState)
        {
            switch (oldState)
            {
                case MovementState.Idling:
                    
                    break;
                case MovementState.Wandering:
                    _wanderBehaviourComponent.Deactivate();
                    break;
                case MovementState.Patrolling:
                    
                    break;
                case MovementState.Repositioning:
                    
                    break;
                case MovementState.Chasing:
                    _positionBehaviourComponent.Deactivate();
                    break;
                case MovementState.Fleeing:
                    
                    break;
                case MovementState.Stunned:
                    _movementController.IsDisabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldState), oldState, null);
            }

            switch (newState)
            {
                case MovementState.Idling:
                    
                    break;
                case MovementState.Wandering:
                    _wanderBehaviourComponent.Activate();
                    break;
                case MovementState.Patrolling:
                    
                    break;
                case MovementState.Repositioning:
                    
                    break;
                case MovementState.Chasing:
                    _positionBehaviourComponent.SetPositionBehaviourData(this, _target.position);
                    _positionBehaviourComponent.Activate();
                    break;
                case MovementState.Fleeing:
                    
                    break;
                case MovementState.Stunned:
                    _movementController.IsDisabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        public Vector3 GetInput()
        {
            return Vector3.zero;
        }
    }
}