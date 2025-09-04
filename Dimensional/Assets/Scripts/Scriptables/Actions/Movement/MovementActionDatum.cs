using Scriptables.Movement;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    public abstract class MovementActionDatum : ActionDatum
    {
        [SerializeField] private MovementActionType movementActionType;
        [Space(10)] 
        [SerializeField] private bool hasMovementDatum;
        [SerializeField] private MovementControllerDatum movementControllerDatum;
        
        public MovementActionType MovementActionType => movementActionType;
        
        public bool HasMovementDatum => hasMovementDatum;
        public MovementControllerDatum MovementControllerDatum => movementControllerDatum;
    }
}
