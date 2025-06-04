using UnityEngine;

namespace Systems.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyForceController : ForceController
    {
        private Rigidbody _rig;

        private void Awake()
        {
            _rig = GetComponent<Rigidbody>();
        }

        protected override void OnSetIsKinematic()
        {
            if (!IsKinematic) SetVelocity(Vector2.zero);
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