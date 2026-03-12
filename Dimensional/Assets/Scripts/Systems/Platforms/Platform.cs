using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Systems.Platforms
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private bool canMove = true;
        [SerializeField] private SplineContainer splineContainer;
        [Space]
        [SerializeField] private Vector3 offset;
        [SerializeField] private bool autoOffset;
        [Space] 
        [Range(0, 1)] [SerializeField] private float durationOffset;
        [SerializeField] private float duration = 1f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        private Rigidbody _rigidbody;
        private float _timer;
        
        public Vector3 Velocity { get; private set; }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();

            if (!splineContainer) return;
            Vector3 startPosition = splineContainer.EvaluatePosition(0);
            if (autoOffset) offset = transform.position - startPosition;
            transform.position = startPosition + offset;
        }

        private void FixedUpdate()
        {
            if (!canMove || !splineContainer) return;
            var deltaTime = Time.fixedDeltaTime;
            _timer += deltaTime;
            var time = _timer / duration + (durationOffset * duration);
            var normalizedTime = Mathf.Clamp01(animationCurve.Evaluate(time));
            
            var nextTime = (_timer + deltaTime) / duration + (durationOffset * duration);
            var nextNormalizedTime = Mathf.Clamp01(animationCurve.Evaluate(nextTime));
                
            Vector3 currentTarget = splineContainer.EvaluatePosition(normalizedTime);
            Vector3 nextTarget = splineContainer.EvaluatePosition(nextNormalizedTime);

            Velocity = (nextTarget - currentTarget) / deltaTime;
                
            _rigidbody.MovePosition(currentTarget + offset);
        }
        
        public void SetCanMove(bool canMove) => this.canMove = canMove;

    }
}
