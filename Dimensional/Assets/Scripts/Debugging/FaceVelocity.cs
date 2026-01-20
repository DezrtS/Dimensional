using System;
using Systems.Forces;
using Systems.Movement;
using UnityEngine;

namespace Debugging
{
    public class FaceVelocity : MonoBehaviour
    {
        [SerializeField] private ComplexForceController forceController;
        [SerializeField] private Transform targetTransform;

        private void FixedUpdate()
        {
            var velocity = forceController.GetVelocityComponent(VelocityType.Movement);
            if (velocity.sqrMagnitude <= 0.1f) return;
            targetTransform.forward = velocity.normalized;
        }
    }
}
