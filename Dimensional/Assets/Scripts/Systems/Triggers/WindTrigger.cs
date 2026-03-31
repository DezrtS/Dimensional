using System;
using System.Collections.Generic;
using Systems.Forces;
using Systems.Movement;
using UnityEngine;

namespace Systems.Triggers
{
    public class WindTrigger : MonoBehaviour
    {
        [SerializeField] private float windSpeed;
        [SerializeField] private Vector3 windDirection;
        [SerializeField] private float maxForceSpeed = 25f;
        [Range(-1, 1)] [SerializeField] private float minVelocityDot = 0.5f;
        
        private readonly List<ForceController> _forceControllers = new List<ForceController>();

        private void FixedUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            for (var i = _forceControllers.Count - 1; i >= 0; i--)
            {
                var controller = _forceControllers[i];
                var velocity = controller.GetVelocity();
                var forceDirection = transform.rotation * windDirection;
                var velocityDot = Vector3.Dot(velocity.normalized, windDirection);
                if (velocity.magnitude >= maxForceSpeed && velocityDot >= minVelocityDot) continue;
                controller.ApplyForce(forceDirection * windSpeed, ForceMode.Force);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ForceController forceController))
            {
                _forceControllers.Add(forceController);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ForceController forceController))
            {
                _forceControllers.Remove(forceController);
            }
        }
    }
}
