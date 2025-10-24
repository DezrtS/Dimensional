using Scriptables.Movement;
using Systems.Actions;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    [CreateAssetMenu(fileName = "MovementActionDatum", menuName = "Scriptable Objects/Actions/Movement/MovementActionDatum")]
    public class MovementActionDatum : ActionDatum
    {
        [SerializeField] private MovementActionType movementActionType;
        [Space(10)] 
        [SerializeField] private bool performEventOnGrounded;
        [SerializeField] private ActionEventType groundedActionEventType;
        [Space(10)] 
        [SerializeField] private bool hasMovementDatum;
        [SerializeField] private MovementControllerDatum movementControllerDatum;
        
        public MovementActionType MovementActionType => movementActionType;
        
        public bool PerformEventOnGrounded => performEventOnGrounded;
        public ActionEventType GroundedActionEventType => groundedActionEventType;
        
        public bool HasMovementDatum => hasMovementDatum;
        public MovementControllerDatum MovementControllerDatum => movementControllerDatum;

        public override Action AttachAction(GameObject actionHolder)
        {
            var movementAction = actionHolder.AddComponent<MovementAction>();
            movementAction.Initialize(this);
            return movementAction;
        }
    }
}
