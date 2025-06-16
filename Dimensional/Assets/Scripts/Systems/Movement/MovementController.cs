using System;
using Interfaces;
using Managers;
using Scriptables.Movement;
using UnityEngine;

namespace Systems.Movement
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private MovementControllerDatum movementControllerDatum;
        [SerializeField] private Dimensions movementDimensions;
        [Space] 
        [SerializeField] private bool disableYInput;

        [SerializeField] private bool setYVelocityOnGrounded;
        [SerializeField] private bool disableGroundedCheck;
        [SerializeField] private float groundedCheckDistance;
        [SerializeField] private Vector3 groundedCheckOffset;
        [SerializeField] private LayerMask groundedLayerMask;
        
        private IMove _move;
        
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
            _move = move;
        }

        private void Update()
        {
            if (disableGroundedCheck) return;
            IsGrounded = Physics.Raycast(transform.position + groundedCheckOffset, Vector3.down, groundedCheckDistance, groundedLayerMask, QueryTriggerInteraction.Ignore);
            if (!IsGrounded || !setYVelocityOnGrounded) return;
            var velocity = ForceController.GetVelocity();
            if (!(velocity.y < 0)) return;
            velocity.y = -0.5f;
            ForceController.SetVelocity(velocity);
        }

        public void Move()
        {
            Move(_move?.GetInput() ?? Vector3.zero);
        }
        
        public void Move(Quaternion rotation)
        {
            Move(TransformInput(_move?.GetInput() ?? Vector3.zero, rotation));
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
        
        protected Vector3 HandleMovement(Vector3 input, MovementControllerDatum datum)
        {
            if (disableYInput) input.y = 0;
            var currentVelocity =  ForceController.GetVelocity();
            var targetVelocity = input.normalized * datum.MaxSpeed;
            var targetSpeed = targetVelocity.magnitude;

            var velocityDifference = targetVelocity - currentVelocity;
            if (disableYInput) velocityDifference.y = 0;
            var differenceDirection = velocityDifference.normalized;
            float accelerationIncrement;

            if (currentVelocity.magnitude <= targetSpeed)
            {
                accelerationIncrement = datum.Acceleration * Time.deltaTime;
            }
            else
            {
                accelerationIncrement = datum.Deceleration * Time.deltaTime;
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