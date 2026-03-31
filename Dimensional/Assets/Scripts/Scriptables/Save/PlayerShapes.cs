using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables.Actions.Movement;
using Scriptables.Shapes;
using Scriptables.User_Interface;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Save
{
    [Serializable]
    public class MovementActionShapesPreset
    {
        public List<MovementActionShape> movementActionShapes;
    }
    
    [CreateAssetMenu(fileName = "PlayerShapes", menuName = "Scriptable Objects/Save/PlayerShapes")]
    public class PlayerShapes : SaveVariable
    {
        [Space]
        [SerializeField] private MovementActionShapesDatum[] movementActionShapesData;
        [SerializeField] private ShapeDatum[] shapeData;
        [Space]
        [SerializeField] private MovementActionShapesPreset defaultValue;
        
        private MovementActionShapesPreset _runtimeValue;
        
        private Dictionary<ShapeType, Dictionary<MovementActionType, MovementActionDatum>> _shapeActionDictionary;
        private Dictionary<MovementActionType, MovementActionDatum> _movementActionDictionary;
        
        public MovementActionShapesPreset Value
        {
            get
            {
                EnsureRuntime();
                return _runtimeValue;
            }
        }

        public void SetMovementActionShapesPreset(MovementActionType movementActionType, ShapeType shapeType)
        {
            foreach (var movementActionShapesPreset in _runtimeValue.movementActionShapes.Where(movementActionShapesPreset => movementActionShapesPreset.MovementActionType == movementActionType))
            {
                movementActionShapesPreset.ShapeType = shapeType;
            }

            SetupMovementActionDictionary();
            SetIsDirty();
        }
        
        private void EnsureRuntime()
        {
            if (_runtimeValue?.movementActionShapes == null) Reset();
        }

        public override string Capture()
        {
            base.Capture();
            return JsonUtility.ToJson(_runtimeValue);
        }

        public override void Restore(string data)
        {
            base.Restore(data);
            _runtimeValue = JsonUtility.FromJson<MovementActionShapesPreset>(data);
            SetupShapeActionDictionary();
        }

        public override void Reset()
        {
            base.Reset();
            _runtimeValue = new MovementActionShapesPreset
            {
                movementActionShapes = defaultValue.movementActionShapes
                    .Select(movementActionShape => new MovementActionShape(movementActionShape.MovementActionType, movementActionShape.ShapeType))
                    .ToList()
            };
            
            SetupShapeActionDictionary();
        }

        private void SetupShapeActionDictionary()
        {
            _shapeActionDictionary = new Dictionary<ShapeType, Dictionary<MovementActionType, MovementActionDatum>>();
            foreach (var shapeDatum in shapeData)
            {
                _shapeActionDictionary.Add(shapeDatum.ShapeType, shapeDatum.DefineMovementActions());
            }

            SetupMovementActionDictionary();
        }

        private void SetupMovementActionDictionary()
        {
            _movementActionDictionary = new Dictionary<MovementActionType, MovementActionDatum>();
            foreach (var movementActionShape in _runtimeValue.movementActionShapes.Where(movementActionShape => movementActionShape.ShapeType is not ShapeType.None))
            {
                _movementActionDictionary.Add(movementActionShape.MovementActionType, _shapeActionDictionary[movementActionShape.ShapeType][movementActionShape.MovementActionType]);
            }
        }

        public MovementActionDatum GetMovementActionDatum(MovementActionType movementActionType)
        {
            return _movementActionDictionary.GetValueOrDefault(movementActionType);
        }
    }
}
