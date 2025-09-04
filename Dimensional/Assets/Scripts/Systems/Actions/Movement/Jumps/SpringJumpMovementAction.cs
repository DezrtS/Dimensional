using Scriptables.Actions;
using Scriptables.Actions.Movement;
using Systems.Movement;
using UnityEngine;

namespace Systems.Actions.Movement
{
    public class SpringJumpMovementAction : JumpMovementAction
    {
        private SpringJumpMovementActionDatum _springJumpMovementActionDatum;
        
        private float _jumpMultiplier;
        private float _chargeTimer;

        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            _springJumpMovementActionDatum = (SpringJumpMovementActionDatum)actionDatum;
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            if (!IsTriggering || IsMoving) return;
            _chargeTimer += fixedDeltaTime;
            if (_springJumpMovementActionDatum.AutoJumpOnCharge && _chargeTimer > _springJumpMovementActionDatum.MaxChargeDuration)
            {
                OnDeactivation(PreviousContext);
            }
        }

        // Change Class to Only run HandleTrigger after _chargeTimer is complete
        protected override void OnTrigger(ActionContext context)
        {
            HandleTrigger(context);
            _chargeTimer = 0;
            MovementController.IsDisabled = true;
        }

        protected override void OnDeactivation(ActionContext context)
        {
            MovementController.IsDisabled = false;
            if (IsMoving)
            {
                StopMovement();
                HandleDeactivation(context);
                return;
            }
            
            _jumpMultiplier = Mathf.Clamp01((_chargeTimer - _springJumpMovementActionDatum.MinChargeDuration) / (_springJumpMovementActionDatum.MaxChargeDuration - _springJumpMovementActionDatum.MinChargeDuration));

            if (_jumpMultiplier <= 0 || (!MovementController.IsGrounded && !((PlayerMovementController)MovementController).IsWallSliding))
            {
                HandleDeactivation(context);
                return;
            }
            
            SetJumpData(context);
            StartMovement();
        }

        protected override void OnCancellation(ActionContext context)
        {
            base.OnCancellation(context);
            MovementController.IsDisabled = false;
            _chargeTimer = 0;
        }

        private void SetJumpData(ActionContext context)
        {
            Context = new MovementActionContext(_springJumpMovementActionDatum.MovementVector * _jumpMultiplier,
                _springJumpMovementActionDatum.ForwardDuration, _springJumpMovementActionDatum.UpDuration,
                _springJumpMovementActionDatum.RightDuration, _springJumpMovementActionDatum.ForwardCurve,
                _springJumpMovementActionDatum.UpCurve, _springJumpMovementActionDatum.RightCurve, 
                _springJumpMovementActionDatum.IsXDistance, _springJumpMovementActionDatum.IsYDistance, 
                _springJumpMovementActionDatum.IsZDistance, _springJumpMovementActionDatum.IsForwardBlend,
                _springJumpMovementActionDatum.IsUpBlend, _springJumpMovementActionDatum.IsRightBlend);
            Context.SetMovementData(_springJumpMovementActionDatum.HasForward, _springJumpMovementActionDatum.HasUp, _springJumpMovementActionDatum.HasRight);
            Context.SetMovementDirection(context.TargetDirection);
        }
    }
}
