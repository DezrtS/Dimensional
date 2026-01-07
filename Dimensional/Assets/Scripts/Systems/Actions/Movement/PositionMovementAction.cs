using Debugging.New_Movement_System;
using Scriptables.Actions;
using Scriptables.Actions.Movement;
using UnityEngine;

namespace Systems.Actions.Movement
{
    public class PositionMovementAction : MovementAction
    {
        private float _movementTimer;

        private PositionMovementActionDatum PositionMovementActionDatum { get; set; }
        protected MovementActionContext Context { get; set; }
        protected bool IsMoving { get; private set; }
        
        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            PositionMovementActionDatum = (PositionMovementActionDatum)actionDatum;
        }

        private bool HasReachedMaxTime()
        {
            var maxTime = Mathf.Max(
                Context.HasForwardVelocity ? Context.ForwardMovementTime : 0,
                Context.HasUpVelocity ? Context.UpMovementTime : 0,
                Context.HasRightVelocity ? Context.RightMovementTime : 0
            );

            return _movementTimer >= maxTime;
        }

        protected override void OnTrigger(ActionContext context)
        {
            base.OnTrigger(context);
            SetMovementContext(context);
            StartMovement();
        }

        protected override void OnDeactivation(ActionContext context)
        {
            if (!PositionMovementActionDatum.StopMovementOnDisable) return;
            base.OnDeactivation(context);
            StopMovement();
        }

        protected override void OnCancellation(ActionContext context)
        {
            base.OnCancellation(context);
            StopMovement();
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (!IsMoving) return;
            HandleMovement(_movementTimer);
            _movementTimer += fixedDeltaTime;

            if (PositionMovementActionDatum.DisableDurationLimit) return;
            
            if (!HasReachedMaxTime()) return;
            OnReachedMaxTime();
        }

        protected virtual void OnReachedMaxTime()
        {
            StopMovement();
            base.OnDeactivation(PreviousContext);
        }

        private void SetMovementContext(ActionContext context)
        {
            Context = new MovementActionContext(PositionMovementActionDatum.MovementVector,
                PositionMovementActionDatum.ForwardDuration, PositionMovementActionDatum.UpDuration,
                PositionMovementActionDatum.RightDuration, PositionMovementActionDatum.ForwardCurve,
                PositionMovementActionDatum.UpCurve, PositionMovementActionDatum.RightCurve, 
                PositionMovementActionDatum.IsXDistance, PositionMovementActionDatum.IsYDistance, 
                PositionMovementActionDatum.IsZDistance, PositionMovementActionDatum.IsForwardBlend,
                PositionMovementActionDatum.IsUpBlend, PositionMovementActionDatum.IsRightBlend);
            Context.SetMovementData(PositionMovementActionDatum.HasForward, PositionMovementActionDatum.HasUp, PositionMovementActionDatum.HasRight);
            Context.SetMovementDirection(context.TargetDirection);
        }

        protected void StartMovement()
        {
            MovementController.ForceController.UseGravity = false;
            IsMoving = true;
            _movementTimer = 0;
        }

        private void HandleMovement(float elapsedTime)
        {
            var velocity = GetVelocity(elapsedTime, MovementController.ForceController.GetVelocityComponent(VelocityType.Movement), Context);
            MovementController.ForceController.SetVelocityComponent(VelocityType.Movement, velocity);
            
            if (PositionMovementActionDatum.DisableDurationLimit || !Context.HasUpVelocity || elapsedTime <= PositionMovementActionDatum.UpDuration) return;
            Context.HasUpVelocity = false;
            MovementController.ForceController.UseGravity = true;
        }
        
        protected void StopMovement()
        {
            IsMoving = false;
            _movementTimer = 0;
            MovementController.ForceController.UseGravity = true;
        }
    }
}
