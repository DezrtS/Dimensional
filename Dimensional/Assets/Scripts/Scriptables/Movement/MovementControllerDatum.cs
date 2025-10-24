using Managers;
using UnityEngine;

namespace Scriptables.Movement
{
    [CreateAssetMenu(fileName = "MovementControllerDatum", menuName = "Scriptable Objects/Movement/MovementControllerDatum")]
    public class MovementControllerDatum : ScriptableObject
    {
        [Header("Speed Settings")]
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private float deceleration;
        [SerializeField] private AnimationCurve decelerationCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [Space(10)]
        [SerializeField] private bool canAccelerateWhileDecelerating;
        [SerializeField] private float decelerationDotThreshold;
        [SerializeField] private float airborneAccelerationMultiplier = 1;
        [SerializeField] private float airborneMaxSpeedMultiplier = 1;
        
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public AnimationCurve AccelerationCurve => accelerationCurve;
        public float Deceleration => deceleration;
        public AnimationCurve DecelerationCurve => decelerationCurve;
        public bool CanAccelerateWhileDecelerating => canAccelerateWhileDecelerating;
        public float DecelerationDotThreshold => decelerationDotThreshold;
        public float AirborneAccelerationMultiplier => airborneAccelerationMultiplier;
        public float AirborneMaxSpeedMultiplier => airborneMaxSpeedMultiplier;
    }
}