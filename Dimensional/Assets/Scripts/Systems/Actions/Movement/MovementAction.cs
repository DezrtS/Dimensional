using Managers;
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

        public Vector3 Forward { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Up { get; private set; }

        public MovementActionContext(Vector3 movementVector, float movementTime, AnimationCurve movementCurve, bool isDistance) 
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
        }

        public MovementActionContext(Vector3 movementVector, float forwardMovementTime, float upMovementTime, float rightMovementTime,
            AnimationCurve forwardMovementCurve, AnimationCurve upMovementCurve, AnimationCurve rightMovementCurve,
            bool isXDistance, bool isYDistance, bool isZDistance)
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
    
    public abstract class MovementAction : Action
    {
        protected MovementController MovementController { get; private set; }

        public void InitializeMovementController(MovementController movementController)
        {
            MovementController = movementController;
        }
        
        protected static Vector3 GetVelocity(float elapsedTime, Vector3 currentVelocity, MovementActionContext context)
        {
            var normalizedForwardTime =
                context.HasForwardVelocity ? Mathf.Clamp01(elapsedTime / context.ForwardMovementTime) : 1;
            var normalizedUpTime = 
                context.HasUpVelocity ? Mathf.Clamp01(elapsedTime / context.UpMovementTime) : 1;
            var normalizedRightTime = 
                context.HasRightVelocity ? Mathf.Clamp01(elapsedTime / context.RightMovementTime) : 1;
            
            var multiplier = Vector3.one;
            multiplier.x = context.IsXDistance ? 
                GameManager.Derivative(context.RightMovementCurve, normalizedRightTime) / context.RightMovementTime 
                : context.RightMovementCurve.Evaluate(normalizedRightTime);
            multiplier.y = context.IsYDistance ? 
                GameManager.Derivative(context.UpMovementCurve, normalizedUpTime) / context.UpMovementTime 
                : context.UpMovementCurve.Evaluate(normalizedUpTime);
            multiplier.z = context.IsZDistance ? 
                GameManager.Derivative(context.ForwardMovementCurve, normalizedForwardTime) / context.ForwardMovementTime 
                : context.ForwardMovementCurve.Evaluate(normalizedForwardTime);

            var targetVelocity =
                context.Forward * (context.MovementVector.z * multiplier.z) +
                context.Up * (context.MovementVector.y * multiplier.y) +
                context.Right * (context.MovementVector.x * multiplier.x);

            var currentForward = Vector3.Dot(currentVelocity, context.Forward);
            var currentUp = Vector3.Dot(currentVelocity, context.Up);
            var currentRight = Vector3.Dot(currentVelocity, context.Right);

            var finalForward = context.HasForwardVelocity ? Vector3.Dot(targetVelocity, context.Forward) : currentForward;
            var finalUp = context.HasUpVelocity ? Vector3.Dot(targetVelocity, context.Up) : currentUp;
            var finalRight = context.HasRightVelocity ? Vector3.Dot(targetVelocity, context.Right) : currentRight;
            
            var finalVelocity = context.Forward * finalForward + context.Right * finalRight + context.Up * finalUp;
            return finalVelocity;
        }
    }
}
