using System.Collections;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class GrappleMovementComponent : MovementComponent
    {
        private Coroutine _grappleCoroutine;

        private float _grappleSpeed;
        private float _grappleTime;
        private AnimationCurve _grappleCurve;

        private Transform _grappleTarget;

        public void SetGrappleMovementData(float grappleSpeed, float grappleTime, AnimationCurve grappleCurve)
        {
            _grappleSpeed = grappleSpeed;
            _grappleTime = grappleTime;
            _grappleCurve = grappleCurve;
        }
        
        public void SetGrappleTarget(Transform grappleTarget) => _grappleTarget = grappleTarget;
        
        protected override void OnActivate()
        {
            MovementController.Grounded += MovementControllerOnGrounded;
            MovementController.ForceController.UseGravity = false;
            _grappleCoroutine = StartCoroutine(GrappleCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_grappleCoroutine != null) StopCoroutine(_grappleCoroutine);
            MovementController.ForceController.UseGravity = true;
            MovementController.Grounded -= MovementControllerOnGrounded;
        }
        
        private void MovementControllerOnGrounded(bool isGrounded)
        {
            Deactivate();
        }
        
        private IEnumerator GrappleCoroutine()
        {
            var direction = (_grappleTarget.position - transform.position).normalized;
            var grappleTimer = 0f;

            while (true)
            {
                var normalizedTime = grappleTimer / _grappleTime;
                var velocity = _grappleSpeed * _grappleCurve.Evaluate(normalizedTime) * direction;
                
                MovementController.ForceController.SetVelocity(velocity);
                
                direction = (_grappleTarget.position - transform.position).normalized;
                if (Vector3.Dot(velocity, direction) < 0) break;
                
                grappleTimer += Time.deltaTime;
                yield return null;
            }
            
            Deactivate();
        }
    }
}
