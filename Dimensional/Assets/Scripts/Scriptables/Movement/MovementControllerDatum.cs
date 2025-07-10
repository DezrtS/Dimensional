using Managers;
using UnityEngine;

namespace Scriptables.Movement
{
    [CreateAssetMenu(fileName = "MovementControllerDatum", menuName = "Scriptable Objects/Movement/MovementControllerDatum")]
    public class MovementControllerDatum : ScriptableObject
    {
        [Header("Gravity Settings")]
        [SerializeField] private float gravityForce = 9.8f;
        [SerializeField] private float maxFallSpeed;
        [Space(15)] 
        [Header("Speed Settings")]
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private float deceleration;
        [SerializeField] private AnimationCurve decelerationCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [Space(10)]
        [SerializeField] private bool canAccelerateWhileDecelerating;
        [SerializeField] private float decelerationDotThreshold;
        [SerializeField] private float airborneMultiplier;
        
        public float GravityForce => gravityForce;
        public float MaxFallSpeed => maxFallSpeed;
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public AnimationCurve AccelerationCurve => accelerationCurve;
        public float Deceleration => deceleration;
        public AnimationCurve DecelerationCurve => decelerationCurve;
        public bool CanAccelerateWhileDecelerating => canAccelerateWhileDecelerating;
        public float DecelerationDotThreshold => decelerationDotThreshold;
        public float AirborneMultiplier => airborneMultiplier;
    }
}