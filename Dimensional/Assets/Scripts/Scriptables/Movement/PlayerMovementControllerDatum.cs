using UnityEngine;

namespace Scriptables.Movement
{
    [CreateAssetMenu(fileName = "PlayerMovementControllerDatum", menuName = "Scriptable Objects/Movement/PlayerMovementControllerDatum")]
    public class PlayerMovementControllerDatum : MovementControllerDatum
    {
        [Space(10)] 
        [SerializeField] private float jumpHeight;
        [SerializeField] private float jumpTime;
        [SerializeField] private AnimationCurve jumpCurve;
        [Space(10)] 
        [SerializeField] private float springJumpHeight;
        [SerializeField] private float springJumpTime;
        [SerializeField] private AnimationCurve springJumpCurve;
        [Space(10)] 
        [SerializeField] private float balloonJumpHeight;
        [SerializeField] private float balloonJumpTime;
        [SerializeField] private AnimationCurve balloonJumpCurve;
        [Space(10)] 
        [SerializeField] private float cutJumpMultiplier;
        [Space(10)] 
        [SerializeField] private float wallJumpHeight;
        [SerializeField] private float wallJumpTime;
        [SerializeField] private AnimationCurve wallJumpCurve;
        [Space(10)] 
        [SerializeField] private float groundPoundSpeed;
        [SerializeField] private float groundPoundTime;
        [SerializeField] private AnimationCurve groundPoundCurve;
        [Space(10)] 
        [SerializeField] private float afterGroundPoundTime;
        [Space(10)] 
        [SerializeField] private float boomerangHeight;
        [SerializeField] private float boomerangTime;
        [SerializeField] private float maxBoomerangSpeed;
        [SerializeField] private AnimationCurve boomerangCurve;
        [Space(10)] 
        [SerializeField] private float initialRollSpeed;
        [SerializeField] private MovementControllerDatum rollMovementControllerDatum;
        [Space(10)] 
        [SerializeField] private Vector3 attackVector;
        [SerializeField] private float attackTime;
        [SerializeField] private AnimationCurve attackCurve;
        
        public float JumpHeight => jumpHeight;
        public float JumpTime => jumpTime;
        public AnimationCurve JumpCurve => jumpCurve;
        
        public float SpringJumpHeight => springJumpHeight;
        public float SpringJumpTime => springJumpTime;
        public AnimationCurve SpringJumpCurve => springJumpCurve;
        
        public float BalloonJumpHeight => balloonJumpHeight;
        public float BalloonJumpTime => balloonJumpTime;
        public AnimationCurve BalloonJumpCurve => balloonJumpCurve;
        
        public float CutJumpMultiplier => cutJumpMultiplier;
        
        public float WallJumpHeight => wallJumpHeight;
        public float WallJumpTime => wallJumpTime;
        public AnimationCurve WallJumpCurve => wallJumpCurve;
        
        public float GroundPoundSpeed => groundPoundSpeed;
        public float GroundPoundTime => groundPoundTime;
        public AnimationCurve GroundPoundCurve => groundPoundCurve;
        
        public float AfterGroundPoundTime => afterGroundPoundTime;
        
        public float BoomerangHeight => boomerangHeight;
        public float BoomerangTime => boomerangTime;
        public float MaxBoomerangSpeed => maxBoomerangSpeed;
        public AnimationCurve BoomerangCurve => boomerangCurve;
        
        public float InitialRollSpeed => initialRollSpeed;
        public MovementControllerDatum RollMovementControllerDatum => rollMovementControllerDatum;

        public Vector3 AttackVector => attackVector;
        public float AttackTime => attackTime;
        public AnimationCurve AttackCurve => attackCurve;
    }
}
