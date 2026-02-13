using System;
using System.Linq;
using Interfaces;
using Managers;
using Scriptables.Movement;
using Systems.Forces;
using Systems.Platforms;
using UnityEngine;

namespace Systems.Movement
{
    public class MovementController : MonoBehaviour
    {
        public delegate void GroundedEventHandler(bool isGrounded);
        public event GroundedEventHandler Grounded;
        
        [Header("Movement Settings")]
        [SerializeField] private MovementControllerDatum movementControllerDatum;
        [SerializeField] private Dimensions movementDimensions;
        [SerializeField] private bool disableYInput;
        [Space]
        [Header("Grounded Settings")]
        [SerializeField] private bool disableGroundedCheck;
        [SerializeField] private bool allowDefaultHitValues;
        [SerializeField] private float groundedNormalDotThreshold;
        [SerializeField] private CheckType groundedCheckType;
        [SerializeField] private Vector3 groundedCheckOffset;
        [SerializeField] private Vector3 groundedCheckSize;
        [SerializeField] private float groundedCheckDistance;
        [SerializeField] private LayerMask groundedCheckLayerMask;
        [Header("Ground Platform Settings")]
        [SerializeField] private bool disableGroundPlatformCheck;
        [SerializeField] private float platformCheckRadius;
        [SerializeField] private Vector3 platformCheckOffset;
        [SerializeField] protected LayerMask platformCheckLayerMask;
        
        private Transform _platformTransform;
        protected Platform ParentPlatform { get; private set; }
        
        protected IMove Mover;

        protected MovementControllerDatum MovementControllerDatum => movementControllerDatum;
        public MovementControllerDatum CurrentMovementControllerDatum { get; set; }
        protected Dimensions MovementDimensions => movementDimensions;
        public ComplexForceController ForceController { get; private set; }
        public bool IsDisabled { get; set; }
        public bool IsGrounded { get; private set; }
        protected bool IsPlatformed { get; private set; }
        protected bool SkipGroundPlatformCheck { get; set; }

        private void Awake()
        {
            CurrentMovementControllerDatum = movementControllerDatum;
            ForceController = GetComponent<ComplexForceController>();
            GameManager.WorldDimensionsChanged += GameManagerOnWorldDimensionsChanged;
            OnAwake();
        }
        
        protected virtual void OnAwake() { }

        public void Initialize(IMove move)
        {
            Mover = move;
            OnInitialized(move);
        }

        protected virtual void OnInitialized(IMove move) { }

        private void Update()
        {
            OnUpdate();

            if (!disableGroundedCheck)
            {
                var previousIsGrounded = IsGrounded;
                IsGrounded = CheckIsGrounded();
                if (previousIsGrounded != IsGrounded) Grounded?.Invoke(IsGrounded);   
            }

            if (disableGroundPlatformCheck || SkipGroundPlatformCheck) return;
            CheckGroundPlatform();
        }

        protected virtual void OnUpdate() { }

        public void ResetMovementControllerDatum() => CurrentMovementControllerDatum = movementControllerDatum;

        private bool CheckIsGrounded()
        {
            var raycastHits = GameManager.CheckCast(transform.position, groundedCheckType, groundedCheckOffset, Vector3.down, groundedCheckSize,
                groundedCheckDistance, groundedCheckLayerMask);
            return raycastHits.Any(hit => Vector3.Dot(hit.normal, Vector3.up) > groundedNormalDotThreshold && (allowDefaultHitValues || hit.point != Vector3.zero));
        }

        private void CheckGroundPlatform()
        {
            var results = new Collider[10];
            var length = Physics.OverlapSphereNonAlloc(transform.position + platformCheckOffset, platformCheckRadius, results, platformCheckLayerMask, QueryTriggerInteraction.Ignore);
            if (length == 0 && IsPlatformed)
            {
                UnPlatform();
                return;
            }
            
            for (var i = 0; i < length; i++)
            {
                var platformCollider = results[i];
                var platform = platformCollider.attachedRigidbody ? platformCollider.attachedRigidbody.transform : platformCollider.transform;
                Platform(platform);
                break;
            }
        }

        protected void Platform(Transform platformTransform)
        {
            if (_platformTransform == platformTransform) return;
            if (IsPlatformed) UnPlatform();
            
            IsPlatformed = true;
            _platformTransform = platformTransform;
            
            var platform = platformTransform.GetComponent<Platform>();
            ParentPlatform = platform;
            ForceController.AddVelocityComponent(VelocityType.Movement, -platform.Velocity);
        }

        protected void UnPlatform()
        {
            if (!IsPlatformed) return;
            
            IsPlatformed = false;
            _platformTransform = null;
            
            ForceController.TransitionVelocityComponent(VelocityType.Platform, VelocityType.Movement, false);
            ParentPlatform = null;
        }

        public void Move()
        {
            Move(Mover?.GetInput() ?? Vector3.zero);
        }
        
        public void Move(Quaternion rotation)
        {
            Move(TransformInput(Mover?.GetInput() ?? Vector3.zero, rotation));
        }

        private void Move(Vector3 input)
        {
            if (IsDisabled) input = Vector3.zero;
            switch (movementDimensions)
            {
                case Dimensions.Two:
                    input.y = input.z;
                    input.z = 0;
                    break;
                case Dimensions.Three:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnMove(input, CurrentMovementControllerDatum);
        }

        protected virtual void OnMove(Vector3 input, MovementControllerDatum datum)
        {
            var trueInput = ForceController.GetRotation() * input;
            ForceController.AddVelocityComponent(VelocityType.Movement, HandleMovement(trueInput, datum));
        }

        private Vector3 HandleMovement(Vector3 input, MovementControllerDatum datum)
        {
            var currentVelocity =  ForceController.GetVelocityComponent(VelocityType.Movement);
            var maxSpeed = datum.MaxSpeed;
            if (!IsGrounded) maxSpeed *= datum.AirborneMaxSpeedMultiplier;
            
            if (disableYInput)
            {
                input.y = 0;
                currentVelocity.y = 0;
            }
            
            var targetVelocity = input * maxSpeed;
            
            var velocityDifference = targetVelocity - currentVelocity;
            var differenceDirection = velocityDifference.normalized;
            
            var velocityDot = Vector3.Dot(currentVelocity.normalized, differenceDirection);
            var directionDot = Vector3.Dot(currentVelocity, targetVelocity);
            
            float accelerationIncrement;
            if (velocityDot > datum.DecelerationDotThreshold || currentVelocity.magnitude == 0 || (datum.CanAccelerateWhileDecelerating && directionDot < datum.DecelerationDotThreshold))
            {
                accelerationIncrement = datum.Acceleration * datum.AccelerationCurve.Evaluate(currentVelocity.magnitude / maxSpeed) * Time.deltaTime;
                if (!IsGrounded) accelerationIncrement *= datum.AirborneAccelerationMultiplier;
            }
            else
            {
                accelerationIncrement = datum.Deceleration * datum.DecelerationCurve.Evaluate(currentVelocity.magnitude / maxSpeed) * Time.deltaTime;
            }

            if (velocityDifference.magnitude < accelerationIncrement) return velocityDifference;
            return differenceDirection * accelerationIncrement;
        }

        private void GameManagerOnWorldDimensionsChanged(Dimensions oldValue, Dimensions newValue)
        {
            movementDimensions = newValue;
        }

        private static Vector3 TransformInput(Vector3 input, Quaternion rotation)
        {
            return rotation * input;
        }
    }
}