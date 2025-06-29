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
        [SerializeField] private bool disableGroundedCheck;
        [SerializeField] private bool setYVelocityOnGrounded;
        
        protected IMove Mover;
        
        public MovementControllerDatum MovementControllerDatum => movementControllerDatum;
        public Dimensions MovementDimensions => movementDimensions;
        public ForceController ForceController { get; private set; }
        public bool IsDisabled { get; set; }
        public bool IsGrounded { get; private set; }

        protected virtual void Awake()
        {
            ForceController = GetComponent<ForceController>();
            GameManager.WorldDimensionsChanged += GameManagerOnWorldDimensionsChanged;
        }

        public void Initialize(IMove move)
        {
            Mover = move;
        }

        private void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
            if (disableGroundedCheck) return;
            UpdateIsGrounded();
            if (!IsGrounded || !setYVelocityOnGrounded) return;
            var velocity = ForceController.GetVelocity();
            if (!(velocity.y < 0)) return;
            velocity.y = -0.5f;
            ForceController.SetVelocity(velocity);
        }

        private void UpdateIsGrounded()
        {
            var previousIsGrounded = IsGrounded;
            var position = transform.position + movementControllerDatum.GroundedCheckOffset;
            IsGrounded = movementControllerDatum.GroundedCheckType switch
            {
                GroundedCheckType.Ray =>
                    Physics.Raycast(position, Vector3.down, movementControllerDatum.GroundedCheckDistance,
                        movementControllerDatum.GroundedLayerMask, QueryTriggerInteraction.Ignore),
                GroundedCheckType.Sphere =>
                    Physics.CheckSphere(position, movementControllerDatum.GroundedCheckDistance,
                        movementControllerDatum.GroundedLayerMask, QueryTriggerInteraction.Ignore),
                GroundedCheckType.Box =>
                    Physics.CheckBox(position, movementControllerDatum.GroundedCheckDistance / 2f * Vector3.one,
                        Quaternion.identity, movementControllerDatum.GroundedLayerMask, QueryTriggerInteraction.Ignore),
                _ => throw new ArgumentOutOfRangeException()
            };
            if (previousIsGrounded != IsGrounded) Grounded?.Invoke(IsGrounded);
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
            float accelerationIncrement;

            if (Vector3.Dot(currentVelocity, differenceDirection) > 0 || currentVelocity.magnitude == 0)
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