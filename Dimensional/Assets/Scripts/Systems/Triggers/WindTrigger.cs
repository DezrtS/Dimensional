using System;
using System.Collections.Generic;
using Debugging.New_Movement_System;
using Systems.Movement;
using UnityEngine;

namespace Systems.Triggers
{
    public class WindTrigger : MonoBehaviour
    {
        [SerializeField] private float windSpeed;
        [SerializeField] private Vector3 windDirection;
        
        private readonly List<ForceController> _forceControllers = new List<ForceController>();

        private void FixedUpdate()
        {
            for (var i = _forceControllers.Count - 1; i >= 0; i--)
            {
                var controller = _forceControllers[i];
                controller.ApplyForce(windDirection * windSpeed, ForceMode.Force);
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
