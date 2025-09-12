using System;
using Interfaces;
using Systems.Movement;
using UnityEngine;

namespace Systems.Entities.Behaviours
{
    public class PositionBehaviourComponent : BehaviourComponent, IMove
    {
        public event Action RangeLimitPassed;
        
        [SerializeField] private bool disableYInput;
        
        [SerializeField] private float upperRangeLimit;
        [SerializeField] private float lowerRangeLimit;
        [SerializeField] private bool fleeTarget;
        
        private MovementController _movementController;
        private IMove _mover;
        private Vector3 _targetPosition;

        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
        }

        private void FixedUpdate()
        {
            if (!IsActive) return;
            var distance = Vector3.Distance(_targetPosition, transform.position);
            if (distance <= upperRangeLimit && distance >= lowerRangeLimit) return;
            RangeLimitPassed?.Invoke();
            Deactivate();
        }
        
        public void SetPositionBehaviourData(IMove mover, Vector3 targetPosition, float upperRangeLimit, float lowerRangeLimit, bool disableYInput, bool fleeTarget)
        {
            _mover = mover;
            _targetPosition = targetPosition;
            this.upperRangeLimit = upperRangeLimit;
            this.lowerRangeLimit = lowerRangeLimit;
            this.fleeTarget = fleeTarget;
        }

        public void SetPositionBehaviourData(IMove mover, Vector3 targetPosition)
        {
            _mover = mover;
            _targetPosition = targetPosition;
        }
        
        public void SetTargetPosition(Vector3 position) => _targetPosition = position;

        protected override void OnActivate()
        {
            _movementController.Initialize(this);
        }

        protected override void OnDeactivate()
        {
            _movementController.Initialize(_mover);
        }

        public Vector3 GetInput()
        {
            var difference = _targetPosition - transform.position;
            if (fleeTarget) difference *= -1f;
            if (disableYInput) difference.y = 0;
            var direction = difference.magnitude > 0.5f ? difference.normalized : difference;
            return direction;
        }
    }
}
