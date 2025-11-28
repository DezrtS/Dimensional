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
        
        public void SetWanderBehaviourData(PositionBehaviourComponent positionBehaviourComponent)
        {
            _positionBehaviourComponent = positionBehaviourComponent;
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
            
            _positionBehaviourComponent.SetTargetPosition(targetPosition);
            if (!_positionBehaviourComponent.IsActive) _positionBehaviourComponent.Activate();
        }
    }
}
