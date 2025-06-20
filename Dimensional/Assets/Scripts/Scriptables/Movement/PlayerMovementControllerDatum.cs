using UnityEngine;

namespace Scriptables.Movement
{
    [CreateAssetMenu(fileName = "PlayerMovementControllerDatum", menuName = "Scriptable Objects/Movement/PlayerMovementControllerDatum")]
    public class PlayerMovementControllerDatum : MovementControllerDatum
    {
        [Space(10)] 
        [Header("Default Jump Settings")]
        [SerializeField] private float jumpHeight;
        [SerializeField] private float jumpTime;
        [SerializeField] private AnimationCurve jumpCurve;
        [Space(10)] 
        [Header("Spring Jump Settings")]
        [SerializeField] private float springJumpHeight;
        [SerializeField] private float springJumpTime;
        [SerializeField] private AnimationCurve springJumpCurve;
        [Space(10)] 
        [SerializeField] private float springJumpMediumPowerHeightMultiplier;
        [SerializeField] private float springJumpMediumPowerTimeMultiplier;
        [SerializeField] private float springJumpHighPowerHeightMultiplier;
        [SerializeField] private float springJumpHighPowerTimeMultiplier;
        [Space(10)] 
        [Header("Balloon Jump Settings")]
        [SerializeField] private float balloonJumpHeight;
        [SerializeField] private float balloonJumpTime;
        [SerializeField] private AnimationCurve balloonJumpCurve;
        [Space(10)]
        [Header("General Jump Settings")]
        [SerializeField] private float cutJumpMultiplier;
        [SerializeField] private float queueJumpTime;
        [SerializeField] private float coyoteTime;
        [Space(10)] 
        [Header("Wall Slide Settings")]
        [SerializeField] private float wallSlideMinExitAngle;
        [SerializeField] private float wallSlideSpeed;
        [SerializeField] private float wallSlideTime;
        [SerializeField] private AnimationCurve wallSlideCurve;
        [Space(10)] 
        [SerializeField] private float wallSlideCoyoteTime;
        [SerializeField] private float wallSlideYVelocityThreshold = 0.2f;
        [SerializeField] private float wallSlideChecks = 8;
        [SerializeField] private float wallSlideCheckDistance;
        [SerializeField] private Vector3 wallSlideCheckOffset;
        [SerializeField] private LayerMask wallSlideLayerMask;
        [Space(10)] 
        [Header("Wall Jump Settings")]
        [SerializeField] private float wallJumpSpeed;
        [SerializeField] private float wallJumpSpeedTime;
        [SerializeField] private AnimationCurve wallJumpSpeedCurve;
        [Space(10)]
        [SerializeField] private float wallJumpHeight;
        [SerializeField] private float wallJumpHeightTime;
        [SerializeField] private AnimationCurve wallJumpHeightCurve;
        [Space(10)] 
        [Header("Ground Pound Settings")]
        [SerializeField] private float groundPoundSpeed;
        [SerializeField] private float groundPoundTime;
        [SerializeField] private AnimationCurve groundPoundCurve;
        [Space(10)] 
        [SerializeField] private float groundPoundMediumPowerTimeThreshold;
        [SerializeField] private float groundPoundHighPowerTimeThreshold;
        [SerializeField] private float groundPoundSpringJumpTime;
        [Space(10)]
        [Header("Boomerang Settings")]
        [SerializeField] private float boomerangSlowDownSpeed;
        [SerializeField] private float boomerangSlowDownTime;
        [SerializeField] private AnimationCurve boomerangSlowDownCurve;
        [SerializeField] private AnimationCurve boomerangSlowDownMultiplierCurve;
        [Space(10)] 
        [SerializeField] private float boomerangFallSpeed;
        [SerializeField] private float boomerangFallTime;
        [SerializeField] private AnimationCurve boomerangFallCurve;
        [SerializeField] private MovementControllerDatum boomerangMovementControllerDatum;
        [Space(10)]
        [Header("Roll Settings")]
        [SerializeField] private float initialRollSpeed;
        [SerializeField] private MovementControllerDatum rollMovementControllerDatum;
        [Space(10)]
        [Header("Attack Settings")]
        [SerializeField] private Vector3 attackVector;
        [SerializeField] private float attackTime;
        [SerializeField] private AnimationCurve attackCurve;
        
