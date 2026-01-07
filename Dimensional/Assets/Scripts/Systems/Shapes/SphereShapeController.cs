using System;
using Scriptables.Actions;
using UnityEngine;

namespace Systems.Shapes
{
    public class SphereShapeController : ShapeController
    {
        /*
        private PositionMovementComponent _positionMovementComponent;
        private bool _isJumping;
        private bool _isDashing;

        protected override void OnAwake()
        {
            _positionMovementComponent = GetComponent<PositionMovementComponent>();
        }

        protected override void OnAction(ActionContext actionContext)
        {
            /*
            var actionType = actionContext.Type;
            if (actionContext.Activated)
            {
                switch (actionType)
                {
                    case Type.JumpAction:
                        if (PlayerMovementController.IsJumping || (!PlayerMovementController.IsGrounded && !PlayerMovementController.CheckCoyoteTime)) return;
                        Jump();
                        break;
                    case Type.WallJumpAction:
                        WallJump();
                        break;
                    case Type.DashAction:
                        Dash();
                        break;
                    case Type.RollAction:
                        Roll();
                        break;
                }
            }
            else
            {
                if (actionType == Type.JumpAction && _isJumping) PlayerMovementController.CutJump();
            }
            */
        /*
        }

        private void Jump()
        {
            _isJumping = true;
            PlayerMovementController.IsJumping = true;
            _positionMovementComponent.Deactivated += PositionMovementComponentOnDeactivated;
            
            var valueTimeCurveActionDatum = ShapeDatum.JumpActionDatum;
            _positionMovementComponent.SetPositionMovementData(Vector3.up * valueTimeCurveActionDatum.Value, valueTimeCurveActionDatum.Time, valueTimeCurveActionDatum.Curve, false, true);
            _positionMovementComponent.SetOverrideData(false, false, true);
            _positionMovementComponent.SetMovementDirection(PlayerMovementController.GetForward());
            _positionMovementComponent.Activate();
        }

        private void WallJump()
        {
            _isJumping = true;
            PlayerMovementController.IsJumping = true;
            _positionMovementComponent.Deactivated += PositionMovementComponentOnDeactivated;
            
            var vectorTimeCurveActionDatum = ShapeDatum.WallJumpActionDatum;
            _positionMovementComponent.SetPositionMovementData(vectorTimeCurveActionDatum.Vector, vectorTimeCurveActionDatum.Time, vectorTimeCurveActionDatum.XCurve, vectorTimeCurveActionDatum.YCurve, vectorTimeCurveActionDatum.ZCurve, true, true);
            _positionMovementComponent.SetOverrideData(true, true, true);
            _positionMovementComponent.SetMovementDirection(-PlayerMovementController.WallSlideDirection);
            _positionMovementComponent.Activate();
        }

        private void PositionMovementComponentOnDeactivated()
        {
            _positionMovementComponent.Deactivated -= PositionMovementComponentOnDeactivated;
            
            if (_isJumping)
            {
                _isJumping = false;
                PlayerMovementController.IsJumping = false;
            }
            else if (_isDashing)
            {
                _isDashing = false;
                PlayerMovementController.IsDashing = false;
            }
        }

        private void Dash()
        {
            _isDashing = true;
            PlayerMovementController.IsDashing = true;
            _positionMovementComponent.Deactivated += PositionMovementComponentOnDeactivated;
            
            var vectorTimeCurveActionDatum = ShapeDatum.DashActionDatum;
            _positionMovementComponent.SetPositionMovementData(vectorTimeCurveActionDatum.Vector, vectorTimeCurveActionDatum.Time, vectorTimeCurveActionDatum.XCurve, vectorTimeCurveActionDatum.YCurve, vectorTimeCurveActionDatum.ZCurve, true, true);
            _positionMovementComponent.SetOverrideData(true, true, true);
            _positionMovementComponent.SetMovementDirection(PlayerMovementController.GetForward());
            _positionMovementComponent.Activate();
        }

        private void Roll()
        {
            
        }
        */
    }
}
