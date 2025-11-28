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
        Idle,
        Attack,
        Flee,
        Stun,
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
            ChangeState(State.Stun);
        }

        private void StunBehaviourOnRecovered()
        {
            ChangeState(State.Idle);
        }

        protected virtual void OnAwake() {}

        protected virtual void HealthOnHealthStateChanged(Health health, bool isDead)
        {
            ChangeState(isDead ? State.Stun : State.Idle);
        }

        protected void ChangeState(State state)
        {
            if (state == State) return;
            
            var previousState = State;
            State = state;
            OnStateChanged(previousState, state);
        }
        
        protected abstract void OnStateChanged(State oldState, State newState);
    }
}