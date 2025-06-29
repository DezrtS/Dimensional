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
        [SerializeField] private float airborneMultiplier;
        [Space(10)]
        [Header("Grounded Settings")]
        [SerializeField] private GroundedCheckType groundedCheckType;
        [SerializeField] private float groundedCheckDistance;
        [SerializeField] private Vector3 groundedCheckOffset;
        [SerializeField] private LayerMask groundedLayerMask;
        
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public AnimationCurve AccelerationCurve => accelerationCurve;
        public float Deceleration => deceleration;
        public AnimationCurve DecelerationCurve => decelerationCurve;
        public float AirborneMultiplier => airborneMultiplier;
        
        public GroundedCheckType GroundedCheckType => groundedCheckType;
        public float GroundedCheckDistance => groundedCheckDistance;
        public Vector3 GroundedCheckOffset => groundedCheckOffset;
        public LayerMask GroundedLayerMask => groundedLayerMask;
    }
}