using UnityEngine;

namespace Systems.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyMovementController : MovementController
    {
        protected override void Move(Vector3 input)
        {
            var trueInput = ForceController.GetRotation() * input;
            ForceController.ApplyForce(HandleMovement(trueInput), ForceMode.VelocityChange);
        }

        private Vector3 HandleMovement(Vector3 input)
        {
            var currentVelocity =  ForceController.GetVelocity();
            var targetVelocity = input.normalized * MovementControllerDatum.MaxSpeed;
            var targetSpeed = targetVelocity.magnitude;

            var velocityDifference = targetVelocity - currentVelocity;
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