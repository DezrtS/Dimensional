using System;
using Interfaces;
using Managers;
using Scriptables.Movement;
using UnityEngine;

namespace Systems.Movement
{
    public class MovementController : MonoBehaviour
    {
        public delegate void GroundedEventHandler(bool isGrounded);
        public event GroundedEventHandler Grounded;
        
        [SerializeField] private MovementControllerDatum movementControllerDatum;
        [SerializeField] private Dimensions movementDimensions;
        [Space] 
        [SerializeField] private bool disableYInput;
        [Space]
        [Header("Grounded Settings")]
        [SerializeField] private bool cancelVelocityOnGrounded;
        [SerializeField] private bool disableGroundedCheck;
        [SerializeField] private GroundedCheckType groundedCheckType;
        [SerializeField] private float groundedCheckDistance;
        [SerializeField] private Vector3 groundedCheckOffset;
        [SerializeField] private LayerMask groundedCheckLayerMask;
        [Space]
        [Header("Gravity Settings")]
        [SerializeField] private bool disableGroundNormalCheck;
        [SerializeField] private float groundNormalCheckRadius;
        [SerializeField] private float groundNormalCheckDistance;
        [SerializeField] private Vector3 groundNormalCheckOffset;
        [SerializeField] private LayerMask groundNormalCheckLayerMask;
        [Space]
        [SerializeField] private float minSlideAngle = 30f; // Minimum angle before sliding starts
        [Space]
        [SerializeField] private float edgeThreshold = 0.2f;
        [SerializeField] private float edgeForce = 0.5f;
        
        [SerializeField] private float velocityThreshold = 0.5f;
        [SerializeField] private float linearDamping = 3;
        
        protected IMove Mover;
        private Vector3 _groundNormal = Vector3.up;
        
        public MovementControllerDatum MovementControllerDatum => movementControllerDatum;
        public Dimensions MovementDimensions => movementDimensions;
        public ForceController ForceController { get; private set; }
        public bool IsDisabled { get; set; }
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            ForceController = GetComponent<ForceController>();
            GameManager.WorldDimensionsChanged += GameManagerOnWorldDimensionsChanged;
            OnAwake();
        }
        
        protected virtual void OnAwake() { }

        public void Initialize(IMove move)
        {
            Mover = move;
        }

        private void Update()
        {
            OnUpdate();
            
            if (disableGroundedCheck) return;
            
            var previousIsGrounded = IsGrounded;
            IsGrounded = CheckIsGrounded();
            if (previousIsGrounded != IsGrounded) Grounded?.Invoke(IsGrounded);
        }

        protected virtual void OnUpdate() { }

        private bool CheckIsGrounded()
        {
            var position = transform.position + groundedCheckOffset;
            return groundedCheckType switch
            {
                GroundedCheckType.Ray =>
                    Physics.Raycast(position, Vector3.down, groundedCheckDistance,
                        groundedCheckLayerMask, QueryTriggerInteraction.Ignore),
                GroundedCheckType.Sphere =>
                    Physics.CheckSphere(position, groundedCheckDistance,
                        groundedCheckLayerMask, QueryTriggerInteraction.Ignore),
                GroundedCheckType.Box =>
                    Physics.CheckBox(position, groundedCheckDistance / 2f * Vector3.one,
                        Quaternion.identity, groundedCheckLayerMask, QueryTriggerInteraction.Ignore),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
            
            if (!disableGroundNormalCheck) _groundNormal = CalculateGroundNormal();
            //Debug.DrawRay(transform.position, _groundNormal, Color.red, Time.fixedDeltaTime);

            var dot = Vector3.Dot(Vector3.up, _groundNormal);
            if (dot < edgeThreshold && dot > -edgeThreshold) ForceController.ApplyForce(_groundNormal * edgeForce, ForceMode.VelocityChange);
            
            if ((IsGrounded || _groundNormal != Vector3.up) && cancelVelocityOnGrounded) ForceController.CancelVelocityInDirection(-_groundNormal);
            ApplyGravity();
            
            if (ForceController.GetVelocity().magnitude < velocityThreshold && Mover.GetInput().magnitude == 0) ForceController.SetVelocity(Vector3.zero);
        }

        protected virtual void OnFixedUpdate() { }

        private void ApplyGravity()
        {
            // Always apply base gravity
            Vector3 baseGravity = Vector3.down * (movementControllerDatum.GravityForce * Time.fixedDeltaTime);
            ForceController.ApplyForce(baseGravity, ForceMode.VelocityChange);
    
            // Apply slope gravity regardless of grounded state
            ApplySlopeGravity();
        }

        private void ApplySlopeGravity()
        {
            // Calculate slope angle between ground normal and up vector
            float slopeAngle = Vector3.Angle(_groundNormal, Vector3.up);
    
            // Only apply slope gravity if beyond minimum slide angle
            if (slopeAngle > minSlideAngle)
            {
                // Calculate gravity direction along the slope
                Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.down, _groundNormal).normalized;
                Vector3 slopeGravity = slopeDirection * (movementControllerDatum.GravityForce * Time.fixedDeltaTime);
        
                ForceController.ApplyForce(slopeGravity, ForceMode.VelocityChange);
            }
        }
        
        private Vector3 CalculateGroundNormal()
        {
            var position = transform.position + groundNormalCheckOffset;
            return Physics.SphereCast(position, groundNormalCheckRadius, Vector3.down, out var hit, groundNormalCheckDistance, groundNormalCheckLayerMask, QueryTriggerInteraction.Ignore) ? hit.normal : Vector3.up;
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

            OnMove(input, movementControllerDatum);
        }

        protected virtual void OnMove(Vector3 input, MovementControllerDatum datum)
        {
            var trueInput = ForceController.GetRotation() * input;
            ForceController.ApplyForce(HandleMovement(trueInput, datum), ForceMode.VelocityChange);
        }

        private Vector3 HandleMovement(Vector3 input, MovementControllerDatum datum)
        {
            var currentVelocity =  ForceController.GetVelocity();
            var maxSpeed = datum.MaxSpeed;
            
            if (disableYInput)
            {
                input.y = 0;
                currentVelocity.y = 0;
            }
            var targetVelocity = input.normalized * maxSpeed;
            
            var velocityDifference = targetVelocity - currentVelocity;
            var differenceDirection = velocityDifference.normalized;
            
            var velocityDot = Vector3.Dot(currentVelocity.normalized, differenceDirection);
            var directionDot = Vector3.Dot(currentVelocity, targetVelocity);
            
            float accelerationIncrement;
            if (velocityDot > movementControllerDatum.DecelerationDotThreshold || currentVelocity.magnitude == 0 || (movementControllerDatum.CanAccelerateWhileDecelerating && directionDot < movementControllerDatum.DecelerationDotThreshold))
            {
                accelerationIncrement = datum.Acceleration * datum.AccelerationCurve.Evaluate(currentVelocity.magnitude / maxSpeed) * Time.deltaTime;
            }
            else
            {
                accelerationIncrement = datum.Deceleration * datum.DecelerationCurve.Evaluate(currentVelocity.magnitude / maxSpeed) * Time.deltaTime;
            }
            if (!IsGrounded) accelerationIncrement *= datum.AirborneMultiplier;

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