using System.Collections;
using Managers;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class JumpMovementComponent : MovementComponent
    {
        private Coroutine _jumpCoroutine;
        
        private float _jumpTime;
        private float _jumpHeight;
        private AnimationCurve _jumpCurve;

        private bool _cutJump;
        private float _cutJumpMultiplier;
        
        public void SetJumpMovementData(float jumpTime, float jumpHeight, AnimationCurve jumpCurve, float cutJumpMultiplier)
        {
            _jumpTime = jumpTime;
            _jumpHeight = jumpHeight;
            _jumpCurve = jumpCurve;
            
            _cutJumpMultiplier = cutJumpMultiplier;
        }

        public void SetCutJump(bool cutJump)
        {
            _cutJump = cutJump;
        }
        
        protected override void OnActivate()
        {
            MovementController.ForceController.UseGravity = false;
            _jumpCoroutine = StartCoroutine(JumpCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_jumpCoroutine != null) StopCoroutine(_jumpCoroutine);
            MovementController.ForceController.UseGravity = true;
            _cutJump = false;
        }

        private IEnumerator JumpCoroutine()
        {
            var jumpTimer = 0f;
            
            while (jumpTimer < _jumpTime)
            {
                var normalizedTime = jumpTimer / _jumpTime;
                var slope = GameManager.Derivative(_jumpCurve, normalizedTime);
                var verticalVelocity = slope / _jumpTime * _jumpHeight;
                
                var currentVelocity = MovementController.ForceController.GetVelocity();
                currentVelocity.y = verticalVelocity;
                
                if (_cutJump)
                {
                    _cutJump = false;
                    currentVelocity.y *= _cutJumpMultiplier;
                    MovementController.ForceController.SetVelocity(currentVelocity);
                    break;
                }
                
                MovementController.ForceController.SetVelocity(currentVelocity);
                jumpTimer += Time.deltaTime;
                yield return null;
            }
            
            Deactivate();
        }
    }
}
