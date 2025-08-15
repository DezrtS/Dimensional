using System;
using Interfaces;
using Scriptables.Entities;
using Systems.Entities;
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

        public EntityDatum EntityDatum => entityDatum;
        public GameObject GameObject => gameObject;
        protected State State { get; private set; }
        protected MovementState MovementState { get; private set; }

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.HealthStateChanged += HealthOnHealthStateChanged;
            OnAwake();
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