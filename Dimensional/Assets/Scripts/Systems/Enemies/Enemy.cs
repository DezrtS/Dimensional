using System;
using Interfaces;
using Managers;
using Scriptables.Entities;
using Systems.Entities;
using Systems.Entities.Behaviours;
using UnityEngine;

namespace Systems.Enemies
{
    public enum State
    {
        None,
        Spawning,
        Idling,
        Attacking,
        Dying
    }
    
    public enum MovementState
    {
        Idling,
        Wandering,
        Patrolling,
        Repositioning,
        Chasing,
        Fleeing,
        Stunned
    }
    
    public abstract class Enemy : MonoBehaviour, IEntity
    {
        [SerializeField] private EntityDatum entityDatum;
        private Health _health;
        private StunBehaviourComponent _stunBehaviour;

        public EntityDatum EntityDatum => entityDatum;
        public GameObject GameObject => gameObject;
        public uint Id { get; private set; }
        protected State State { get; private set; }
        protected MovementState MovementState { get; private set; }

        private void Awake()
        {
            Id = EntityManager.GetNextEntityId();
            _health = GetComponent<Health>();
            _health.HealthStateChanged += HealthOnHealthStateChanged;
            
            _stunBehaviour = GetComponent<StunBehaviourComponent>();
            _stunBehaviour.Stunned += StunBehaviourOnStunned;
            _stunBehaviour.Recovered += StunBehaviourOnRecovered;
            OnAwake();
        }

        private void StunBehaviourOnStunned()
        {
            ChangeMovementState(MovementState.Stunned);
        }

        private void StunBehaviourOnRecovered()
        {
            ChangeMovementState(MovementState.Wandering);
        }

        protected virtual void OnAwake() {}

        protected virtual void HealthOnHealthStateChanged(Health health, bool isDead)
        {
            if (!isDead) return;
            ChangeMovementState(MovementState.Idling);
            ChangeState(State.Dying);
        }

        public void ChangeState(State state)
        {
            if (state == State) return;
            
            var previousState = State;
            State = state;
            OnStateChanged(previousState, state);
        }

        public void ChangeMovementState(MovementState movementState)
        {
            if (movementState == MovementState) return;
            
            var previousMovementState = MovementState;
            MovementState = movementState;
            OnMovementStateChanged(previousMovementState, movementState);
        }
        
        protected abstract void OnStateChanged(State oldState, State newState);
        protected abstract void OnMovementStateChanged(MovementState oldState, MovementState newState);
    }
}