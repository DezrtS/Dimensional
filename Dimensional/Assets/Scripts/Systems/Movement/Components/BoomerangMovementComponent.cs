using System.Collections;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class BoomerangMovementComponent : MovementComponent
    {
        private Coroutine _boomerangCoroutine;

        private float _boomerangFallSpeedThreshold;
        private float _boomerangFallTimeThreshold;
        private float _boomerangTime;
        private AnimationCurve _boomerangCurve;

        public void SetBoomerangMovementData(float boomerangFallSpeedThreshold, float boomerangFallTimeThreshold, float boomerangTime, AnimationCurve boomerangCurve)
        {
            _boomerangFallSpeedThreshold = boomerangFallSpeedThreshold;
            _boomerangFallTimeThreshold = boomerangFallTimeThreshold;
            _boomerangTime = boomerangTime;
            _boomerangCurve = boomerangCurve;
        }
        
        protected override void OnActivate()
        {
            MovementController.Grounded += MovementControllerOnGrounded;
            _boomerangCoroutine = StartCoroutine(BoomerangCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_boomerangCoroutine != null) StopCoroutine(_boomerangCoroutine);
            MovementController.ForceController.UseGravity = true;
            MovementController.Grounded -= MovementControllerOnGrounded;
        }
        
        private void MovementControllerOnGrounded(bool isGrounded)
        {
            Deactivate();
        }
        
        private IEnumerator BoomerangCoroutine()
        {
            var velocity = MovementController.ForceController.GetVelocity();
            while (velocity.y > _boomerangFallSpeedThreshold)
            {
                yield return new WaitForFixedUpdate();
                velocity = MovementController.ForceController.GetVelocity();
            }

            var boomerangFallTimer = 0f;
            while (boomerangFallTimer < _boomerangFallTimeThreshold)
            {
                boomerangFallTimer += Time.fixedDeltaTime;
                yield return null;
            }
            
            MovementController.ForceController.UseGravity = false;

            var boomerangTimer = 0f;
            var initialY = velocity.y;

            while (boomerangTimer < _boomerangTime)
            {
                var normalizedTime = Mathf.Clamp01(boomerangTimer / _boomerangTime);
                var verticalVelocity = initialY * _boomerangCurve.Evaluate(normalizedTime);
                
                velocity = MovementController.ForceController.GetVelocity();
                velocity.y = verticalVelocity;
                MovementController.ForceController.SetVelocity(velocity);

                boomerangTimer += Time.deltaTime;
                yield return null;
            }
            
            Deactivate();
        }
    }
}
