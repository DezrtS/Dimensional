using System.Collections;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class WallDashMovementComponent : MovementComponent
    {
        private Coroutine _wallDashCoroutine;

        private float _wallDashSpeed;
        private float _wallDashTime;
        private AnimationCurve _wallDashCurve;
        
        private Vector3 _wallDashDirection;

        public void SetWallDashMovementData(float wallDashSpeed, float wallDashTime, AnimationCurve wallDashCurve)
        {
            _wallDashSpeed = wallDashSpeed;
            _wallDashTime = wallDashTime;
            _wallDashCurve = wallDashCurve;
        }
        
        public void SetWallDashDirection(Vector3 wallDashDirection) => _wallDashDirection = wallDashDirection;
        
        protected override void OnActivate()
        {
            MovementController.Grounded += MovementControllerOnGrounded;
            MovementController.ForceController.UseGravity = false;
            _wallDashCoroutine = StartCoroutine(WallDashCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_wallDashCoroutine != null) StopCoroutine(_wallDashCoroutine);
            MovementController.ForceController.UseGravity = true;
            MovementController.Grounded -= MovementControllerOnGrounded;
        }
        
        private void MovementControllerOnGrounded(bool isGrounded)
        {
            Deactivate();
        }
        
        private IEnumerator WallDashCoroutine()
        {
            var wallDashTimer = 0f;

            while (wallDashTimer < _wallDashTime)
            {
                var normalizedTime = wallDashTimer / _wallDashTime;
                var dashVelocity = _wallDashCurve.Evaluate(normalizedTime) * _wallDashSpeed * _wallDashDirection;
                var velocity = MovementController.ForceController.GetVelocity();
                velocity.x = dashVelocity.x;
                velocity.z = dashVelocity.z;
                MovementController.ForceController.SetVelocity(velocity);
                wallDashTimer += Time.deltaTime;
                yield return null;
            }
            
            Deactivate();
        }

    }
}
