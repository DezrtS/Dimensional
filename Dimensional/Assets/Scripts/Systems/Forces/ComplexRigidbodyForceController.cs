using Debugging.New_Movement_System;
using UnityEngine;

namespace Systems.Forces
{
    [RequireComponent(typeof(Rigidbody))]
    public class ComplexRigidbodyForceController : ComplexForceController
    {
        private Rigidbody _rig;
        public override float Mass { get => _rig.mass; set => _rig.mass = value; }

        protected override void Awake()
        {
            _rig = GetComponent<Rigidbody>();
            _rig.useGravity = false;
            base.Awake();
        }

        protected override void OnSetIsKinematic()
        {
            if (!IsKinematic) ClearVelocityComponent(VelocityType.All);
            _rig.isKinematic = IsKinematic;
        }

        private void OnCollisionStay(Collision other)
        {
            if (IsKinematic) return;
            foreach (var contact in other.contacts)
            {
                CollisionNormals.Add(contact.normal);   
            }
        }

        protected override void ApplyVelocity(Vector3 velocity)
        {
            var currentVelocity = GetVelocity();
            var velocityDelta =  velocity - currentVelocity;
            _rig.AddForce(velocityDelta, ForceMode.VelocityChange);
        }
        
        public override Vector3 GetVelocity()
        {
            return _rig.linearVelocity;
        }

        protected override void OnApplyTorque(Vector3 torque, ForceMode forceMode)
        {
            _rig.AddTorque(torque, forceMode);
        }
    }
}
