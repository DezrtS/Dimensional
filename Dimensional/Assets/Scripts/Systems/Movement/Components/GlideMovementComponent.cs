using System.Collections;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class GlideMovementComponent : MovementComponent
    {
        private Coroutine _glideCoroutine;

        private float _glideFallSpeedThreshold;
        private float _glideFallTimeThreshold;
        
        private float _glideFallSpeed;
        private float _glideSlowDownTime;
        private AnimationCurve _glideSlowDownCurve;

        public void SetGlideMovementData(float glideFallSpeedThreshold, float glideFallTimeThreshold, float glideFallSpeed, float glideSlowDownTime, AnimationCurve glideSlowDownCurve)
        {
            _glideFallSpeedThreshold = glideFallSpeedThreshold;
            _glideFallTimeThreshold = glideFallTimeThreshold;
            
            _glideFallSpeed = glideFallSpeed;
            _glideSlowDownTime = glideSlowDownTime;
            _glideSlowDownCurve = glideSlowDownCurve;
        }
        
        protected override void OnActivate()
        {
            MovementController.Grounded += MovementControllerOnGrounded;
            _glideCoroutine = StartCoroutine(GlideCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_glideCoroutine != null) StopCoroutine(_glideCoroutine);
            MovementController.ForceController.UseGravity = true;
            MovementController.Grounded -= MovementControllerOnGrounded;
        }

        private void MovementControllerOnGrounded(bool isGrounded)
        {
            Deactivate();
        }
        
        private IEnumerator GlideCoroutine()
        {
            var velocity = MovementController.ForceController.GetVelocity();
            while (velocity.y > _glideFallSpeedThreshold)
            {
                yield return new WaitForFixedUpdate();
                velocity = MovementController.ForceController.GetVelocity();
            }

            var glideFallTimer = 0f;
            while (glideFallTimer < _glideFallTimeThreshold)
            {
                glideFallTimer += Time.deltaTime;
                yield return null;
            }
            
            MovementController.ForceController.UseGravity = false;
            var initialY = MovementController.ForceController.GetVelocity().y;
            var glideSlowDownTimer = 0f;
            
            var diff = _glideFallSpeed - initialY;

            while (true)
            {
                var normalizedTime = Mathf.Clamp01(glideSlowDownTimer / _glideSlowDownTime);
                var verticalVelocity = initialY + diff * _glideSlowDownCurve.Evaluate(normalizedTime);
                
                velocity = MovementController.ForceController.GetVelocity();
                velocity.y = verticalVelocity;
                MovementController.ForceController.SetVelocity(velocity);
                
                glideSlowDownTimer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
