using System;
using System.Linq;
using UnityEngine;

namespace Scriptables.Shapes
{
    public enum Type
    {
        None,
        Spring,
        Rocket,
        Balloon,
        Drill,
        Parachute,
        Weight,
        Boomerang
    }

    [Serializable]
    public struct MovementSynergy
    {
        [SerializeField] private Actions.Type actionType;
        [SerializeField] private Type[] typeSynergies;
        
        public Actions.Type ActionType => actionType;
        public bool HasSynergyWith(Type type) => typeSynergies.Contains(type);
    }
    
    [CreateAssetMenu(fileName = "ShapeDatum", menuName = "Scriptable Objects/Shapes/ShapeDatum")]
    public class ShapeDatum : ScriptableObject
    {
        [SerializeField] private Type shapeType;
        [SerializeField] private MovementSynergy[] movementSynergies;
        
        public Type ShapeType => shapeType;
        public bool HasMovementSynergyWith( Actions.Type actionType, Type otherShapeType)
        {
            return movementSynergies.Any(x => x.ActionType == actionType && x.HasSynergyWith(otherShapeType));
        }
    }
}