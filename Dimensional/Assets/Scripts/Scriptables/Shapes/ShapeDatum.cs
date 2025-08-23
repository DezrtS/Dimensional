using System;
using System.Linq;
using Scriptables.Actions;
using Scriptables.Movement;
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
    public struct Synergy
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
        [SerializeField] private Synergy[] synergies;
        [SerializeField] private Actions.Type[] actionTypes;
        [Space(10)]
        [SerializeField] private ValueTimeCurveActionDatum jumpActionDatum;
        [SerializeField] private ValueTimeCurveActionDatum doubleJumpActionDatum;
        [SerializeField] private ValueTimeCurveActionDatum wallJumpActionDatum;
        [SerializeField] private ValueTimeCurveActionDatum wallDashActionDatum;
        [SerializeField] private VectorTimeCurveActionDatum dashActionDatum;
        [SerializeField] private VectorTimeCurveActionDatum diveActionDatum;
        [Space(10)]
        [SerializeField] private MovementControllerDatum rollMovementControllerDatum;
        
        public Type ShapeType => shapeType;
        public bool HasSynergyWith(Actions.Type actionType, Type otherShapeType) => synergies.Any(x => x.ActionType == actionType && x.HasSynergyWith(otherShapeType));
        public bool HasActionType(Actions.Type actionType) => actionTypes.Contains(actionType);
    }
}