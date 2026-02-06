using Systems.Movement;
using UnityEngine;

namespace Systems.Forces
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterForceController : ForceController
    {
        [SerializeField] private float mass = 1.0f;
        [SerializeField] private float gravityForce = 9.8f;
        
        private CharacterController _characterController;
        private Vector3 Velocity { get; set; }
        public override float Mass { get => mass; set => mass = value; }

        protected override void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            if (TryGetComponent(out Rigidbody rig))
            {
                rig.mass = mass;
            }
            base.Awake();
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (IsKinematic) return;
            if (UseGravity)
            {
                Velocity += Mathf.Min(gravityForce, maxFallSpeed + Velocity.y) * fixedDeltaTime * Vector3.down;
            }
            _characterController.Move(Velocity * fixedDeltaTime);
        }

        protected override void OnSetIsKinematic()
        {
            if (!IsKinematic) SetVelocity(Vector3.zero);
        }

        public override void SetVelocity(Vector3 velocity)
        {
            Velocity = velocity;
        }

        public override Vector3 GetVelocity()
        {
            return Velocity;
        }

        public override void ApplyForce(Vector3 force, ForceMode forceMode)
        {
            throw new System.NotImplementedException();
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
