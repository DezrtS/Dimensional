using System;
using Managers;
using Scriptables.Actions;
using Scriptables.Actions.Movement;
using Systems.Movement;
using UnityEngine;

namespace Systems.Actions.Movement
{
    public enum MovementActionType
    {
        None,
        JumpAction,
        DoubleJumpAction,
        WallJumpAction,
        RollJumpAction,
        DashAction,
        DiveAction,
        AirAction,
        RollAction,
        LeftSpecialAction,
        RightSpecialAction,
        
        WallSlideAction,
    }
    
    public class MovementActionContext
    {
        public Vector3 MovementVector;
        
        public readonly float ForwardMovementTime;
        public readonly float UpMovementTime;
        public readonly float RightMovementTime;
        
        public readonly AnimationCurve ForwardMovementCurve;
        public readonly AnimationCurve UpMovementCurve;
        public readonly AnimationCurve RightMovementCurve;
        
        public bool HasForwardVelocity;
        public bool HasRightVelocity;
        public bool HasUpVelocity;
        
        public readonly bool IsXDistance;
        public readonly bool IsYDistance;
        public readonly bool IsZDistance;
        
        public readonly bool IsForwardBlend;
        public readonly bool IsUpBlend;
        public readonly bool IsRightBlend;

        public Vector3 Forward { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Up { get; private set; }

        public MovementActionContext(Vector3 movementVector, float movementTime, AnimationCurve movementCurve, bool isDistance, bool isBlend) 
        {
            MovementVector = movementVector;
            
            ForwardMovementCurve = movementCurve;
            UpMovementCurve = movementCurve;
            RightMovementCurve = movementCurve;
            
            ForwardMovementTime = movementTime;
            UpMovementTime = movementTime;
            RightMovementTime = movementTime;
            
            IsXDistance = isDistance;
            IsYDistance = isDistance;
            IsZDistance = isDistance;
            
            IsForwardBlend = isBlend;
            IsUpBlend = isBlend;
            IsRightBlend = isBlend;
        }

        public MovementActionContext(Vector3 movementVector, float forwardMovementTime, float upMovementTime, float rightMovementTime,
            AnimationCurve forwardMovementCurve, AnimationCurve upMovementCurve, AnimationCurve rightMovementCurve,
            bool isXDistance, bool isYDistance, bool isZDistance, bool isForwardBlend, bool isUpBlend, bool isRightBlend)
        {
            MovementVector = movementVector;
            
            ForwardMovementCurve = forwardMovementCurve;
            UpMovementCurve = upMovementCurve;
            RightMovementCurve = rightMovementCurve;
            
            ForwardMovementTime = forwardMovementTime;
            UpMovementTime = upMovementTime;
            RightMovementTime = rightMovementTime;
            
            IsXDistance = isXDistance;
            IsYDistance = isYDistance;
            IsZDistance = isZDistance;
            
            IsForwardBlend = isForwardBlend;
            IsUpBlend = isUpBlend;
            IsRightBlend = isRightBlend;
        }
        
        public void SetMovementData(bool hasForwardVelocity, bool hasUpVelocity, bool hasRightVelocity)
        {
            HasForwardVelocity = hasForwardVelocity;
            HasUpVelocity = hasUpVelocity;
            HasRightVelocity = hasRightVelocity;
        }

        public void SetMovementDirection(Vector3 movementDirection)
        {
            Forward = movementDirection.normalized;
            Right = Vector3.Cross(Vector3.up, Forward).normalized;
            Up = Vector3.up;
        }
    }
    
    public class MovementAction : Action
    {
        private MovementActionDatum _movementActionDatum;
        
        protected MovementController MovementController { get; private set; }

        public override void Initialize(ActionDatum actionDatum)
        {
            base.Initialize(actionDatum);
            _movementActionDatum = (MovementActionDatum)actionDatum;
        }

        private void MovementControllerOnGrounded(bool isGrounded)
        {
            if (!isGrounded) return;
            
            switch (_movementActionDatum.GroundedActionEventType)
            {
                case ActionEventType.Activated:
                    Activate(PreviousContext);
                    break;
                case ActionEventType.Triggered:
                    Trigger(PreviousContext);
                    break;
                case ActionEventType.Deactivated:
                    Deactivate(PreviousContext);
                    break;
                case ActionEventType.Interrupted:
                    Interrupt(PreviousContext);
                    break;
                case ActionEventType.Cancelled:
                    Cancel(PreviousContext);
                    break;
                case ActionEventType.None:
                default:
                    break;
            }
        }

