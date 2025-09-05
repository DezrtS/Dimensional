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
        private bool _isCharging;
        private float _chargeTimer;

        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            _springJumpMovementActionDatum = (SpringJumpMovementActionDatum)actionDatum;
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            if (!_isCharging) return;
            _chargeTimer += fixedDeltaTime;
            if (_springJumpMovementActionDatum.AutoJumpOnCharge && _chargeTimer > _springJumpMovementActionDatum.MaxChargeDuration)
            {
                OnDeactivation(PreviousContext);
            }
        }

        protected override void OnActivation(ActionContext context)
        {
            _isCharging = false;
            _chargeTimer = 0;
            base.OnActivation(context);
        }

        protected override void OnTrigger(ActionContext context)
        {
            if (!_isCharging)
            {
                _isCharging = true;
                _chargeTimer = 0;
                MovementController.IsDisabled = true;
            }
            else
            {
                _isCharging = false;
                HandleTrigger(context);
                SetJumpData(context);
                StartMovement();
            }
        }

        protected override void OnReachedMaxTime()
        {
            HandleDeactivation(PreviousContext);
            StopMovement();
        }

        protected override void OnDeactivation(ActionContext context)
        {
            if (IsTriggering) return;
            
            MovementController.IsDisabled = false;
            _jumpMultiplier = Mathf.Clamp01(_chargeTimer / _springJumpMovementActionDatum.MaxChargeDuration);

            if (_chargeTimer <= _springJumpMovementActionDatum.MinChargeDuration || IsTriggering)
            {
                if (_springJumpMovementActionDatum.HasMovementDatum) MovementController.ResetMovementControllerDatum();
                HandleDeactivation(context);
                StopMovement();
            }
            else
            {
                OnTrigger(context);
            }
        }

        protected override void OnCancellation(ActionContext context)
        {
            base.OnCancellation(context);
            MovementController.IsDisabled = false;
        }

        private void SetJumpData(ActionContext context)
        {
            var timeMultiplier = _jumpMultiplier / 1.5f + (1f / 3f);
            Context = new MovementActionContext(_springJumpMovementActionDatum.MovementVector * _jumpMultiplier,
                _springJumpMovementActionDatum.ForwardDuration * timeMultiplier, _springJumpMovementActionDatum.UpDuration * timeMultiplier,
                _springJumpMovementActionDatum.RightDuration * timeMultiplier, _springJumpMovementActionDatum.ForwardCurve,
                _springJumpMovementActionDatum.UpCurve, _springJumpMovementActionDatum.RightCurve, 
                _springJumpMovementActionDatum.IsXDistance, _springJumpMovementActionDatum.IsYDistance, 
                _springJumpMovementActionDatum.IsZDistance, _springJumpMovementActionDatum.IsForwardBlend,
                _springJumpMovementActionDatum.IsUpBlend, _springJumpMovementActionDatum.IsRightBlend);
            Context.SetMovementData(_springJumpMovementActionDatum.HasForward, _springJumpMovementActionDatum.HasUp, _springJumpMovementActionDatum.HasRight);
            Context.SetMovementDirection(context.TargetDirection);
        }
    }
}
