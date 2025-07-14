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
        
        [Header("Movement Settings")]
        [SerializeField] private MovementControllerDatum movementControllerDatum;
        [SerializeField] private Dimensions movementDimensions;
        [SerializeField] private bool disableYInput;
        [Space]
        [Header("Grounded Settings")]
        [SerializeField] private bool disableGroundedCheck;
        [SerializeField] private GroundedCheckType groundedCheckType;
        [SerializeField] private float groundedCheckDistance;
        [SerializeField] private Vector3 groundedCheckOffset;
        [SerializeField] private LayerMask groundedCheckLayerMask;
        
        protected IMove Mover;
        
        public MovementControllerDatum MovementControllerDatum => movementControllerDatum;
        public MovementControllerDatum CurrentMovementControllerDatum { get; protected set; }
        public Dimensions MovementDimensions => movementDimensions;
        public ForceController ForceController { get; private set; }
        public bool IsDisabled { get; set; }
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            CurrentMovementControllerDatum = movementControllerDatum;
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
            if (velocityDot > datum.DecelerationDotThreshold || currentVelocity.magnitude == 0 || (datum.CanAccelerateWhileDecelerating && directionDot < datum.DecelerationDotThreshold))
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