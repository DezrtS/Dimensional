using Systems.Actions;
using Systems.Actions.Movement;
using Systems.Movement;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    public abstract class MovementActionDatum : ActionDatum
    {
        [SerializeField] private MovementActionType movementActionType;
        
        public MovementActionType MovementActionType => movementActionType;
    }
}