        protected override void HandleActivation(ActionContext context)
        {
            base.HandleActivation(context);
            if (_movementActionDatum.PerformEventOnGrounded) MovementController.Grounded += MovementControllerOnGrounded;
            if (_movementActionDatum.HasMovementDatum) MovementController.CurrentMovementControllerDatum = _movementActionDatum.MovementControllerDatum;
        }

        protected override void HandleDeactivation(ActionContext context)
        {
            base.HandleDeactivation(context);
            if (_movementActionDatum.PerformEventOnGrounded) MovementController.Grounded -= MovementControllerOnGrounded;
            if (_movementActionDatum.HasMovementDatum) MovementController.ResetMovementControllerDatum();
        }

        protected override void HandleInterruption(ActionContext context)
        {
            base.HandleInterruption(context);
            if (_movementActionDatum.PerformEventOnGrounded) MovementController.Grounded -= MovementControllerOnGrounded;
            if (_movementActionDatum.HasMovementDatum) MovementController.ResetMovementControllerDatum();
        }

        protected override void HandleCancellation(ActionContext context)
        {
            base.HandleCancellation(context);
            if (_movementActionDatum.PerformEventOnGrounded) MovementController.Grounded -= MovementControllerOnGrounded;
            if (_movementActionDatum.HasMovementDatum) MovementController.ResetMovementControllerDatum();
        }

        protected override void OnEntityChanged(ActionContext context)
        {
            MovementController = context.SourceGameObject.GetComponent<MovementController>();
        }

        protected static Vector3 GetVelocity(float elapsedTime, Vector3 currentVelocity, MovementActionContext context)
        {
            var normalizedForwardTime =
                context.HasForwardVelocity ? Mathf.Clamp01(elapsedTime / context.ForwardMovementTime) : 1;
            var normalizedUpTime =
                context.HasUpVelocity ? Mathf.Clamp01(elapsedTime / context.UpMovementTime) : 1;
            var normalizedRightTime =
                context.HasRightVelocity ? Mathf.Clamp01(elapsedTime / context.RightMovementTime) : 1;

            // Precompute curve values (multiplier or blend factors depending on flags)
            var forwardCurveValue = context.ForwardMovementCurve.Evaluate(normalizedForwardTime);
            var upCurveValue = context.UpMovementCurve.Evaluate(normalizedUpTime);
            var rightCurveValue = context.RightMovementCurve.Evaluate(normalizedRightTime);

            // Handle distance curves
            if (context.IsZDistance && !context.IsForwardBlend)
                forwardCurveValue = GameManager.Derivative(context.ForwardMovementCurve, normalizedForwardTime) / context.ForwardMovementTime;

            if (context.IsYDistance && !context.IsUpBlend)
                upCurveValue = GameManager.Derivative(context.UpMovementCurve, normalizedUpTime) / context.UpMovementTime;

            if (context.IsXDistance && !context.IsRightBlend)
                rightCurveValue = GameManager.Derivative(context.RightMovementCurve, normalizedRightTime) / context.RightMovementTime;

            // Target components (only matter for override or blend-to-target)
            var targetForward = context.MovementVector.z;
            var targetUp = context.MovementVector.y;
            var targetRight = context.MovementVector.x;

            // Current components
            var currentForward = Vector3.Dot(currentVelocity, context.Forward);
            var currentUp = Vector3.Dot(currentVelocity, context.Up);
            var currentRight = Vector3.Dot(currentVelocity, context.Right);

            // Final axis velocities
            float finalForward, finalUp, finalRight;

            if (context.HasForwardVelocity)
            {
                finalForward = context.IsForwardBlend
                    ? Mathf.Lerp(currentForward, targetForward, forwardCurveValue) // curve = blend factor
                    : targetForward * forwardCurveValue;                           // curve = velocity/distance multiplier
            }
            else finalForward = currentForward;

            if (context.HasUpVelocity)
            {
                finalUp = context.IsUpBlend
                    ? Mathf.Lerp(currentUp, targetUp, upCurveValue)
                    : targetUp * upCurveValue;
            }
            else finalUp = currentUp;

            if (context.HasRightVelocity)
            {
                finalRight = context.IsRightBlend
                    ? Mathf.Lerp(currentRight, targetRight, rightCurveValue)
                    : targetRight * rightCurveValue;
            }
            else finalRight = currentRight;

            // Reconstruct final velocity
            return context.Forward * finalForward +
                   context.Right * finalRight +
                   context.Up * finalUp;
        }
    }
}
