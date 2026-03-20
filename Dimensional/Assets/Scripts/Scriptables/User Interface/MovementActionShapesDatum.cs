using System;
using Scriptables.Shapes;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.User_Interface
{
    [CreateAssetMenu(fileName = "MovementActionShapesDatum", menuName = "Scriptable Objects/Shapes/MovementActionShapesDatum")]
    public class MovementActionShapesDatum : ScriptableObject
    {
        [SerializeField] private string movementActionName;
        [SerializeField] private Sprite movementActionIcon;
        [SerializeField] private MovementActionType movementActionType;
        [SerializeField] private ShapeDatum[] shapeData;
        
        public string MovementActionName => movementActionName;
        public Sprite MovementActionIcon => movementActionIcon;
        public MovementActionType MovementActionType => movementActionType;
        public ShapeDatum[] ShapeData => shapeData;
    }
}
