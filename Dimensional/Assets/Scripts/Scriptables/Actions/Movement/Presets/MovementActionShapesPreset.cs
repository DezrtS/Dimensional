using System;
using System.Collections.Generic;
using Scriptables.Shapes;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    [Serializable]
    public class MovementActionShape
    {
        [SerializeField] private MovementActionType movementActionType;
        [SerializeField] private ShapeType shapeType;
        
        public MovementActionType MovementActionType => movementActionType;
        public ShapeType ShapeType => shapeType;

        public MovementActionShape(MovementActionType movementActionType, ShapeType shapeType)
        {
            this.movementActionType = movementActionType;
            this.shapeType = shapeType;
        }
    }
    
    [CreateAssetMenu(fileName = "MovementActionShapesPreset", menuName = "Scriptable Objects/Actions/Movement/Presets/MovementActionShapesPreset")]
    public class MovementActionShapesPreset : ScriptableObject
    {
        [SerializeField] private string presetName;
        [SerializeField] private MovementActionShape[] movementActionShapes;
        
        public string PresetName => presetName;
        public MovementActionShape[] MovementActionShapes => movementActionShapes;
    }
}
