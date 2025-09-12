using Scriptables.Actions;
using Scriptables.Actions.Movement;
using UnityEngine;

namespace Systems.Actions.Movement
{
    public class FlyMovementAction : MovementAction
    {
        private FlyMovementActionDatum _flyMovementActionDatum;
        private float _previousMaxFallSpeed;
        private float _previousMass;
        
        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            _flyMovementActionDatum = (FlyMovementActionDatum)actionDatum;
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            if (!IsTriggering) return;
            var velocity = MovementController.ForceController.GetVelocity();
            velocity.y = 0;
            var magnitude = velocity.magnitude;
            var ratio = Mathf.Clamp01((magnitude - _flyMovementActionDatum.MinVelocityMagnitude) / (_flyMovementActionDatum.MaxVelocityMagnitude - _flyMovementActionDatum.MinVelocityMagnitude));
            var blend = _flyMovementActionDatum.FlyFallCurve.Evaluate(ratio);
            var maxFallSpeed = Mathf.Lerp(_previousMaxFallSpeed, _flyMovementActionDatum.FlyFallSpeed, blend);
            MovementController.ForceController.MaxFallSpeed = maxFallSpeed;
        }

        protected override void OnTrigger(ActionContext context)
        {
            _previousMaxFallSpeed = MovementController.ForceController.MaxFallSpeed;
            _previousMass = MovementController.ForceController.Mass;
            base.OnTrigger(context);
            MovementController.ForceController.Mass = _flyMovementActionDatum.FlyMass;
        }

        protected override void OnDeactivation(ActionContext context)
        {
            base.OnDeactivation(context);
            MovementController.ForceController.MaxFallSpeed = _previousMaxFallSpeed;
            MovementController.ForceController.Mass = _previousMass;
        }

        protected override void OnInterruption(ActionContext context)
        {
            base.OnInterruption(context);
            MovementController.ForceController.MaxFallSpeed = _previousMaxFallSpeed;
            MovementController.ForceController.Mass = _previousMass;
        }

        protected override void OnCancellation(ActionContext context)
        {
            base.OnCancellation(context);
            MovementController.ForceController.MaxFallSpeed = _previousMaxFallSpeed;
            MovementController.ForceController.Mass = _previousMass;
        }
    }
}
