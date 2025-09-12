using System;
using Managers;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class PositionMovementComponentContext
    {
        public Vector3 MovementVector;
        public readonly float MovementTime;
        public readonly AnimationCurve XMovementCurve;
        public readonly AnimationCurve YMovementCurve;
        public readonly AnimationCurve ZMovementCurve;

        public bool OverrideForwardVelocity;
        public bool OverrideRightVelocity;
        public bool OverrideUpVelocity;

        public readonly bool UseVelocity;

        public Vector3 Forward { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Up { get; private set; }

        public PositionMovementComponentContext(Vector3 movementVector, float movementTime, AnimationCurve xMovementCurve, AnimationCurve yMovementCurve, AnimationCurve zMovementCurve, bool useVelocity)
        {
            MovementVector = movementVector;
            MovementTime = movementTime;
            XMovementCurve = xMovementCurve;
            YMovementCurve = yMovementCurve;
            ZMovementCurve = zMovementCurve;
            
            UseVelocity = useVelocity;
        }
        
        public PositionMovementComponentContext(Vector3 movementVector, float movementTime, AnimationCurve movementCurve, bool useVelocity)
        {
            MovementVector = movementVector;
            MovementTime = movementTime;
            XMovementCurve = movementCurve;
            YMovementCurve = movementCurve;
            ZMovementCurve = movementCurve;
            
            UseVelocity = useVelocity;
        }
        
        public void SetOverrideData(bool overrideForwardVelocity, bool overrideRightVelocity, bool overrideUpVelocity)
        {
            OverrideForwardVelocity = overrideForwardVelocity;
            OverrideRightVelocity = overrideRightVelocity;
            OverrideUpVelocity = overrideUpVelocity;
        }

        public void SetMovementDirection(Vector3 movementDirection)
        {
            Forward = movementDirection.normalized;
            Right = Vector3.Cross(Vector3.up, Forward).normalized;
            Up = Vector3.up;
        }
    }
    
    public abstract class MovementComponent : MonoBehaviour
    {
        public event Action Deactivated;
        
        public bool IsActive { get; private set; }
        protected MovementController MovementController { get; private set; }

        public virtual void Initialize(MovementController movementController)
        {
            MovementController = movementController;
        }

        protected virtual bool CanActivate()
        {
            return !IsActive;   
        }

        public void Activate()
        {
            if (!CanActivate()) return;
            IsActive = true;
            OnActivate();
        }
        
        protected abstract void OnActivate();

        public void Deactivate()
        {
            if (!IsActive) return;
            Deactivated?.Invoke();
            IsActive = false;
            OnDeactivate();
        }
        
        protected abstract void OnDeactivate();

        public static Vector3 GetNewVelocity(float normalizedTime, Vector3 currentVelocity, PositionMovementComponentContext positionMovementComponentContext)
        {
            var multiplier = positionMovementComponentContext.UseVelocity
                ? new Vector3(positionMovementComponentContext.XMovementCurve.Evaluate(normalizedTime),
                    positionMovementComponentContext.YMovementCurve.Evaluate(normalizedTime),
                    positionMovementComponentContext.ZMovementCurve.Evaluate(normalizedTime))
                : new Vector3(GameManager.Derivative(positionMovementComponentContext.XMovementCurve, normalizedTime) / positionMovementComponentContext.MovementTime,
                    GameManager.Derivative(positionMovementComponentContext.YMovementCurve, normalizedTime) / positionMovementComponentContext.MovementTime,
                    GameManager.Derivative(positionMovementComponentContext.ZMovementCurve, normalizedTime) / positionMovementComponentContext.MovementTime);
            
            var targetVelocity =
                positionMovementComponentContext.Forward * (positionMovementComponentContext.MovementVector.z * multiplier.z) +
                positionMovementComponentContext.Right   * (positionMovementComponentContext.MovementVector.x * multiplier.x) +
                positionMovementComponentContext.Up      * (positionMovementComponentContext.MovementVector.y * multiplier.y);
            
            var currentForward = Vector3.Dot(currentVelocity, positionMovementComponentContext.Forward);
            var currentRight   = Vector3.Dot(currentVelocity, positionMovementComponentContext.Right);
            var currentUp      = Vector3.Dot(currentVelocity, positionMovementComponentContext.Up);
            
            var finalForward = positionMovementComponentContext.OverrideForwardVelocity ? Vector3.Dot(targetVelocity, positionMovementComponentContext.Forward) : currentForward;
            var finalRight   = positionMovementComponentContext.OverrideRightVelocity ? Vector3.Dot(targetVelocity, positionMovementComponentContext.Right)   : currentRight;
            var finalUp      = positionMovementComponentContext.OverrideUpVelocity ? Vector3.Dot(targetVelocity, positionMovementComponentContext.Up)      : currentUp;
            
            var finalVelocity = positionMovementComponentContext.Forward * finalForward + positionMovementComponentContext.Right * finalRight + positionMovementComponentContext.Up * finalUp;
            return finalVelocity;
        }
    }
}
