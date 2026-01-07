using System;
using UnityEngine;

namespace Debugging.New_Movement_System
{
    public class NewRigidbodyForceController : NewForceController
    {
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionStay(Collision other)
        {
            foreach (var contact in other.contacts)
            {
                CollisionNormals.Add(contact.normal);   
            }
        }

        protected override void ApplyVelocity(Vector3 velocity)
        {
            var currentVelocity = GetVelocity();
            var velocityDelta = velocity - currentVelocity;
            _rigidbody.AddForce(velocityDelta, ForceMode.VelocityChange);
        }

        protected override Vector3 GetVelocity()
        {
            return _rigidbody.linearVelocity;
        }
    }
}
