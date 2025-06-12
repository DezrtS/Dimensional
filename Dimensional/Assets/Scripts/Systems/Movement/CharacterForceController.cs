using System;
using UnityEngine;

namespace Systems.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterForceController : ForceController
    {
        [SerializeField] private float mass = 1.0f;
        [SerializeField] private float gravityForce = 9.8f;
        
        private CharacterController _characterController;
        private Vector3 Velocity { get; set; }

        protected override void Awake()
        {
            base.Awake();
            _characterController = GetComponent<CharacterController>();
            if (TryGetComponent(out Rigidbody rig))
            {
                rig.mass = mass;
            }
        }

        private void FixedUpdate()
        {
            if (IsKinematic) return;
            var fixedDeltaTime = Time.fixedDeltaTime;
            if (UseGravity) Velocity += Vector3.down * (gravityForce * fixedDeltaTime); ;
            _characterController.Move(Velocity * fixedDeltaTime);
        }

        protected override void OnSetIsKinematic()
        {
            if (!IsKinematic) SetVelocity(Vector3.zero);
        }

        public override Vector3 GetVelocity()
        {
            return Velocity;
        }

        protected override void OnSetVelocity(Vector3 velocity)
        {
            Velocity = velocity;
        }

        protected override void OnApplyForce(Vector3 force, ForceMode forceMode)
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            var deltaV = forceMode switch
            {
                ForceMode.Force =>
                    force * fixedDeltaTime / mass,
                ForceMode.Acceleration =>
                    force * fixedDeltaTime,
                ForceMode.Impulse =>
                    force / mass,
                ForceMode.VelocityChange =>
                    force,
                _ => throw new System.ArgumentException("Unsupported force mode")
            };

            Velocity += deltaV;
        }

        protected override void OnApplyTorque(Vector3 torque, ForceMode forceMode)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnTeleport(Vector3 position)
        {
            _characterController.enabled = false;
            base.OnTeleport(position);
            _characterController.enabled = true;
        }
    }
}