        public float JumpHeight => jumpHeight;
        public float JumpTime => jumpTime;
        public AnimationCurve JumpCurve => jumpCurve;
        
        public float SpringJumpHeight => springJumpHeight;
        public float SpringJumpTime => springJumpTime;
        public AnimationCurve SpringJumpCurve => springJumpCurve;

        public float SpringJumpMediumPowerHeightMultiplier => springJumpMediumPowerHeightMultiplier;
        public float SpringJumpMediumPowerTimeMultiplier => springJumpMediumPowerTimeMultiplier;
        public float SpringJumpHighPowerHeightMultiplier => springJumpHighPowerHeightMultiplier;
        public float SpringJumpHighPowerTimeMultiplier => springJumpHighPowerTimeMultiplier;
        
        public float BalloonJumpHeight => balloonJumpHeight;
        public float BalloonJumpTime => balloonJumpTime;
        public AnimationCurve BalloonJumpCurve => balloonJumpCurve;
        
        public float CutJumpMultiplier => cutJumpMultiplier;
        public float QueueJumpTime => queueJumpTime;
        public float CoyoteTime => coyoteTime;
        
        public float WallSlideMinExitAngle => wallSlideMinExitAngle;
        public float WallSlideSpeed => wallSlideSpeed;
        public float WallSlideTime => wallSlideTime;
        public AnimationCurve WallSlideCurve => wallSlideCurve;
        
        public float WallSlideYVelocityThreshold => wallSlideYVelocityThreshold;
        public float WallSlideChecks => wallSlideChecks;
        public float WallSlideCheckDistance => wallSlideCheckDistance;
        public Vector3 WallSlideCheckOffset => wallSlideCheckOffset;
        public LayerMask WallSlideLayerMask => wallSlideLayerMask;

        public float WallJumpSpeed => wallJumpSpeed;
        public float WallJumpSpeedTime => wallJumpSpeedTime;
        public AnimationCurve WallJumpSpeedCurve => wallJumpSpeedCurve;
        
        public float WallJumpHeight => wallJumpHeight;
        public float WallJumpHeightTime => wallJumpHeightTime;
        public AnimationCurve WallJumpHeightCurve => wallJumpHeightCurve;
        
        public float WallSlideCoyoteTime => wallSlideCoyoteTime;
        
        public float GroundPoundSpeed => groundPoundSpeed;
        public float GroundPoundTime => groundPoundTime;
        public AnimationCurve GroundPoundCurve => groundPoundCurve;
        
        public float GroundPoundMediumPowerTimeThreshold => groundPoundMediumPowerTimeThreshold;
        public float GroundPoundHighPowerTimeThreshold => groundPoundHighPowerTimeThreshold;
        public float GroundPoundSpringJumpTime => groundPoundSpringJumpTime;

        public float BoomerangSlowDownSpeed => boomerangSlowDownSpeed;
        public float BoomerangSlowDownTime => boomerangSlowDownTime;
        public AnimationCurve BoomerangSlowDownCurve => boomerangSlowDownCurve;
        public AnimationCurve BoomerangSlowDownMultiplierCurve => boomerangSlowDownMultiplierCurve;
        
        public float BoomerangFallSpeed => boomerangFallSpeed;
        public float BoomerangFallTime => boomerangFallTime;
        public AnimationCurve BoomerangFallCurve => boomerangFallCurve;
        public MovementControllerDatum BoomerangMovementControllerDatum => boomerangMovementControllerDatum;
        
        public float InitialRollSpeed => initialRollSpeed;
        public MovementControllerDatum RollMovementControllerDatum => rollMovementControllerDatum;

        public Vector3 AttackVector => attackVector;
        public float AttackTime => attackTime;
        public AnimationCurve AttackCurve => attackCurve;
    }
}
