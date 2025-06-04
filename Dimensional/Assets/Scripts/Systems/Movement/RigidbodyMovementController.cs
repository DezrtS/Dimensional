using UnityEngine;

namespace Systems.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyMovementController : MovementController
    {
        [SerializeField] private bool disableYInput;
        
        protected override void OnMove(Vector3 input)
        {
            var trueInput = ForceController.GetRotation() * input;
            ForceController.ApplyForce(HandleMovement(trueInput), ForceMode.VelocityChange);
        }

        private Vector3 HandleMovement(Vector3 input)
        {
            if (disableYInput) input.y = 0;
            var currentVelocity =  ForceController.GetVelocity();
            var targetVelocity = input.normalized * MovementControllerDatum.MaxSpeed;
            var targetSpeed = targetVelocity.magnitude;

            var velocityDifference = targetVelocity - currentVelocity;
            if (disableYInput) velocityDifference.y = 0;
            var differenceDirection = velocityDifference.normalized;
            float accelerationIncrement;

            if (currentVelocity.magnitude <= targetSpeed)
            {
                accelerationIncrement = MovementControllerDatum.Acceleration * Time.deltaTime;
            }
            else
            {
                accelerationIncrement = MovementControllerDatum.Deceleration * Time.deltaTime;
            }

            if (velocityDifference.magnitude < accelerationIncrement) return velocityDifference;
            return differenceDirection * accelerationIncrement;
        }
    }
}