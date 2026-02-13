using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Systems.Forces
{
    public abstract class ComplexForceController : ForceController
    {
        [Space] 
        [SerializeField] protected float gravityScale;
        
        private VelocityComponents _velocityComponents;
        
        protected readonly List<Vector3> CollisionNormals = new();
        
        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (UseGravity)
            {
                var velocity = GetVelocityComponent(VelocityType.Movement);
                velocity.y -= gravityScale * fixedDeltaTime;

                if (velocity.y < maxFallSpeed)
                {
                    velocity.y = Mathf.Min(velocity.y + overSpeedDeceleration * fixedDeltaTime, maxFallSpeed);   
                }
                
                velocity = ProjectVectorOnCollisionNormals(velocity);
                SetVelocityComponent(VelocityType.Movement, velocity);
            }
            
            if (IsKinematic) return;
            ApplyVelocity(GetVelocityComponent(VelocityType.All));
            CollisionNormals.Clear();
        }
        
        public Vector3 GetVelocityComponent(VelocityType velocityType)
        {
            return velocityType switch
            {
                VelocityType.All => _velocityComponents.GetVelocity(),
                VelocityType.Movement => _velocityComponents.Movement,
                VelocityType.Platform => _velocityComponents.Platform,
                VelocityType.External => _velocityComponents.External,
                _ => throw new ArgumentOutOfRangeException(nameof(velocityType), velocityType, null)
            };
        }
        
        public void SetVelocityComponent(VelocityType velocityType, Vector3 velocity)
        {
            switch (velocityType)
            {
                case VelocityType.All:
                    _velocityComponents.Movement = velocity;
                    _velocityComponents.Platform = velocity;
                    _velocityComponents.External = velocity;
                    break;
                case VelocityType.Movement:
                    _velocityComponents.Movement = velocity;
                    break;
                case VelocityType.Platform:
                    _velocityComponents.Platform = velocity;
                    break;
                case VelocityType.External:
                    _velocityComponents.External = velocity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(velocityType), velocityType, null);
            }
        }
        
        public void AddVelocityComponent(VelocityType velocityType, Vector3 delta)
        {
            SetVelocityComponent(velocityType, GetVelocityComponent(velocityType) + delta);
        }
        
        public void ClearVelocityComponent(VelocityType velocityType) => SetVelocityComponent(velocityType, Vector3.zero);

        public void TransitionVelocityComponent(VelocityType fromVelocityType, VelocityType toVelocityType, bool overrideVelocity)
        {
            if (overrideVelocity) SetVelocityComponent(toVelocityType, GetVelocityComponent(fromVelocityType));
            else AddVelocityComponent(toVelocityType, GetVelocityComponent(fromVelocityType));
            ClearVelocityComponent(fromVelocityType);
        }

        private Vector3 ProjectVectorOnCollisionNormals(Vector3 vector)
        {
            foreach (var normal in CollisionNormals.Where(normal => Vector3.Dot(vector, normal) < 0f))
            {
                vector = Vector3.ProjectOnPlane(vector, normal);
            }

            return vector;
        }

        
        protected abstract void ApplyVelocity(Vector3 velocity);
        
        public override void SetVelocity(Vector3 velocity)
        {
            SetVelocityComponent(VelocityType.Movement, velocity);
        }

        public override void ApplyForce(Vector3 force, ForceMode forceMode)
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            var deltaV = forceMode switch
            {
                ForceMode.Force =>
                    force * fixedDeltaTime / Mass,
                ForceMode.Acceleration =>
                    force * fixedDeltaTime,
                ForceMode.Impulse =>
                    force / Mass,
                ForceMode.VelocityChange =>
                    force,
                _ => throw new System.ArgumentException("Unsupported force mode")
            };

            AddVelocityComponent(VelocityType.Movement, deltaV);
        }
    }
}
