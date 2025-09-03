using Systems.Actions;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    [CreateAssetMenu(fileName = "PositionMovementActionDatum", menuName = "Scriptable Objects/Actions/Movement/PositionMovementActionDatum")]
    public class PositionMovementActionDatum : MovementActionDatum
    {
        [Space(10)] 
        [SerializeField] private bool stopMovementOnDisable;
        [SerializeField] private bool disableDurationLimit;
        [Space(10)]
        [SerializeField] private Vector3 movementVector;
        [Space(10)]
        [SerializeField] private bool hasRight;
        [SerializeField] private bool isXDistance;
        [SerializeField] private float rightDuration;
        [SerializeField] private AnimationCurve rightCurve;
        [Space(10)]
        [SerializeField] private bool hasUp;
        [SerializeField] private bool isYDistance;
        [SerializeField] private float upDuration;
        [SerializeField] private AnimationCurve upCurve;
        [Space(10)]
        [SerializeField] private bool hasForward;
        [SerializeField] private bool isZDistance;
        [SerializeField] private float forwardDuration;
        [SerializeField] private AnimationCurve forwardCurve;
        
        public bool StopMovementOnDisable => stopMovementOnDisable;
        public bool DisableDurationLimit => disableDurationLimit;
        
        public Vector3 MovementVector => movementVector;
        
        public bool HasRight => hasRight;
        public bool IsXDistance => isXDistance;
        public float RightDuration => rightDuration;
        public AnimationCurve RightCurve => rightCurve;
        
        public bool HasUp => hasUp;
        public bool IsYDistance => isYDistance;
        public float UpDuration => upDuration;
        public AnimationCurve UpCurve => upCurve;
        
        public bool HasForward => hasForward;
        public bool IsZDistance => isZDistance;
        public float ForwardDuration => forwardDuration;
        public AnimationCurve ForwardCurve => forwardCurve;
        
        public override Action AttachAction(GameObject actionHolder)
        {
            var positionMovementAction = actionHolder.AddComponent<PositionMovementAction>();
            positionMovementAction.Initialize(this);
            return positionMovementAction;
        }
    }
}