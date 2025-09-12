using System.Collections;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class GlideMovementComponent : MovementComponent
    {
        [SerializeField] private float glideMass;
        
        private Coroutine _glideCoroutine;

        private float _glideFallSpeedThreshold;
        private float _glideFallTimeThreshold;
        
        private float _glideFallSpeed;
        private float _previousMaxFallSpeed;
        private float _previousMass;

        public void SetGlideMovementData(float glideFallSpeedThreshold, float glideFallTimeThreshold, float glideFallSpeed)
        {
            _glideFallSpeedThreshold = glideFallSpeedThreshold;
            _glideFallTimeThreshold = glideFallTimeThreshold;
            
            _glideFallSpeed = glideFallSpeed;
        }
        
        protected override void OnActivate()
        {
            MovementController.Grounded += MovementControllerOnGrounded;
            _previousMaxFallSpeed = MovementController.ForceController.MaxFallSpeed;
            _previousMass = MovementController.ForceController.Mass;
            _glideCoroutine = StartCoroutine(GlideCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_glideCoroutine != null) StopCoroutine(_glideCoroutine);
            MovementController.ForceController.MaxFallSpeed = _previousMaxFallSpeed;
            MovementController.ForceController.Mass = _previousMass;
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
            
            MovementController.ForceController.MaxFallSpeed = _glideFallSpeed;
            MovementController.ForceController.Mass = glideMass;
        }
    }
}
