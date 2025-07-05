using System.Collections;
using Managers;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class DashMovementComponent : MovementComponent
    {
        private Coroutine _dashCoroutine;
        
        private Vector3 _dashVector;
        private float _dashTime;
        private AnimationCurve _dashCurve;
        
        private Vector3 _dashDirection;

        public void SetDashMovementData(Vector3 dashVector, float dashTime, AnimationCurve dashCurve)
        {
            _dashVector = dashVector;
            _dashTime = dashTime;
            _dashCurve = dashCurve;
        }
        
        public void SetDashDirection(Vector3 dashDirection) => _dashDirection = dashDirection;
        
        // May Need to Add DisableGravity Option so DashMovementComponent can be Used in Combination With Other Components.
        protected override void OnActivate()
        {
            MovementController.ForceController.UseGravity = false;
            _dashCoroutine = StartCoroutine(DashCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_dashCoroutine != null) StopCoroutine(_dashCoroutine);
            MovementController.ForceController.UseGravity = true;
        }
        
        private IEnumerator DashCoroutine()
        {
            var dashTimer = 0f;
            var rotatedDashVector = Quaternion.LookRotation(_dashDirection.normalized) * _dashVector;

            while (dashTimer < _dashTime)
            {
                var normalizedTime = dashTimer / _dashTime;
                var slope = GameManager.Derivative(_dashCurve, normalizedTime);
                var velocity = slope / _dashTime * rotatedDashVector;

                MovementController.ForceController.SetVelocity(velocity);
                dashTimer += Time.deltaTime;
                yield return null;
            }
            
            Deactivate();
        }
    }
}
