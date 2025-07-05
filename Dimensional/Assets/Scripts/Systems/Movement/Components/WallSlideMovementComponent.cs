using System.Collections;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class WallSlideMovementComponent : MovementComponent
    {
        [SerializeField] private bool canShiftWallSlide;
        
        private Coroutine _wallSlideCoroutine;
        
        private float _wallSlideSpeed;
        private float _wallSlideTime;
        private AnimationCurve _wallSlideCurve;
        
        private Vector3 _wallSlideDirection;
        
        private Vector3 _wallSlideCheckOffset;
        private float _wallSlideCheckDistance;
        private LayerMask _wallSlideCheckLayerMask;

        public void SetWallSlideMovementData(float wallSlideSpeed, float wallSlideTime, AnimationCurve wallSlideCurve, Vector3 wallSlideCheckOffset, float wallSlideCheckDistance, LayerMask wallSlideCheckLayerMask)
        {
            _wallSlideSpeed = wallSlideSpeed;
            _wallSlideTime = wallSlideTime;
            _wallSlideCurve = wallSlideCurve;
            
            _wallSlideCheckOffset = wallSlideCheckOffset;
            _wallSlideCheckDistance = wallSlideCheckDistance;
            _wallSlideCheckLayerMask = wallSlideCheckLayerMask;
        }
        
        public void SetWallSlideDirection(Vector3 wallSlideDirection) => _wallSlideDirection = wallSlideDirection;
        
        protected override void OnActivate()
        {
            MovementController.Grounded += MovementControllerOnGrounded;
            MovementController.ForceController.UseGravity = false;
            _wallSlideCoroutine = StartCoroutine(WallSlideCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_wallSlideCoroutine != null) StopCoroutine(_wallSlideCoroutine);
            MovementController.ForceController.UseGravity = true;
            MovementController.Grounded -= MovementControllerOnGrounded;
        }
        
        private void MovementControllerOnGrounded(bool isGrounded)
        {
            Deactivate();
        }
        
        private IEnumerator WallSlideCoroutine()
        {
            var wallSlideTimer = 0f;

            while (Physics.Raycast(transform.position + _wallSlideCheckOffset, _wallSlideDirection, _wallSlideCheckDistance, _wallSlideCheckLayerMask))
            {
                var normalizedTime = Mathf.Clamp01(wallSlideTimer / _wallSlideTime);
                var verticalVelocity = _wallSlideCurve.Evaluate(normalizedTime) * _wallSlideSpeed;
                var velocity = MovementController.ForceController.GetVelocity();
                if (!canShiftWallSlide) velocity = Vector3.zero;
                velocity.y = verticalVelocity;
                MovementController.ForceController.SetVelocity(velocity);
                wallSlideTimer += Time.deltaTime;
                yield return null;
            }
            
            Deactivate();
        }
    }
}
