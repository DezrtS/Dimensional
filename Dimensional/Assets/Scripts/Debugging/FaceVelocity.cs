using System;
using Systems.Forces;
using Systems.Movement;
using UnityEngine;

namespace Debugging
{
    public class FaceVelocity : MonoBehaviour
    {
        [SerializeField] private ForceController forceController;
        [SerializeField] private Transform targetTransform;

        private void FixedUpdate()
        {
            var velocity = forceController.GetVelocity();
            if (velocity.sqrMagnitude <= 0.1f) return;
            targetTransform.forward = velocity.normalized;
        }
    }
}
