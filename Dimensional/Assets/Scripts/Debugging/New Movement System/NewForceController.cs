using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Debugging.New_Movement_System
{
    public abstract class NewForceController : MonoBehaviour
    {
        private VelocityComponents _velocityComponents;

        protected readonly List<Vector3> CollisionNormals = new List<Vector3>();

        private void FixedUpdate()
        {
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

        public Vector3 ProjectVectorOnCollisionNormals(Vector3 vector)
        {
            return CollisionNormals.Aggregate(vector, Vector3.ProjectOnPlane);
        }
        
        protected abstract void ApplyVelocity(Vector3 velocity);

        protected abstract Vector3 GetVelocity();
    }
}
