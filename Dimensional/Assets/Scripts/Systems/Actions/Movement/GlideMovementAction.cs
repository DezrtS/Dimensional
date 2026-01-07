using Debugging.New_Movement_System;
using Scriptables.Actions;
using Scriptables.Actions.Movement;

namespace Systems.Actions.Movement
{
    public class GlideMovementAction : MovementAction
    {
        private GlideMovementActionDatum _glideMovementActionDatum;
        private float _previousMaxFallSpeed;
        private float _previousMass;

        private bool _reachedFallThreshold;
        private float _timer;
        
        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            _glideMovementActionDatum = (GlideMovementActionDatum)actionDatum;
        }

        protected override void OnActivation(ActionContext context)
        {
            _timer = 0;
            _reachedFallThreshold = false;
            base.OnActivation(context);
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            if (!_reachedFallThreshold)
            {
                var velocity = MovementController.ForceController.GetVelocityComponent(VelocityType.Movement);
                _reachedFallThreshold = velocity.y <= _glideMovementActionDatum.GlideFallSpeedThreshold;
                return;
            }
            
            if (_timer <= 0) return;
            _timer -= fixedDeltaTime;
            
            if (_timer > 0) return;
            StartMovement();
            HandleTrigger(PreviousContext);
        }

        private void StartMovement()
        {
            MovementController.ForceController.MaxFallSpeed = _glideMovementActionDatum.GlideFallSpeed;
            MovementController.ForceController.Mass = _glideMovementActionDatum.GlideMass;
        }

        private void StopMovement()
        {
            MovementController.ForceController.MaxFallSpeed = _previousMaxFallSpeed;
            MovementController.ForceController.Mass = _previousMass;
        }

        protected override void OnTrigger(ActionContext context)
        {
            _previousMaxFallSpeed = MovementController.ForceController.MaxFallSpeed;
            _previousMass = MovementController.ForceController.Mass;
            _timer = _glideMovementActionDatum.GlideFallTimeThreshold;
        }

        protected override void OnDeactivation(ActionContext context)
        {
            base.OnDeactivation(context);
            StopMovement();
        }

        protected override void OnInterruption(ActionContext context)
        {
            base.OnInterruption(context);
            StopMovement();
        }

        protected override void OnCancellation(ActionContext context)
        {
            base.OnCancellation(context);
            StopMovement();
        }
    }
}
