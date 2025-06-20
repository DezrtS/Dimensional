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
        [SerializeField] private float deceleration;
        [Space(10)]
        [Header("Grounded Settings")]
        [SerializeField] private GroundedCheckType groundedCheckType;
        [SerializeField] private float groundedCheckDistance;
        [SerializeField] private Vector3 groundedCheckOffset;
        [SerializeField] private LayerMask groundedLayerMask;
        
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        
        public GroundedCheckType GroundedCheckType => groundedCheckType;
        public float GroundedCheckDistance => groundedCheckDistance;
        public Vector3 GroundedCheckOffset => groundedCheckOffset;
        public LayerMask GroundedLayerMask => groundedLayerMask;
    }
}