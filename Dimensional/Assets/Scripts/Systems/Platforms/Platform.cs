using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Systems.Platforms
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private bool canMove = true;
        [SerializeField] private bool canRotate;
        [SerializeField] private bool reverse;
        [SerializeField] private SplineContainer splineContainer;

        [Header("Position")]
        [SerializeField] private Vector3 offset;
        [SerializeField] private bool autoOffset;

        [Range(-1, 1)] [SerializeField] private float durationOffset;
        [SerializeField] private float duration = 1f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Rotation")]
        [SerializeField] private bool isOverride;
        [SerializeField] private Vector3 overrideEuler; // target rotation offset
        [SerializeField] private AnimationCurve overrideCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        [SerializeField] private Vector3 constantAngularVelocity; // degrees per second

        [SerializeField] private AnimationCurve angularX = AnimationCurve.Constant(0, 1, 1);
        [SerializeField] private AnimationCurve angularY = AnimationCurve.Constant(0, 1, 1);
        [SerializeField] private AnimationCurve angularZ = AnimationCurve.Constant(0, 1, 1);

        [SerializeField] private float rotationDuration = 1f;
        [Range(-1, 1)] [SerializeField] private float rotationOffset;

        private Quaternion _initialRotation;
        private Quaternion _previousRotation;
        private Rigidbody _rigidbody;
        private float _timer;

        public Vector3 Velocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _initialRotation = transform.rotation;
            _previousRotation = transform.rotation;
            
            if (reverse) rotationOffset = -rotationOffset;

            if (!splineContainer) return;

            if (autoOffset)
            {
                Vector3 startPosition = splineContainer.EvaluatePosition(0);
                offset = transform.position - startPosition;
            }

            Vector3 position = splineContainer.EvaluatePosition(durationOffset);
            transform.position = position + offset;
        }

        private void FixedUpdate()
        {
            if (!canMove) return;

            var deltaTime = Time.fixedDeltaTime;
            _timer += deltaTime;
            
            // =========================
            // ROTATION
            // =========================
            if (canRotate)
            {
                var rotationTime = (_timer / rotationDuration) + rotationOffset;
                var normalizedRotationTime = Mathf.Repeat(rotationTime, 1f);

                if (isOverride)
                {
                    // --- OVERRIDE MODE (angle-based) ---

                    float t = overrideCurve.Evaluate(normalizedRotationTime);

                    var targetRotation = _initialRotation * Quaternion.Euler(overrideEuler * t);

                    _rigidbody.MoveRotation(targetRotation);

                    // Compute angular velocity from rotation delta
                    var deltaRotation = targetRotation * Quaternion.Inverse(_previousRotation);
                    deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

                    if (angle > 180f) angle -= 360f;

                    AngularVelocity = axis * (angle * Mathf.Deg2Rad / deltaTime);

                    _previousRotation = targetRotation;
                }
                else
                {
                    // --- VELOCITY MODE (existing) ---

                    var angularMultiplier = new Vector3(
                        angularX.Evaluate(normalizedRotationTime),
                        angularY.Evaluate(normalizedRotationTime),
                        angularZ.Evaluate(normalizedRotationTime)
                    );

                    AngularVelocity = Vector3.Scale(constantAngularVelocity, angularMultiplier) * Mathf.Deg2Rad;

                    var deltaRotation = Quaternion.Euler(AngularVelocity * (Mathf.Rad2Deg * deltaTime));

                    var newRotation = _rigidbody.rotation * deltaRotation;
                    _rigidbody.MoveRotation(newRotation);

                    _previousRotation = newRotation;
                }
            }

            if (!splineContainer) return;

            var time = _timer / duration + durationOffset;
            var normalizedTime = Mathf.Clamp01(animationCurve.Evaluate(time));

            var nextTime = (_timer + deltaTime) / duration + durationOffset;
            var nextNormalizedTime = Mathf.Clamp01(animationCurve.Evaluate(nextTime));

            Vector3 currentTarget = splineContainer.EvaluatePosition(normalizedTime);
            Vector3 nextTarget = splineContainer.EvaluatePosition(nextNormalizedTime);

            Velocity = (nextTarget - currentTarget) / deltaTime;

            _rigidbody.MovePosition(currentTarget + offset);
        }

        public void SetCanMove(bool canMove) => this.canMove = canMove;

        public Vector3 GetVelocityAtPoint(Vector3 worldPoint)
        {
            var r = worldPoint - transform.position;
            var tangentialVelocity = Vector3.Cross(AngularVelocity, r);
            return Velocity + tangentialVelocity;
        }
    }
}