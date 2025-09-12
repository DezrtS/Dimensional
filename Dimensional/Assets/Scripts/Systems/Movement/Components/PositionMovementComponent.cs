using System.Collections;
using Managers;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class PositionMovementComponent : MovementComponent
    {
        private Coroutine _movementCoroutine;
        
        private Vector3 _movementVector;
        private float _movementTime;
        private AnimationCurve _xMovementCurve;
        private AnimationCurve _yMovementCurve;
        private AnimationCurve _zMovementCurve;

        private bool _overrideForwardVelocity;
        private bool _overrideRightVelocity;
        private bool _overrideUpVelocity;

        private bool _useVelocity;
        private bool _disableGravity;
        private Vector3 _movementDirection = Vector3.forward;

        public void SetPositionMovementData(Vector3 movementVector, float movementTime, AnimationCurve xMovementCurve, AnimationCurve yMovementCurve, AnimationCurve zMovementCurve, bool useVelocity, bool disableGravity)
        {
            _movementVector = movementVector;
            _movementTime = movementTime;
            _xMovementCurve = xMovementCurve;
            _yMovementCurve = yMovementCurve;
            _zMovementCurve = zMovementCurve;
            
            _useVelocity = useVelocity;
            _disableGravity = disableGravity;
        }
        
        public void SetPositionMovementData(Vector3 movementVector, float movementTime, AnimationCurve movementCurve, bool useVelocity, bool disableGravity)
        {
            _movementVector = movementVector;
            _movementTime = movementTime;
            _xMovementCurve = movementCurve;
            _yMovementCurve = movementCurve;
            _zMovementCurve = movementCurve;
            
            _useVelocity = useVelocity;
            _disableGravity = disableGravity;
        }

        public void SetOverrideData(bool overrideForwardVelocity, bool overrideRightVelocity, bool overrideUpVelocity)
        {
            _overrideForwardVelocity = overrideForwardVelocity;
            _overrideRightVelocity = overrideRightVelocity;
            _overrideUpVelocity = overrideUpVelocity;
            if (!_overrideForwardVelocity && !_overrideRightVelocity && !_overrideUpVelocity && !_overrideUpVelocity) Deactivate();
        }

        public void SetOverrideUpVelocity(bool overrideUpVelocity)
        {
            _overrideUpVelocity = overrideUpVelocity;
            if (!_overrideForwardVelocity && !_overrideRightVelocity && !_overrideUpVelocity && !_overrideUpVelocity) Deactivate();
        }
        
        public void SetMovementDirection(Vector3 movementDirection) => _movementDirection = movementDirection;
        
        protected override void OnActivate()
        {
            if (_disableGravity) MovementController.ForceController.UseGravity = false;
            _movementCoroutine = StartCoroutine(MovementCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_movementCoroutine != null) StopCoroutine(_movementCoroutine);
            if (_disableGravity) MovementController.ForceController.UseGravity = true;
        }
        
        private IEnumerator MovementCoroutine()
        {
            var movementTimer = 0f;

            // Basis vectors (local space relative to movementDirection)
            var forward = _movementDirection.normalized;
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            var up = Vector3.up;

            while (movementTimer < _movementTime)
            {
                var normalizedTime = movementTimer / _movementTime;

                var multiplier = _useVelocity
                    ? new Vector3(_xMovementCurve.Evaluate(normalizedTime),
                                  _yMovementCurve.Evaluate(normalizedTime),
                                  _zMovementCurve.Evaluate(normalizedTime))
                    : new Vector3(GameManager.Derivative(_xMovementCurve, normalizedTime) / _movementTime,
                                  GameManager.Derivative(_yMovementCurve, normalizedTime) / _movementTime,
                                  GameManager.Derivative(_zMovementCurve, normalizedTime) / _movementTime);

                // Build target velocity in local basis
                var targetVelocity =
                    forward * (_movementVector.z * multiplier.z) +
                    right   * (_movementVector.x * multiplier.x) +
                    up      * (_movementVector.y * multiplier.y);

                var currentVelocity = MovementController.ForceController.GetVelocity();

                // Project current velocity into local basis
                var currentForward = Vector3.Dot(currentVelocity, forward);
                var currentRight   = Vector3.Dot(currentVelocity, right);
                var currentUp      = Vector3.Dot(currentVelocity, up);

                // Decide what to override
                var finalForward = _overrideForwardVelocity ? Vector3.Dot(targetVelocity, forward) : currentForward;
                var finalRight   = _overrideRightVelocity ? Vector3.Dot(targetVelocity, right)   : currentRight;
                var finalUp      = _overrideUpVelocity ? Vector3.Dot(targetVelocity, up)      : currentUp;

                // Recombine back into world space
                var finalVelocity = forward * finalForward + right * finalRight + up * finalUp;

                MovementController.ForceController.SetVelocity(finalVelocity);

                movementTimer += Time.deltaTime;
                yield return null;
            }

            Deactivate();
        }
    }
}
