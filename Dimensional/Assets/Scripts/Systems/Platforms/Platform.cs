using System;
using Systems.Movement;
using UnityEngine;
using UnityEngine.Splines;

namespace Systems.Platforms
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private float duration = 1f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        private RigidbodyForceController _rigidbodyForceController;
        private Rigidbody _rigidbody;
        
        public Vector3 Velocity => _rigidbodyForceController.GetVelocity();

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbodyForceController = GetComponent<RigidbodyForceController>();
        }

        private void FixedUpdate()
        {
            if (!splineContainer) return;
            
            var deltaTime = Time.fixedDeltaTime;
            var time = Time.timeSinceLevelLoad / duration;
            var normalizedTime = Mathf.Clamp01(animationCurve.Evaluate(time));

            var targetPosition = splineContainer.EvaluatePosition(normalizedTime);
            //var position = transform.position;
            //var difference = new Vector3(targetPosition.x - position.x, targetPosition.y - position.y, targetPosition.z - position.z);
            
            _rigidbody.MovePosition(targetPosition);
            
            //_rigidbodyForceController.ApplyForce(difference * deltaTime, ForceMode.VelocityChange);
        }
    }
}
