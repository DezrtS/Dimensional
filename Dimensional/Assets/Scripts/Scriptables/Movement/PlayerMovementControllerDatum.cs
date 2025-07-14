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
        [Space(10)]
        [SerializeField] private float jumpHeight;
        [SerializeField] private float jumpTime;
        [SerializeField] private AnimationCurve jumpCurve;
        [Space(10)]
        [Header("Double Jump Settings")]
        [SerializeField] private float doubleJumpHeight;
        [SerializeField] private float doubleJumpTime;
        [SerializeField] private AnimationCurve doubleJumpCurve;
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
        [Space(15)] 
        [Header("Boomerang Settings")]
        [SerializeField] private float boomerangFallSpeedThreshold;
        [SerializeField] private float boomerangFallTimeThreshold; 
        [Space(10)] 
        [SerializeField] private float boomerangTime;
        [SerializeField] private AnimationCurve boomerangCurve;
        [Space(15)] 
        [Header("Glide Settings")] 
        [SerializeField] private float glideFallSpeedThreshold;
        [SerializeField] private float glideFallTimeThreshold;
        [Space(10)] 
        [SerializeField] private float glideFallSpeed;
        [SerializeField] private float glideSlowDownTime;
        [SerializeField] private AnimationCurve glideSlowDownCurve;
        [SerializeField] private MovementControllerDatum glideMovementControllerDatum;
        [Space(15)] 
        [Header("Grapple Settings")]
        [SerializeField] private float grappleTargetCheckRadius;
        [SerializeField] private float grappleTargetCheckMaxAngleDifference;
        [SerializeField] private LayerMask grappleTargetCheckLayerMask;
        [Space(10)] 
        [SerializeField] private float grappleSpeed;
        [SerializeField] private float grappleTime;
        [SerializeField] private AnimationCurve grappleCurve;
        [Space(15)] 
        [Header("Ground Pound Settings")]
        [SerializeField] private float groundPoundSpeed;
        [SerializeField] private float groundPoundTime;
        [SerializeField] private AnimationCurve groundPoundCurve;
        [Space(10)] 
        [SerializeField] private float groundPoundMediumPowerTimeThreshold;
        [SerializeField] private float groundPoundHighPowerTimeThreshold;
        [SerializeField] private float groundPoundSpringJumpTime;
        [Space(15)] 
        [Header("Wall Jump Settings")]
        [SerializeField] private float wallJumpCoyoteTime;
        [Space(10)]
        [SerializeField] private float wallJumpHeight;
        [SerializeField] private float wallJumpTime;
        [SerializeField] private AnimationCurve wallJumpCurve;
        [Space(10)]
        [Header("Wall Dash Settings")]
        [SerializeField] private float wallDashSpeed;
        [SerializeField] private float wallDashTime;
        [SerializeField] private AnimationCurve wallDashCurve;
        [Space(15)] 
        [Header("Wall Slide Settings")]
        [SerializeField] private float wallSlideCheckIntervals = 8;
        [SerializeField] private float wallSlideCheckDistance;
        [SerializeField] private Vector3 wallSlideCheckOffset;
        [SerializeField] private LayerMask wallSlideCheckLayerMask;
        [Space(10)] 
        [SerializeField] private float wallSlideYVelocityThreshold = 0.2f;
        [SerializeField] private float wallSlideMinExitAngle;
        [Space(10)] 
        [SerializeField] private float wallSlideSpeed;
        [SerializeField] private float wallSlideTime;
        [SerializeField] private AnimationCurve wallSlideCurve;
        [SerializeField] private MovementControllerDatum wallSlideMovementControllerDatum;
        [Space(15)] 
        [Header("Attack Settings")]
        [SerializeField] private Vector3 attackVector;
        [SerializeField] private float attackTime;
        [SerializeField] private AnimationCurve attackCurve;
        [Space(15)]
        [Header("Crouch Settings")]
        [SerializeField] private MovementControllerDatum crouchMovementControllerDatum;
        [Space(15)]
        [Header("Roll Settings")]
        [SerializeField] private float initialRollSpeed;
        [SerializeField] private MovementControllerDatum rollMovementControllerDatum;

        public float CutJumpMultiplier => cutJumpMultiplier;
        public float QueueJumpTime => queueJumpTime;
        public float CoyoteTime => coyoteTime;
        public float JumpHeight => jumpHeight;
        public float JumpTime => jumpTime;
        public AnimationCurve JumpCurve => jumpCurve;
        public float DoubleJumpHeight => doubleJumpHeight;
        public float DoubleJumpTime => doubleJumpTime;
        public AnimationCurve DoubleJumpCurve => doubleJumpCurve;
        public float SpringJumpHeight => springJumpHeight;
        public float SpringJumpTime => springJumpTime;
        public AnimationCurve SpringJumpCurve => springJumpCurve;
        public float SpringJumpMediumPowerHeightMultiplier => springJumpMediumPowerHeightMultiplier;
        public float SpringJumpMediumPowerTimeMultiplier => springJumpMediumPowerTimeMultiplier;
        public float SpringJumpHighPowerHeightMultiplier => springJumpHighPowerHeightMultiplier;
        public float SpringJumpHighPowerTimeMultiplier => springJumpHighPowerTimeMultiplier;
        public float BoomerangFallSpeedThreshold => boomerangFallSpeedThreshold;
        public float BoomerangFallTimeThreshold => boomerangFallTimeThreshold;
        public float BoomerangTime => boomerangTime;
        public AnimationCurve BoomerangCurve => boomerangCurve;
        public float GlideFallSpeedThreshold => glideFallSpeedThreshold;
        public float GlideFallTimeThreshold => glideFallTimeThreshold;
        public float GlideFallSpeed => glideFallSpeed;
        public float GlideSlowDownTime => glideSlowDownTime;
        public AnimationCurve GlideSlowDownCurve => glideSlowDownCurve;
        public MovementControllerDatum GlideMovementControllerDatum => glideMovementControllerDatum;
        public float GrappleTargetCheckRadius => grappleTargetCheckRadius;
        public float GrappleTargetCheckMaxAngleDifference => grappleTargetCheckMaxAngleDifference;
        public LayerMask GrappleTargetCheckLayerMask => grappleTargetCheckLayerMask;
        public float GrappleSpeed => grappleSpeed;
        public float GrappleTime => grappleTime;
        public AnimationCurve GrappleCurve => grappleCurve;
        public float GroundPoundSpeed => groundPoundSpeed;
        public float GroundPoundTime => groundPoundTime;
        public AnimationCurve GroundPoundCurve => groundPoundCurve;
        public float GroundPoundMediumPowerTimeThreshold => groundPoundMediumPowerTimeThreshold;
        public float GroundPoundHighPowerTimeThreshold => groundPoundHighPowerTimeThreshold;
        public float GroundPoundSpringJumpTime => groundPoundSpringJumpTime;
        public float WallJumpCoyoteTime => wallJumpCoyoteTime;
        public float WallJumpHeight => wallJumpHeight;
        public float WallJumpTime => wallJumpTime;
        public AnimationCurve WallJumpCurve => wallJumpCurve;
        public float WallDashSpeed => wallDashSpeed;
        public float WallDashTime => wallDashTime;
        public AnimationCurve WallDashCurve => wallDashCurve;
        public float WallSlideCheckIntervals => wallSlideCheckIntervals;
        public float WallSlideCheckDistance => wallSlideCheckDistance;
        public Vector3 WallSlideCheckOffset => wallSlideCheckOffset;
        public LayerMask WallSlideCheckLayerMask => wallSlideCheckLayerMask;
        public float WallSlideYVelocityThreshold => wallSlideYVelocityThreshold;
        public float WallSlideMinExitAngle => wallSlideMinExitAngle;
        public float WallSlideSpeed => wallSlideSpeed;
        public float WallSlideTime => wallSlideTime;
        public AnimationCurve WallSlideCurve => wallSlideCurve;
        public MovementControllerDatum WallSlideMovementControllerDatum => wallSlideMovementControllerDatum;
        public Vector3 AttackVector => attackVector;
        public float AttackTime => attackTime;
        public AnimationCurve AttackCurve => attackCurve;
        public MovementControllerDatum CrouchMovementControllerDatum => crouchMovementControllerDatum;
        public float InitialRollSpeed => initialRollSpeed;
        public MovementControllerDatum RollMovementControllerDatum => rollMovementControllerDatum;
    }
}