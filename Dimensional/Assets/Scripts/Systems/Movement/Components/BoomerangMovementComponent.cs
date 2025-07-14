using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Movement.Components
{
    public class BoomerangMovementComponent : MovementComponent
    {
        [Header("Rewind Settings")]
        [SerializeField] private float minRecordDistance = 0.1f;
        [SerializeField] private float rewindSpeed = 10f;
        [SerializeField] private float maxRewindDuration = 3f;
        [SerializeField] private float positionSnapThreshold = 0.05f;

        [SerializeField] private float rewindTime;
        [SerializeField] private AnimationCurve rewindCurve;
        
        private Coroutine _boomerangCoroutine;
        private readonly Stack<Vector3> _positionHistory = new Stack<Vector3>();
        private Vector3 _lastRecordedPosition;
        private bool _isRecording;
        private float _rewindStartTime;

        public override void Initialize(MovementController movementController)
        {
            base.Initialize(movementController);
            movementController.Grounded += MovementControllerOnGrounded;
        }
        
        protected override void OnActivate()
        {
            MovementController.ForceController.UseGravity = false;
            _boomerangCoroutine = StartCoroutine(BoomerangCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_boomerangCoroutine != null) 
            {
                StopCoroutine(_boomerangCoroutine);
                _boomerangCoroutine = null;
            }
            MovementController.ForceController.UseGravity = true;
            _isRecording = false;
        }
        
        private void MovementControllerOnGrounded(bool isGrounded)
        {
            if (!isGrounded && !_isRecording)
            {
                StartRecording();
            }
        }

        private void StartRecording()
        {
            _positionHistory.Clear();
            _lastRecordedPosition = transform.position;
            _positionHistory.Push(_lastRecordedPosition);
            _isRecording = true;
        }

        private void Update()
        {
            if (!_isRecording || IsActive) return;
            
            // Record based on distance moved instead of time
            float distanceMoved = Vector3.Distance(transform.position, _lastRecordedPosition);
            if (distanceMoved >= minRecordDistance)
            {
                _positionHistory.Push(transform.position);
                _lastRecordedPosition = transform.position;
            }
        }

        private IEnumerator BoomerangCoroutine()
        {
            if (_positionHistory.Count == 0)
            {
                Debug.LogWarning("No position history for boomerang rewind!");
                Deactivate();
                yield break;
            }
            
            _isRecording = false;
            var rewindTimer = 0f;
            _rewindStartTime = Time.time;
            MovementController.ForceController.SetVelocity(Vector3.zero);
            
            // Start from current position
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = _positionHistory.Pop();
            
            while (_positionHistory.Count > 0 && Time.time - _rewindStartTime < maxRewindDuration)
            {
                var deltaTime = Time.deltaTime;
                // Calculate movement for this frame
                float step = rewindSpeed * rewindCurve.Evaluate(rewindTimer / rewindTime) * deltaTime;
                float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);
                
                // Move directly to target if we're close enough
                if (distanceToTarget <= step + positionSnapThreshold)
                {
                    currentPosition = targetPosition;
                    MovementController.ForceController.Teleport(currentPosition);
                    
                    // Get next target position
                    if (_positionHistory.Count > 0)
                    {
                        targetPosition = _positionHistory.Pop();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    // Move toward target
                    Vector3 moveDirection = (targetPosition - currentPosition).normalized;
                    currentPosition += moveDirection * step;
                    MovementController.ForceController.Teleport(currentPosition);
                }
                
                rewindTimer += deltaTime;
                yield return null;
            }
            
            // Final position correction
            MovementController.ForceController.Teleport(targetPosition);
            Deactivate();
        }
    }
}