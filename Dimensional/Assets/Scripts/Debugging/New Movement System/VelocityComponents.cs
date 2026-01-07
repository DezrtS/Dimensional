using System;
using UnityEngine;

namespace Debugging.New_Movement_System
{
    public enum VelocityType
    {
        All,
        Movement,
        Platform,
        External
    }
    
    [Serializable]
    public struct VelocityComponents
    {
        public Vector3 Movement;
        public Vector3 Platform;
        public Vector3 External;
        
        public Vector3 GetVelocity() => Movement + Platform + External;
    }
}
