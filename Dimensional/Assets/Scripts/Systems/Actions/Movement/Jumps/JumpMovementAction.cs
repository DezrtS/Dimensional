using Scriptables.Actions;
using Scriptables.Actions.Movement;
using Systems.Movement;
using UnityEngine;

namespace Systems.Actions.Movement
{
    public class JumpMovementAction : PositionMovementAction
    {
        private JumpMovementActionDatum _jumpMovementActionDatum;
        
        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            _jumpMovementActionDatum = (JumpMovementActionDatum)actionDatum;
        }

        protected override void OnDeactivation(ActionContext context)
        {
            CutJump();
            if (Context.HasForwardVelocity || Context.HasRightVelocity || Context.HasUpVelocity) return;
            StopMovement();
            HandleDeactivation(context);
        }

        private void CutJump()
        {
            if (!Context.HasUpVelocity) return;
            
            Context.HasUpVelocity = false;
            var velocity = MovementController.ForceController.GetVelocity();
            velocity.y *= _jumpMovementActionDatum.CutJumpMultiplier;
            MovementController.ForceController.SetVelocity(velocity);
            MovementController.ForceController.UseGravity = true;
        }
    }
}
