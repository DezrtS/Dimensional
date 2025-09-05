using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables.Actions.Movement;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Shapes
{
    public enum ShapeType
    {
        None,
        Sphere,
        Spring,
        Rocket,
        Balloon,
        Drill,
        Parachute,
        Weight,
        Boomerang,
        PaperAirplane,
    }
    
    [Serializable]
    public struct ShapeMovementAction
    {
        [SerializeField] private MovementActionType movementActionType;
        [SerializeField] private MovementActionDatum movementActionDatum;
        [SerializeField] private MovementSynergy[] movementSynergies;
        
        public MovementActionType MovementActionType => movementActionType;
        public MovementActionDatum MovementActionDatum => movementActionDatum;
        public MovementSynergy[] MovementSynergies => movementSynergies;
    }

    [Serializable]
    public struct MovementSynergy
    {
        [SerializeField] private ShapeType shapeType;
        [SerializeField] private MovementActionType movementActionType;
        [SerializeField] private MovementActionDatum movementActionDatum;
        
        public ShapeType ShapeType => shapeType;
        public MovementActionType MovementActionType => movementActionType;
        public MovementActionDatum MovementActionDatum => movementActionDatum;
    }
    
    [CreateAssetMenu(fileName = "ShapeDatum", menuName = "Scriptable Objects/Shapes/ShapeDatum")]
    public class ShapeDatum : ScriptableObject
    {
        [SerializeField] private ShapeType shapeType;
        [SerializeField] private ShapeMovementAction[] shapeMovementActions;
        
        public ShapeType ShapeType => shapeType;
        public ShapeMovementAction[] ShapeMovementActions => shapeMovementActions;

        public Dictionary<MovementActionType, MovementActionDatum> DefineMovementActions()
        {
            return shapeMovementActions.ToDictionary(action => action.MovementActionType, action => action.MovementActionDatum);
        }
    }
}