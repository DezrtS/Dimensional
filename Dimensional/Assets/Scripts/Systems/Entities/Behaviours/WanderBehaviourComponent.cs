using System;
using Interfaces;
using UnityEngine;

namespace Systems.Entities.Behaviours
{
    public class WanderBehaviourComponent : BehaviourComponent
    {
        [SerializeField] private float wanderRange;
        [SerializeField] private float wanderFrequency;
        [SerializeField] private float wanderFrequencyVariation;
        
        private PositionBehaviourComponent _positionBehaviourComponent;
        private IMove _mover;
        private float _wanderTimer;
        
        private void FixedUpdate()
        {
            if (!IsActive) return;
            
            if (_wanderTimer < 0) return;
            _wanderTimer -= Time.fixedDeltaTime;

            if (_wanderTimer > 0) return;
            SelectWanderTarget();
            ResetWanderTimer();
        }
        
        public void SetWanderBehaviourData(PositionBehaviourComponent positionBehaviourComponent, IMove mover)
        {
            _positionBehaviourComponent = positionBehaviourComponent;
            _mover = mover;
        }

        protected override void OnActivate()
        {
            ResetWanderTimer();
        }

        protected override void OnDeactivate()
        {
            _positionBehaviourComponent.Deactivate();
        }

        private void ResetWanderTimer()
        {
            _wanderTimer = UnityEngine.Random.Range(wanderFrequency - wanderFrequencyVariation, wanderFrequency + wanderFrequencyVariation);
        }

        private void SelectWanderTarget()
        {
            var angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            
            var wanderOffset = direction * UnityEngine.Random.Range(0, wanderRange);
            var targetPosition = transform.position + wanderOffset;
            
            _positionBehaviourComponent.SetPositionBehaviourData(_mover, targetPosition);
            _positionBehaviourComponent.Activate();
        }
    }
}
