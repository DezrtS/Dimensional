using System;
using Debugging.New_Movement_System;
using UnityEngine;

namespace Systems.Movement
{
    public class CharacterMovementController : MovementController
    {
        [Header("Gravity Settings")]
        [SerializeField] private bool disableGroundNormalCheck;
        [SerializeField] private float groundNormalCheckRadius;
        [SerializeField] private float groundNormalCheckDistance;
        [SerializeField] private Vector3 groundNormalCheckOffset;
        [SerializeField] private LayerMask groundNormalCheckLayerMask;
        [Space]
        [SerializeField] private float edgeThreshold = 0.2f;
        [SerializeField] private float edgeForce = 0.5f;
        
        private Vector3 _groundNormal = Vector3.up;

        private void FixedUpdate()
        {
            if (!disableGroundNormalCheck) _groundNormal = CalculateGroundNormal();

            var dot = Vector3.Dot(Vector3.up, _groundNormal);
            if (dot < edgeThreshold && dot > -edgeThreshold) ForceController.SetVelocityComponent(VelocityType.Movement, _groundNormal * edgeForce);
            
            //if (IsGrounded || _groundNormal != Vector3.up) ForceController.CancelVelocityInDirection(-_groundNormal);
        }
        
        private Vector3 CalculateGroundNormal()
        {
            var position = transform.position + groundNormalCheckOffset;
            return Physics.SphereCast(position, groundNormalCheckRadius, Vector3.down, out var hit, groundNormalCheckDistance, groundNormalCheckLayerMask, QueryTriggerInteraction.Ignore) ? hit.normal : Vector3.up;
        }
    }
}
