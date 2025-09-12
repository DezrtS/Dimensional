using System;
using UnityEngine;

namespace Systems.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyForceController : ForceController
    {
        private Rigidbody _rig;
        public override float Mass { get => _rig.mass; set => _rig.mass = value; }

        protected override void Awake()
        {
            _rig = GetComponent<Rigidbody>();
            base.Awake();
        }

        private void FixedUpdate()
        {
            if (IsKinematic || !UseGravity) return;
            var velocity = GetVelocity();
            if (velocity.y >= maxFallSpeed) return;
            velocity.y = Mathf.Min(velocity.y + overSpeedDeceleration * Time.fixedDeltaTime, maxFallSpeed);
            SetVelocity(velocity);
        }

        protected override void OnSetIsKinematic()
        {
            if (!IsKinematic) SetVelocity(Vector3.zero);
            _rig.isKinematic = IsKinematic;
        }

        protected override void OnSetUseGravity()
        {
            _rig.useGravity = UseGravity;
        }

        protected override void OnApplyForce(Vector3 force, ForceMode forceMode)
        {
            _rig.AddForce(force, forceMode);
        }

        protected override void OnApplyTorque(Vector3 torque, ForceMode forceMode)
        {
            _rig.AddTorque(torque, forceMode);
        }

        public override Vector3 GetVelocity()
        {
            return _rig.linearVelocity;
        }

        protected override void OnSetVelocity(Vector3 velocity)
        {
            _rig.linearVelocity = velocity;
        }
    }
}