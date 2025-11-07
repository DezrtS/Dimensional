using UnityEngine;

namespace Scriptables.Movement
{
    [CreateAssetMenu(fileName = "PlayerMovementControllerDatum", menuName = "Scriptable Objects/Movement/PlayerMovementControllerDatum")]
    public class PlayerMovementControllerDatum : MovementControllerDatum
    {
        [Space(15)] 
        [Header("Jump Settings")]
        [SerializeField] private float cutJumpMultiplier;
        [SerializeField] private float queueJumpTime;
        [SerializeField] private float coyoteTime;
        [Space(15)] 
        [Header("Wall Jump Settings")]
        [SerializeField] private float wallJumpCoyoteTime;
        [Space(15)] 
        [Header("Wall Slide Settings")]
        [SerializeField] private float wallSlideCheckIntervals = 8;
        [SerializeField] private float wallSlideCheckDistance;
        [SerializeField] private Vector3 wallSlideCheckOffset;
        [SerializeField] private LayerMask wallSlideCheckLayerMask;
        [Space(10)] 
        [SerializeField] private float wallSlideYVelocityThreshold = 0.2f;
        [SerializeField] private float wallSlideMinEnterMagnitude = 0.5f;
        [SerializeField] private float wallSlideMinDirectionDot;
        [SerializeField] private float wallSlideMinExitAngle;
        [Space(15)]
        [Header("Roll Settings")]
        [SerializeField] private float initialRollSpeed;
        
        public float QueueJumpTime => queueJumpTime;
        public float CoyoteTime => coyoteTime;
        public float WallJumpCoyoteTime => wallJumpCoyoteTime;
        public float WallSlideCheckIntervals => wallSlideCheckIntervals;
        public float WallSlideCheckDistance => wallSlideCheckDistance;
        public Vector3 WallSlideCheckOffset => wallSlideCheckOffset;
        public LayerMask WallSlideCheckLayerMask => wallSlideCheckLayerMask;
        public float WallSlideYVelocityThreshold => wallSlideYVelocityThreshold;
        public float WallSlideMinEnterMagnitude => wallSlideMinEnterMagnitude;
        public float WallSlideMinDirectionDot => wallSlideMinDirectionDot;
        public float WallSlideMinExitAngle => wallSlideMinExitAngle;
        public float InitialRollSpeed => initialRollSpeed;
    }
}